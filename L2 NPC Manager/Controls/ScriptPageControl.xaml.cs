using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Search;
using L2NPCManager.AI.Completion;
using L2NPCManager.AI.Scripts;
using L2NPCManager.AI.Workers;
using L2NPCManager.Data;
using L2NPCManager.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace L2NPCManager.Controls
{
    public partial class ScriptPageControl : UserControl
    {
        public delegate void ScriptLoadedEvent(ScriptPageControl page);
        public event ScriptLoadedEvent OnScriptLoaded;

        public ScriptManager ScriptMgr {get; set;}
        public ScriptItem Script {get; private set;}
        public ObservableCollection<EventHandlerItem> Items {get; private set;}

        private ScriptDecompileWorker decompileTask;
        private bool is_loading, has_changes;
        private CompletionWindow completion;
        private SearchPanel search;


        public ScriptPageControl() {
            InitializeComponent();
            //
            search = SearchPanel.Install(txtScript);
            Items = new ObservableCollection<EventHandlerItem>();
            gridScriptError.Visibility = Visibility.Collapsed;
            //
            txtScript.TextArea.TextView.BackgroundRenderers.Add(new LineRenderer(txtScript));
            txtScript.TextArea.TextEntering += TextArea_TextEntering;
            txtScript.TextArea.TextEntered += TextArea_TextEntered;
            //
            UpdateSettings();
        }

        //=============================

        public void Clear() {
            setText(null);
            has_changes = false;
            Script = null;
        }

        public void Load(string name) {
            Clear();
            if (ScriptMgr == null) throw new Exception("A Script Manager has not been assigned!");
            try {Script = ScriptMgr.Get(name);}
            catch (Exception) {}
            //
            is_loading = true;
            if (Script != null) {
                if (ScriptManager.ENABLE_DECOMPILER) {
                    if (!Script.IsDecompiled) {
                        decompileTask = new ScriptDecompileWorker(Script.Source);
                        decompileTask.OnComplete += decompileTask_OnComplete;
                        decompileTask.Run(null);
                    } else {
                        setText(Script.Data);
                        parseEventHandlers();
                    }
                } else {
                    setText(Script.Source);
                }
                //
                gridScriptError.Visibility = Visibility.Collapsed;
            } else {
                gridScriptError.Visibility = Visibility.Visible;
                lblScriptErrorTitle.Text = "Warning:";
                lblScriptErrorMessage.Text = "Script \""+name+"\" not found!";
            }
            is_loading = false;
            has_changes = false;
        }

        public void Save() {
            if (!has_changes) return;
            if (ScriptMgr == null) throw new Exception("A Script Manager has not been assigned!");
            //
            if (Script == null) {
                Script = new ScriptItem();
                Script.IsDecompiled = true;
            }
            Script.Data = txtScript.Text;
            Script.IsModified = true;
            //
            Script.Name = Script.GetName();
            ScriptMgr.Set(Script);
            ScriptMgr.HasChanges = true;
            has_changes = false;
            //
            gridScriptError.Visibility = Visibility.Collapsed;
        }

        public string GetSuper() {
            if (Script == null) return null;
            return Script.GetSuper();
        }

        public void NavTo(EventHandlerItem item) {
            DocumentLine line = txtScript.Document.GetLineByOffset(item.Location);
            txtScript.Select(line.Offset, line.Length);
            txtScript.ScrollToLine(line.LineNumber);
        }

        public void ToggleSearch() {
            if (!search.IsVisible) search.Close();
            else search.Open();
        }

        public void UpdateSettings() {
            txtScript.FontSize = Settings.FontSize;
            //
            txtScript.FontFamily = MesloFont.Get();
        }

        //-----------------------------

        private void TextArea_TextEntering(object sender, TextCompositionEventArgs e) {
            // Inside string
            if (e.Text.Length < 1) return;
            if (isInString()) {
                if (e.Text == "\"") {
                    int p = txtScript.CaretOffset;
                    if (p > 0 && txtScript.Text[p-1] == '\\') return;
                    if (txtScript.Text[p] == '\"') {
                        txtScript.CaretOffset++;
                        e.Handled = true;
                    }
                }
                return;
            }
            //
            // Ignore matching characters
            if (txtScript.CaretOffset < txtScript.Text.Length) {
                char caretChar = txtScript.Text[txtScript.CaretOffset];
                if ((e.Text == "(" && caretChar == '(')
                   || (e.Text == ")" && caretChar == ')')
                   || (e.Text == "{" && caretChar == '{')
                   || (e.Text == "}" && caretChar == '}')
                   || (e.Text == "\"" && caretChar == '\"')
                   || (e.Text == "\'" && caretChar == '\'')) {
                    txtScript.CaretOffset++;
                    e.Handled = true;
                    return;
                }
            }
            //
            // Brackets + Newline -> Indent
            if (e.Text == "\n") {
                int p = txtScript.CaretOffset;
                if (txtScript.Text[p-1] == '{') {
                    bool has_end = (txtScript.Text[p] == '}');
                    DocumentLine line = txtScript.Document.GetLineByOffset(p);
                    string indent = getIndent(line);
                    //
                    txtScript.Document.Insert(p, "\n"+indent+"\t");
                    if (has_end) txtScript.Document.Insert(p+indent.Length+2, "\n"+indent);
                    txtScript.CaretOffset = p+indent.Length+2;
                    e.Handled = true;
                }
            }
            //
            // Auto-complete
            bool isLetterDigit = (char.IsLetterOrDigit(e.Text[0]) || e.Text == "_");
            if (completion != null) {
                if (!isLetterDigit) completion.CompletionList.RequestInsertion(e);
            } else {
                if (isLetterDigit) {
                    string[] prefix = getPrefix();
                    setCompletionItems(prefix);
                }
            }
        }

        private void TextArea_TextEntered(object sender, TextCompositionEventArgs e) {
            if (isInString()) {
                if (e.Text == "\"") {
                    int p = txtScript.CaretOffset;
                    if (p > 0 && txtScript.Text[p-2] == '\\') return;
                    txtScript.Document.Insert(p, "\"");
                    txtScript.CaretOffset = p;
                }
                return;
            }
            //
            if (e.Text == ".") {
                string[] prefix = getPrefix();
                if (prefix == null || prefix.Length == 0) return;
                setCompletionItems(prefix);
            } else if (e.Text == "(") {
                string[] prefix = getPrefix();
                if (prefix != null && prefix.Length > 0) {
                    InsightWindow info = new InsightWindow(txtScript.TextArea);
                    info.Content = findMethod(prefix);
                    info.Show();
                }
                //
                int p = txtScript.CaretOffset;
                txtScript.Document.Insert(p, ")");
                txtScript.CaretOffset = p;
            } else if (e.Text == "{") {
                int p = txtScript.CaretOffset;
                txtScript.Document.Insert(p, "}");
                txtScript.CaretOffset = p;
            }
        }

        private bool isInString() {
            int o = txtScript.CaretOffset;
            DocumentLine line = txtScript.Document.GetLineByOffset(o);
            string txt = txtScript.Text.Substring(line.Offset, o - line.Offset);
            //
            bool r = false;
            int c = txt.Length;
            for (int i = 0; i < c; i++) {
                if (txt[i] == '\"') {
                    if (i > 0 && txt[i-1] == '\\') continue;
                    r = !r;
                }
            }
            return r;
        }

        private void setCompletionItems(string[] prefix) {
            CompletionDocument doc = CompletionDocument.GetInstance();
            //
            IList<CompletionItem> items = doc.Find(prefix);
            if (items != null) {
                completion = new CompletionWindow(txtScript.TextArea);
                IList<ICompletionData> data = completion.CompletionList.CompletionData;
                foreach (CompletionItem i in items) data.Add(i);
                //
                completion.Show();
                completion.Closed += delegate {
                    completion = null;
                };
            }
        }

        private void decompileTask_OnComplete(bool cancelled, Exception error) {
            if (!cancelled) {
                if (error != null) {
                    gridScriptError.Visibility = Visibility.Visible;
                    lblScriptErrorTitle.Text = "Error:";
                    lblScriptErrorMessage.Text = "Failed to load AI script! "+error.Message;
                } else if (string.IsNullOrEmpty(decompileTask.Result)) {
                    gridScriptError.Visibility = Visibility.Visible;
                    lblScriptErrorTitle.Text = "Error:";
                    lblScriptErrorMessage.Text = "Failed to decompile AI script!";
                } else {
                    Script.Data = decompileTask.Result;
                    Script.IsDecompiled = true;
                    setText(Script.Data);
                    //
                    parseEventHandlers();
                }
            }
            //
            invokeScriptLoaded();
            has_changes = false;
        }

        private void txtScript_TextChanged(object sender, EventArgs e) {
            if (is_loading) return;
            if (!has_changes) has_changes = true;
        }

        //-----------------------------

        private void setText(string data) {
            is_loading = true;
            txtScript.Text = data;
            is_loading = false;
        }

        private string getIndent(DocumentLine line) {
            char c;
            int x = line.Offset;
            int y = x + line.TotalLength;
            for (int i = x; i < y; i++) {
                c = txtScript.Text[i];
                if (c != ' ' && c != '\t') {
                    return txtScript.Text.Substring(x, i - x);
                }
            }
            return string.Empty;
        }

        private string findMethod(string[] prefix) {
            return "(...)";
        }

        private string[] getPrefix() {
            int p = txtScript.CaretOffset;
            string prefix = findPrefixString(txtScript.Text, p);
            return prefix.Split('.');
        }

        private string findPrefixString(string text, int end_pos) {
            int i = end_pos;
            do {
                i--;
                if (isStopChar(text, i)) {
                    if (end_pos - i <= 1) return string.Empty;
                    return text.Substring(i+1, end_pos - i - 2);
                }
            }
            while (i > 0);
            return text;
        }

        private bool isStopChar(string text, int pos) {
            if (matches(text, " ", pos)) return true;
            if (matches(text, "\t", pos)) return true;
            if (matches(text, "\r\n", pos)) return true;
            if (matches(text, "(", pos)) return true;
            if (matches(text, ")", pos)) return true;
            if (matches(text, ";", pos)) return true;
            return false;
        }

        private bool matches(string text, string value, int pos) {
            int c = value.Length;
            for (int i = 0; i < c; i++) {
                if (!text[pos+i].Equals(value[i])) return false;
            }
            return true;
        }

        private void parseEventHandlers() {
            string pattern = @"\b\s*EventHandler\s+([\w_]+\([^\)]*\))\s*";
            MatchCollection matches = Regex.Matches(Script.Data, pattern, RegexOptions.Multiline);
            //
            EventHandlerItem item;
            Items.Clear();
            foreach (Match m in matches) {
                item = new EventHandlerItem();
                item.Name = m.Groups[1].Value;
                item.Location = m.Index;
                Items.Add(item);
            }
        }

        private void invokeScriptLoaded() {
            if (OnScriptLoaded != null) OnScriptLoaded(this);
        }
    }
}