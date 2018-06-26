using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using L2NPCManager.AI;
using L2NPCManager.AI.Workers;
using L2NPCManager.Controls;
using L2NPCManager.Data;
using L2NPCManager.Data.Npc;
using L2NPCManager.IO;
using L2NPCManager.IO.Workers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml;

namespace L2NPCManager.Windows
{
    public partial class MainWindow : Window
    {
        private const int NEW_NPC_ID = 40000;

        private NpcManager npcMgr;
        private ScriptManager scriptMgr;
        private ItemManager itemMgr;
        private GeoManager geoMgr;

        private NpcData selected_item;
        private ItemDataWindow itemdata_window;
        private ICollectionView item_list;

        private bool isLoading, closeAfterSave;
        private bool hasPropertyChanges;
        private bool hasDropChanges, hasSpoilChanges;
        private double npcListWidth;


        public MainWindow() {
            try {loadHighlightingDefinition();}
            catch (Exception error) {
                Console.WriteLine("Failed to load highlighting definition! "+error.Message);
            }
            //
            InitializeComponent();
            //
            npcMgr = new NpcManager();
            itemMgr = new ItemManager();
            scriptMgr = new ScriptManager();
            geoMgr = new GeoManager();
            //
            lstProperties.NpcMgr = npcMgr;
            lstProperties.ScriptMgr = scriptMgr;
            scriptControl.Window = this;
            scriptControl.ScriptMgr = scriptMgr;
            position_ctrl.GeoMgr = geoMgr;
            //
            setState(false);
            progress.Visibility = Visibility.Hidden;
            status.Content = "Initializing...";
            lstProperties.OnChanged += lstProperties_OnChanged;
        }

        //=============================

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            LoadSettingsWorker settings_task;
            //
            status.Content = "Loading Settings...";
            settings_task = new LoadSettingsWorker();
            settings_task.OnComplete += settingsTask_OnComplete;
            settings_task.Run(null);
        }

        private void settingsTask_OnComplete(bool cancelled, Exception error) {
            status.Content = "Ready";
            progress.Visibility = Visibility.Collapsed;
            menuOpen.IsEnabled = true;
            //
            if (!cancelled && error != null) {
                MessageBox.Show("Failed to load settings! "+error.Message);
            }
            //
            Paths.DataPath = Settings.DataPath;
            if (checkDataPath()) loadAll();
        }

        private void Window_Closing(object sender, CancelEventArgs e) {
            checkSaveItem();
            //
            bool? r = checkSave();
            if (r.HasValue) {
                e.Cancel = true;
                if (r.Value) {
                    closeAfterSave = true;
                    saveAll();
                }
            }
            //
            if (loadTask != null && loadTask.IsActive) loadTask.Cancel();
        }

        //-----------------------------
        // Menu

        private void menuOpen_Click(object sender, RoutedEventArgs e) {
            checkSaveItem();
            //
            bool? r = checkSave();
            if (r.HasValue) {
                if (!r.Value) return;
                saveAll();
                return;
            }
            //
            if (Paths.GetDataPath()) {
                Settings.DataPath = Paths.DataPath;
                Settings.Save();
                loadAll();
            }
        }

        private void menuSave_Click(object sender, RoutedEventArgs e) {
            checkSaveItem();
            //
            saveAll();
        }

        private void menuExit_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void menuEditItemData_Click(object sender, RoutedEventArgs e) {
            if (itemdata_window == null || !itemdata_window.IsLoaded) {
                itemdata_window = new ItemDataWindow();
                itemdata_window.Owner = this;
                itemdata_window.ItemMgr = itemMgr;
                itemdata_window.Show();
            } else {
                itemdata_window.Activate();
            }
        }

        private void menuEditAdvanced_Click(object sender, RoutedEventArgs e) {
            bool enabled = menuEditAdvanced.IsChecked;
            txtID.IsReadOnly = !enabled;
            txtName.IsReadOnly = !enabled;
        }

        private void menuEditSettings_Click(object sender, RoutedEventArgs e) {
            SettingsWindow window = new SettingsWindow();
            window.Owner = this;
            bool? r = window.ShowDialog();
            if (r.HasValue && r.Value) {
                position_ctrl.UpdateSettings();
                scriptControl.UpdateSettings();
            }
        }

        private void menuAdjustRates_Click(object sender, RoutedEventArgs e) {
            checkSaveItem();
            AdjustRatesWindow window = new AdjustRatesWindow();
            window.Owner = this;
            window.NpcMgr = npcMgr;
            window.ShowDialog();
            if (window.HasChanges) {
                if (selected_item != null) setSelectedItem(selected_item);
            }
        }

        private void menuBalanceRemainders_Click(object sender, RoutedEventArgs e) {
            checkSaveItem();
            BalanceRemaindersWindow window = new BalanceRemaindersWindow();
            window.Owner = this;
            window.NpcMgr = npcMgr;
            window.ShowDialog();
            if (window.HasChanges) {
                if (selected_item != null) setSelectedItem(selected_item);
            }
        }

        //-----------------------------

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control) {
                checkSaveItem();
                saveAll();
            }
        }

        private void btnNpcListToggle_Click(object sender, RoutedEventArgs e) {
            if (colNpcList.Width.Value == 0) {
                colNpcList.Width = new GridLength(npcListWidth);
            } else {
                npcListWidth = colNpcList.Width.Value;
                colNpcList.Width = new GridLength(0);
            }
        }

        private void tabs_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (e.Source != tabs || npcMgr == null) return;
            checkSaveItem();
            //
            if (tabs.SelectedIndex == 1) position_ctrl.Start();
            else position_ctrl.Stop();
            //
            if (tabs.SelectedIndex == 2) {
                isLoading = true;
                scriptControl.Load(selected_item);
                isLoading = false;
            }
        }

        private void lstItems_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (npcMgr == null) return;
            checkSaveItem();
            //
            setSelectedItem((NpcData)lstItems.SelectedItem);
        }

        private void btnItemsSearch_Click(object sender, RoutedEventArgs e) {
            updateSearchFilter();
        }

        private void btnItemsAdd_Click(object sender, RoutedEventArgs e) {
            int id = getNewID();
            NpcData item = new NpcData();
            item.ID = id.ToString();
            item.Name = "[new_npc_"+id+"]";
            item.UpdateDisplayValues();
            //
            npcMgr.Items.Add(item);
            npcMgr.HasChanges = true;
            lstItems.SelectedItem = item;
            lstItems.ScrollIntoView(item);
        }

        private void btnItemsClone_Click(object sender, RoutedEventArgs e) {
            NpcData sel_item = (NpcData)lstItems.SelectedItem;
            if (sel_item == null) return;
            //
            string name = StringUtils.getString(sel_item.Name);
            if (name == null) name = "";
            int id = getNewID();
            //
            NpcData item = new NpcData();
            sel_item.Clone(item);
            item.ID = id.ToString();
            item.Name = getCloneName(name);
            item.UpdateDisplayValues();
            //
            npcMgr.Items.Add(item);
            npcMgr.HasChanges = true;
            lstItems.SelectedItem = item;
            lstItems.ScrollIntoView(item);
        }

        private void txtID_TextChanged(object sender, TextChangedEventArgs e) {
            if (isLoading) return;
            if (!hasPropertyChanges) {
                npcMgr.HasChanges = true;
                hasPropertyChanges = true;
            }
        }

        private void txtName_TextChanged(object sender, TextChangedEventArgs e) {
            if (isLoading) return;
            if (!hasPropertyChanges) {
                npcMgr.HasChanges = true;
                hasPropertyChanges = true;
            }
        }

        private void txtItemSearch_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) updateSearchFilter();
        }

        private void btnNewDropGroup_Click(object sender, RoutedEventArgs e) {
            NpcDropGroup data = new NpcDropGroup();
            data.Chance = "50";
            data.Items.Add(new NpcDropItem() {Min = 1, Max = 1});
            appendDropPanel(data);
            npcMgr.HasChanges = true;
            hasDropChanges = true;
        }

        private void lstProperties_OnChanged() {
            if (!hasPropertyChanges) {
                npcMgr.HasChanges = true;
                hasPropertyChanges = true;
            }
        }

        //-----------------------------
        // Drop Groups

        private GroupPanel appendDropPanel(NpcDropGroup data) {
            int i = lstDropGroups.Children.Count;
            GroupPanel panel = new GroupPanel();
            panel.Margin = new Thickness(2, 2, 2, 2);
            panel.SetIndex(i);
            panel.Load(data);
            panel.OnDelete += dropPanel_OnDelete;
            panel.OnChanged += dropPanel_OnChanged;
            lstDropGroups.Children.Add(panel);
            return panel;
        }

        private void dropPanel_OnChanged(GroupPanel sender) {
            hasDropChanges = true;
            npcMgr.HasChanges = true;
        }

        private void dropPanel_OnDelete(GroupPanel sender, NpcDropGroup data) {
            int index = lstDropGroups.Children.IndexOf(sender);
            if (index < 0) return;
            //
            if (data.Items.Count > 0) {
                MessageBoxResult r = MessageBox.Show("Are you sure you want to delete the current Drop group?", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (r == MessageBoxResult.No) return;
            }
            //
            GroupPanel panel;
            int c = lstDropGroups.Children.Count;
            for (int i = index+1; i < c; i++) {
                panel = (GroupPanel)lstDropGroups.Children[i];
                panel.SetIndex(i - 1);
            }
            //
            lstDropGroups.Children.RemoveAt(index);
            npcMgr.HasChanges = true;
            hasDropChanges = true;
        }

        //-----------------------------
        // Spoil Items

        private void lstSpoilItems_OnChanged(SpoilPanel sender) {
            hasSpoilChanges = true;
            npcMgr.HasChanges = true;
        }

        //-----------------------------

        private bool checkDataPath() {
            if (string.IsNullOrEmpty(Paths.DataPath) || !Directory.Exists(Paths.DataPath)) {
                if (!Paths.GetDataPath()) return false;
                Settings.DataPath = Paths.DataPath;
                Settings.Save();
            }
            //
            return true;
        }

        private void createItemList() {
            if (npcMgr != null) {
                CollectionViewSource src_view = new CollectionViewSource() { Source = npcMgr.Items };
                item_list = src_view.View;
                lstItems.ItemsSource = item_list;
            } else {
                lstItems.ItemsSource = null;
            }
        }

        private void updateSearchFilter() {
            item_list.Filter = search_filter;
        }

        private bool search_filter(object item) {
            string search = txtItemSearch.Text.Replace(' ', '_');
            NpcData data = (NpcData)item;
            if (data.ID != null && data.ID.Contains(search)) return true;
            if (data.Name != null && data.Name.Contains(search)) return true;
            return false;
        }

        private void setSelectedItem(NpcData item) {
            this.selected_item = item;
            //
            if (item == null) clearData();
            else {
                try {loadItemData(item);}
                catch (Exception error) {
                    Console.WriteLine("Failed to load NPC item! "+error.Message);
                    MessageBox.Show("Failed to load NPC data! May be corrupted.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        //-----------------------------

        private void setState(bool enabled) {
            bool has_npc = (npcMgr != null);
            //
            menuOpen.IsEnabled = enabled;
            menuSave.IsEnabled = enabled;
            menuSave.IsEnabled = enabled;
            menuEditItemData.IsEnabled = enabled;
            //
            menuAdjustRates.IsEnabled = enabled && has_npc;
            menuBalanceRemainders.IsEnabled = enabled && has_npc;
            //
            txtItemSearch.IsEnabled = enabled && has_npc;
            btnItemsSearch.IsEnabled = enabled && has_npc;
            btnItemsAdd.IsEnabled = enabled && has_npc;
            btnItemsClone.IsEnabled = enabled && has_npc;
            lstItems.IsEnabled = enabled && has_npc;
            //
            txtID.IsEnabled = enabled && has_npc;
            txtName.IsEnabled = enabled && has_npc;
            lstProperties.setContentEnabled(enabled && has_npc);
            //
            btnNewDropGroup.IsEnabled = enabled && has_npc;
            lstDropGroups.IsEnabled = enabled && has_npc;
            lstSpoilItems.IsEnabled = enabled && has_npc;
            //
            scriptControl.IsEnabled = enabled && has_npc;
        }

        private void loadHighlightingDefinition() {
            IHighlightingDefinition nascHighlighting;
            using (Stream s = this.GetType().Assembly.GetManifestResourceStream("L2NPCManager.Resources.nasc.xshd")) {
                if (s == null) throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s)) {
                    nascHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            //
            HighlightingManager.Instance.RegisterHighlighting("NASC", new string[] { ".nasc" }, nascHighlighting);
        }

        private string getCloneName(string name) {
            string f_name = name.Replace(' ', '_');
            int x = f_name.IndexOf("_clone_");
            if (x >= 0) f_name = f_name.Substring(0, x);
            //
            int i = 1;
            string n;
            bool exists;
            do {
                n = '['+f_name+"_clone_"+i+']';
                exists = (npcMgr.FindItemByName(n) != null);
                i++;
            } while (exists);
            return n;
        }

        private int getNewID() {
            int i = NEW_NPC_ID;
            while (npcMgr.FindItemByID(i.ToString()) != null) i++;
            return i;
        }

        private bool? checkSave() {
            bool has_changes = false;
            if (npcMgr != null && npcMgr.HasChanges) has_changes = true;
            if (itemMgr != null && itemMgr.HasChanges) has_changes = true;
            if (scriptMgr.HasChanges) has_changes = true;
            if (!has_changes) return null;
            //
            MessageBoxResult r = MessageBox.Show("Would you like to save your changes first?", "Warning!", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if (r == MessageBoxResult.Cancel) return false;
            else if (r == MessageBoxResult.Yes) return true;
            return null;
        }

        private void checkSaveItem() {
            if (selected_item != null) saveItemData(selected_item);
        }

        private void clearData() {
            isLoading = true;
            //
            txtID.Text = null;
            txtName.Text = null;
            lstProperties.Clear();
            hasPropertyChanges = false;
            //
            lstDropGroups.Children.Clear();
            hasDropChanges = false;
            //
            lstSpoilItems.Clear();
            hasSpoilChanges = false;
            //
            scriptControl.Clear();
            //
            isLoading = false;
        }

        private void loadItemData(NpcData data) {
            isLoading = true;
            loadProperties(data);
            loadDropGroups(data);
            loadSpoilItems(data);
            if (tabs.SelectedIndex == 2) {
                scriptControl.Load(data);
            }
            isLoading = false;
        }

        private void loadProperties(NpcData data) {
            hasPropertyChanges = false;
            txtID.Text = data.ID;
            txtName.Text = StringUtils.getString(data.Name);
            lstProperties.LoadItem(data);
        }

        private void loadDropGroups(NpcData data) {
            hasDropChanges = false;
            lstDropGroups.Children.Clear();
            List<NpcDropGroup> drop_groups = data.GetDropGroups();
            if (drop_groups != null) {
                foreach (NpcDropGroup i in drop_groups) appendDropPanel(i);
            }
        }

        private void loadSpoilItems(NpcData data) {
            hasSpoilChanges = false;
            lstSpoilItems.Load(data.GetSpoilItems());
        }

        private void saveItemData(NpcData data) {
            if (hasPropertyChanges) {
                Console.WriteLine("Saving NPC Properties");
                data.ID = txtID.Text;
                data.Name = StringUtils.setString(txtName.Text);
                lstProperties.SaveItem(data);
                hasPropertyChanges = false;
                selected_item.UpdateDisplayValues();
                npcMgr.HasChanges = true;
                //
                npcMgr.addAvailableType(data.Type);
                npcMgr.addAvailableClan(data.GetValue(NpcData.VAR_CLAN, null));
                npcMgr.addAvailableIgnoreClan(data.GetValue(NpcData.VAR_IGNORE_CLAN_LIST, null));
                npcMgr.addAvailableRace(data.GetValue(NpcData.VAR_RACE, null));
                npcMgr.addAvailableBaseAttackType(data.GetValue(NpcData.VAR_BASE_ATTACK_TYPE, null));
            }
            //
            if (hasDropChanges) {
                Console.WriteLine("Saving NPC Drop Groups");
                saveDropGroups(data);
                hasDropChanges = false;
                npcMgr.HasChanges = true;
            }
            //
            if (hasSpoilChanges) {
                Console.WriteLine("Saving NPC Spoils");
                NpcSpoilGroup spoil_items = lstSpoilItems.Write();
                data.SetSpoilItems(spoil_items);
                hasSpoilChanges = false;
                npcMgr.HasChanges = true;
            }
            //
            scriptControl.Save();
        }

        private void saveDropGroups(NpcData data) {
            GroupPanel panel;
            List<NpcDropGroup> drop_groups = new List<NpcDropGroup>();
            int c = lstDropGroups.Children.Count;
            for (int i = 0; i < c; i++) {
                panel = (GroupPanel)lstDropGroups.Children[i];
                drop_groups.Add(panel.Write());
            }
            data.SetDropGroups(drop_groups);
        }

        //=============================
        // Load & Save

        private LoadWorker loadTask;
        private SaveWorker saveTask;
        private CompileWorker compileTask;

        private void loadAll() {
            setState(false);
            status.Content = "Loading...";
            progress.Visibility = Visibility.Visible;
            //
            loadTask = new LoadWorker();
            loadTask.NpcMgr = npcMgr;
            loadTask.ItemMgr = itemMgr;
            loadTask.ScriptMgr = scriptMgr;
            loadTask.GeoMgr = geoMgr;
            loadTask.OnStatusChanged += loadTask_OnStatusChanged;
            loadTask.OnComplete += loadTask_OnComplete;
            loadTask.Run(loadTask_Progress);
        }

        private void loadTask_OnStatusChanged(string text, bool is_indeterminate) {
            status.Content = text;
            progress.IsIndeterminate = is_indeterminate;
        }

        private void loadTask_Progress() {
            progress.Value = loadTask.GetProgress();
        }

        private void loadTask_OnComplete(bool cancelled, Exception error) {
            progress.Visibility = Visibility.Hidden;
            //
            if (!cancelled) {
                if (error != null) {
                    status.Content = "Error!";
                    MessageBox.Show("Failed to load documents! "+error.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                } else {
                    status.Content = "Loaded at "+DateTime.Now.ToShortTimeString();
                    lstProperties.Load();
                    //
                    if (!loadTask.LoadedAll) {
                        if (!loadTask.LoadedAny) {
                            MessageBox.Show("No data was found in the selected directory!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                        } else {
                            StringBuilder data = new StringBuilder();
                            if (!loadTask.LoadedNPC) {
                                if (data.Length > 0) data.AppendLine();
                                data.Append("NPC Data not found!");
                            }
                            if (!loadTask.LoadedItem) {
                                if (data.Length > 0) data.AppendLine();
                                data.Append("Item Data not found!");
                            }
                            if (!loadTask.LoadedAI) {
                                if (data.Length > 0) data.AppendLine();
                                data.Append("AI Scripts not found!");
                            }
                            if (!loadTask.LoadedGeo) {
                                if (data.Length > 0) data.AppendLine();
                                data.Append("Npc Position Data not found!");
                            }
                            MessageBox.Show(data.ToString(), "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    //
                    if (tabs.SelectedItem == tab_position) {
                        position_ctrl.Start();
                    }
                }
                createItemList();
            } else {
                status.Content = null;
            }
            //
            setState(true);
        }

        private bool saveAll() {
            setState(false);
            status.Content = "Saving...";
            progress.Visibility = Visibility.Visible;
            //
            saveTask = new SaveWorker();
            saveTask.NpcMgr = npcMgr;
            saveTask.ItemMgr = itemMgr;
            saveTask.ScriptMgr = scriptMgr;
            saveTask.OnStatusChanged += saveTask_OnStatusChanged;
            saveTask.OnComplete += saveTask_OnComplete;
            saveTask.Run(saveTask_Progress);
            return true;
        }

        private void saveTask_OnStatusChanged(string text, bool is_indeterminate) {
            status.Content = text;
            progress.IsIndeterminate = is_indeterminate;
        }

        private void saveTask_Progress() {
            progress.Value = saveTask.GetProgress();
        }

        private void saveTask_OnComplete(bool cancelled, Exception error) {
            setState(true);
            progress.Visibility = Visibility.Hidden;
            //
            if (!cancelled) {
                if (error != null) {
                    status.Content = "Error!";
                    MessageBox.Show("Failed to save documents! "+error.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                } else {
                    status.Content = "Saved at "+DateTime.Now.ToShortTimeString();
                    //
                    CompilerParser r = saveTask.CompilerResult;
                    if (r != null && !r.IsSuccess) {
                        CompilerOutputWindow window = new CompilerOutputWindow();
                        window.Owner = this;
                        window.Result = r;
                        window.Show();
                    } else {
                        scriptMgr.HasChanges = false;
                        if (closeAfterSave) Close();
                    }
                }
            } else {
                status.Content = null;
            }
        }

        public void CompileScripts() {
            setState(false);
            status.Content = "Compiling...";
            progress.Visibility = Visibility.Visible;
            //
            compileTask = new CompileWorker();
            compileTask.ScriptMgr = scriptMgr;
            compileTask.OnStatusChanged += compileTask_OnStatusChanged;
            compileTask.OnComplete += compileTask_OnComplete;
            compileTask.Run(compileTask_Progress);
        }

        private void compileTask_OnStatusChanged(string text, bool is_indeterminate) {
            status.Content = text;
            progress.IsIndeterminate = is_indeterminate;
        }

        private void compileTask_Progress() {
            progress.Value = compileTask.GetProgress();
        }

        private void compileTask_OnComplete(bool cancelled, Exception error) {
            setState(true);
            progress.Visibility = Visibility.Hidden;
            //
            if (!cancelled) {
                if (error != null) {
                    status.Content = "Error!";
                    MessageBox.Show("Failed to compile AI scripts! "+error.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                } else {
                    status.Content = "Compiled at "+DateTime.Now.ToShortTimeString();
                    //
                    CompilerParser r = compileTask.CompilerResult;
                    if (r != null && !r.IsSuccess) {
                        CompilerOutputWindow window = new CompilerOutputWindow();
                        window.Owner = this;
                        window.Result = r;
                        window.Show();
                    } else {
                        scriptMgr.HasChanges = false;
                    }
                }
            } else {
                status.Content = null;
            }
        }

        //-----------------------------

        class LoadSettingsWorker : ProgressWorker {
            protected override void Process() {
                Settings.Load();
            }
        }

        class LoadWorker : ProgressWorker {
            public delegate void StatusChangedEvent(string text, bool is_indeterminate);
            public event StatusChangedEvent OnStatusChanged;

            public NpcManager NpcMgr {get; set;}
            public ItemManager ItemMgr {get; set;}
            public ScriptManager ScriptMgr {get; set;}
            public GeoManager GeoMgr {get; set;}
            public bool LoadedNPC {get; private set;}
            public bool LoadedItem {get; private set;}
            public bool LoadedAI {get; private set;}
            public bool LoadedGeo {get; private set;}
            public bool LoadedAll {get; private set;}
            public bool LoadedAny {get; private set;}


            protected override void Process() {
                string npc_filename = Paths.GetNPCData();
                string item_filename = Paths.GetItemData();
                string ai_filename = Paths.GetAIData();
                string geo_filename = Paths.GetNpcPositionData();
                //
                LoadedNPC = false;
                LoadedItem = false;
                LoadedAI = false;
                LoadedGeo = false;
                LoadedAll = false;
                LoadedAny = false;
                //
                if (cancelPending) return;
                if (File.Exists(npc_filename)) {
                    setStatus("Loading NPC Data...", false);
                    NpcMgr.Load(npc_filename, npc_Progress);
                    LoadedNPC = true;
                    LoadedAny = true;
                }
                //
                if (cancelPending) return;
                if (File.Exists(item_filename)) {
                    setStatus("Loading Item Data...", false);
                    ItemMgr.Load(item_filename, item_Progress);
                    LoadedItem = true;
                    LoadedAny = true;
                }
                //
                if (cancelPending) return;
                if (File.Exists(ai_filename)) {
                    setStatus("Loading AI Scripts...", false);
                    ScriptParser parser = new ScriptParser();
                    parser.OnProgress += ai_Progress;
                    parser.Load(ai_filename);
                    ScriptMgr.Header = parser.Header;
                    ScriptMgr.Items = parser.Items;
                    LoadedAI = true;
                    LoadedAny = true;
                }
                //
                if (cancelPending) return;
                if (File.Exists(geo_filename)) {
                    setStatus("Loading NPC Position Data...", false);
                    GeoMgr.Load(geo_filename, geo_Progress);
                    LoadedGeo = true;
                    LoadedAny = true;
                }
                //
                LoadedAll = (LoadedNPC && LoadedItem && LoadedAI && LoadedGeo);
            }

            private void npc_Progress(double progress) {
                setProgress((int)(progress * 100));
            }

            private void item_Progress(double progress) {
                setProgress((int)(progress * 100));
            }

            private void ai_Progress(int progress, string class_name) {
                setProgress(progress);
            }

            private void geo_Progress(double progress) {
                setProgress((int)(progress * 100));
            }

            private void setStatus(string text, bool is_indeterminate) {
                if (OnStatusChanged != null) dispatch.BeginInvoke(OnStatusChanged, text, is_indeterminate);
            }
        }

        class SaveWorker : ProgressWorker {
            public delegate void StatusChangedEvent(string text, bool is_indeterminate);
            public event StatusChangedEvent OnStatusChanged;

            public ScriptManager ScriptMgr {get; set;}
            public NpcManager NpcMgr {get; set;}
            public ItemManager ItemMgr {get; set;}
            public CompilerParser CompilerResult {get; set;}

            private bool is_compiled;


            protected override void Process() {
                if (NpcMgr.HasChanges) {
                    string npc_filename = Paths.GetNPCData();
                    setStatus("Saving NPC Data...", false);
                    BackupHelper.Create(npc_filename);
                    NpcMgr.Save(npc_filename, npc_Progress);
                }
                //
                if (ItemMgr.HasChanges) {
                    string item_filename = Paths.GetItemData();
                    setStatus("Saving Item Data...", false);
                    BackupHelper.Create(item_filename);
                    ItemMgr.Save(item_filename, item_Progress);
                }
                //
                if (ScriptMgr.HasChanges) {
                    string ai_filename = Paths.GetAIData();
                    setStatus("Compiling AI Scripts...", true);
                    BackupHelper.Create(ai_filename);
                    is_compiled = false;
                    CompilerResult = ScriptMgr.Compile(ai_Progress);
                }
            }

            private void npc_Progress(double progress) {
                setProgress((int)(progress * 100));
            }

            private void item_Progress(double progress) {
                setProgress((int)(progress * 100));
            }

            private void ai_Progress(int progress) {
                setProgress(progress);
                //
                if (progress > 0 && !is_compiled) {
                    setStatus("Saving AI Scripts...", false);
                    is_compiled = true;
                }
            }

            private void setStatus(string text, bool is_indeterminate) {
                if (OnStatusChanged != null) dispatch.BeginInvoke(OnStatusChanged, text, is_indeterminate);
            }
        }

        class CompileWorker : ProgressWorker {
            public delegate void StatusChangedEvent(string text, bool is_indeterminate);
            public event StatusChangedEvent OnStatusChanged;

            public ScriptManager ScriptMgr {get; set;}
            public CompilerParser CompilerResult {get; set;}

            private bool is_compiled;


            protected override void Process() {
                if (ScriptMgr.HasChanges) {
                    setStatus("Compiling AI Scripts...", true);
                    BackupHelper.Create(Paths.GetAIData());
                    is_compiled = false;
                    CompilerResult = ScriptMgr.Compile(ai_Progress);
                }
            }

            private void ai_Progress(int progress) {
                setProgress(progress);
                //
                if (progress > 0 && !is_compiled) {
                    setStatus("Saving AI Scripts...", false);
                    is_compiled = true;
                }
            }

            private void setStatus(string text, bool is_indeterminate) {
                if (OnStatusChanged != null) dispatch.BeginInvoke(OnStatusChanged, text, is_indeterminate);
            }
        }
    }
}