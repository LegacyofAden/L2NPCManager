using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Windows.Media;

namespace L2NPCManager.AI.Completion
{
    public class CompletionItem : ICompletionData
    {
        public string Text {get; set;}
        public ImageSource Image {get {return CompletionIcons.Get(Type);}}
        public CompletionCollection Items {get; private set;}

        public object Content {get {return this.Text;}}
        public object Description {get; set;}
        public double Priority {get {return 0;}}
        public bool IsDeprecated {get; set;}
        public CompletionTypes Type {get; set;}

        public string Params {get; set;}
        public string ParamsDisplay {get {return (Params != null ? Params : "(...)");}}


        public CompletionItem() {
            Items = new CompletionCollection();
        }

        //=============================

        public CompletionCollection Find(string[] prefix, int level) {
            if (level >= prefix.Length) return Items;
            string p = prefix[level];
            //
            CompletionItem item;
            int c = Items.Count;
            for (int i = 0; i < c; i++) {
                item = Items[i];
                if (item.Text.Equals(p)) return item.Find(prefix, level + 1);
            }
            //
            return Items;
        }

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs e) {
            int offset = 0;
            string t = this.Text;
            //if (Type == CompletionTypes.Method) {
            //    t += "()";
            //    offset = -1;
            //}
            textArea.Document.Replace(completionSegment, t);
            if (offset != 0) textArea.Caret.Offset += offset;
            //
            InsightWindow info = new InsightWindow(textArea);
            info.Content = ParamsDisplay;
            info.Show();
        }
    }

    public enum CompletionTypes {
        None,
        Class,
        Event,
        Field,
        Method,
        Property,
        Struct,
    }
}