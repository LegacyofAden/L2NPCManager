using L2NPCManager.AI.Completion;
using L2NPCManager.AI.Scripts;
using L2NPCManager.Data;
using L2NPCManager.Data.Npc;
using L2NPCManager.Windows;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace L2NPCManager.Controls
{
    public partial class ScriptControl : UserControl
    {
        public MainWindow Window {get; set;}
        public ScriptManager ScriptMgr {get; set;}
        public bool HasChanges {get; private set;}
        private ScriptPageControl current_page;

        private ObservableCollection<ScriptItem> scripts;
        private bool is_loading;


        public ScriptControl() {
            InitializeComponent();
            //
            try {CompletionIcons.PreLoad();}
            catch (Exception error) {
                Console.WriteLine("Failed to pre-load intellisense icons! "+error.Message);
            }
            //
            scripts = new ObservableCollection<ScriptItem>();
            lstScript.ItemsSource = scripts;
        }

        //=============================

        private void setPage(ScriptItem script) {
            bool is_page;
            foreach (ScriptPageControl c in list.Children) {
                is_page = (c.Script == script);
                c.Visibility = (is_page ? Visibility.Visible : Visibility.Collapsed);
                if (is_page) {
                    current_page = c;
                    lstItems.ItemsSource = c.Items;
                    lstItems.IsEnabled = (c.Items.Count > 0);
                }
            }
        }

        public void Load(NpcData data) {
            Clear();
            if (data == null) {
                HasChanges = false;
                return;
            }
            //
            string var_ai = data.GetValue(NpcData.VAR_NPC_AI, null);
            ScriptProperty prop = ScriptProperty.FromString(var_ai);
            string name = prop.ScriptName;
            //
            if (!string.IsNullOrEmpty(name)) {
                ScriptPageControl page, first_page;
                first_page = page = addTab(name);
                //
                string current, super;
                bool has_super;
                current = name;
                do {
                    super = page.GetSuper();
                    has_super = (!string.IsNullOrEmpty(super) && super != current && super != "(null)");
                    if (has_super) page = addTab(super);
                }
                while (page != null && has_super);
                //
                is_loading = true;
                current_page = first_page;
                lstScript.SelectedItem = first_page.Script;
                setPage(first_page.Script);
                is_loading = false;
            }
            //
            HasChanges = false;
        }

        public void Save() {
            foreach (ScriptPageControl page in list.Children) page.Save();
        }

        public void Clear() {
            is_loading = true;
            scripts.Clear();
            list.Children.Clear();
            current_page = null;
            is_loading = false;
        }

        public void UpdateSettings() {
            foreach (ScriptPageControl page in list.Children) page.UpdateSettings();
        }

        //-----------------------------

        private ScriptPageControl addTab(string name) {
            ScriptPageControl page = new ScriptPageControl();
            page.OnScriptLoaded += page_OnScriptLoaded;
            page.ScriptMgr = ScriptMgr;
            page.Load(name);
            list.Children.Add(page);
            scripts.Insert(0, page.Script);
            return page;
        }

        //-----------------------------

        private void page_OnScriptLoaded(ScriptPageControl page) {
            if (page == current_page) {
                lstLanguage.SelectedIndex = (page.Script.IsDecompiled ? 1 : 0);
                lstItems.ItemsSource = page.Items;
                lstItems.IsEnabled = (page.Items.Count > 0);
            }
        }

        private void lstScript_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (is_loading) return;
            setPage((ScriptItem)lstScript.SelectedItem);
        }

        private void lstItems_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            EventHandlerItem item = (EventHandlerItem)lstItems.SelectedItem;
            if (item != null && current_page != null) current_page.NavTo(item);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e) {
            if (current_page != null) current_page.ToggleSearch();
        }

        private void btnCompile_Click(object sender, RoutedEventArgs e) {
            Save();
            Window.CompileScripts();
        }
    }
}