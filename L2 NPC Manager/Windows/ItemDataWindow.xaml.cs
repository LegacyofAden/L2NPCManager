using L2NPCManager.Data;
using L2NPCManager.Data.Items;
using L2NPCManager.IO;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace L2NPCManager.Windows
{
    public partial class ItemDataWindow : Window
    {
        private const int NEW_NPC_ID = 40000;

        public ItemManager ItemMgr {get; set;}

        private string filename;
        private ItemData selected_item;
        private ICollectionView item_list;

        private bool isLoading, hasPropertyChanges;


        public ItemDataWindow() {
            InitializeComponent();
            //
            this.filename = Paths.GetItemData();
            //
            setState(false);
            progress.Visibility = Visibility.Hidden;
            status.Content = "Initializing...";
        }

        //=============================

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            createItemList();
            setState(true);
            status.Content = "Ready";
        }

        private void Window_Closing(object sender, CancelEventArgs e) {
            checkSaveItem();
        }

        private void menuClose_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void lstItems_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (ItemMgr == null) return;
            checkSaveItem();
            //
            setSelectedItem((ItemData)lstItems.SelectedItem);
        }

        private void btnItemsSearch_Click(object sender, RoutedEventArgs e) {
            updateSearchFilter();
        }

        private void btnItemsAdd_Click(object sender, RoutedEventArgs e) {
            int id = getNewID();
            ItemData item = new ItemData();
            item.ID = id.ToString();
            item.Name = "[new_item_"+id+"]";
            item.UpdateDisplayValues();
            //
            ItemMgr.Items.Add(item);
            ItemMgr.HasChanges = true;
            lstItems.SelectedItem = item;
            lstItems.ScrollIntoView(item);
        }

        private void btnItemsClone_Click(object sender, RoutedEventArgs e) {
            ItemData sel_item = (ItemData)lstItems.SelectedItem;
            if (sel_item == null) return;
            //
            string name = StringUtils.getString(sel_item.Name);
            if (name == null) name = "";
            int id = getNewID();
            //
            ItemData item = new ItemData();
            sel_item.Clone(item);
            item.ID = id.ToString();
            item.Name = getCloneName(name);
            item.UpdateDisplayValues();
            //
            ItemMgr.Items.Add(item);
            ItemMgr.HasChanges = true;
            lstItems.SelectedItem = item;
            lstItems.ScrollIntoView(item);
        }

        private void txtID_TextChanged(object sender, TextChangedEventArgs e) {
            if (isLoading) return;
            if (!hasPropertyChanges) {
                ItemMgr.HasChanges = true;
                hasPropertyChanges = true;
            }
        }

        private void txtName_TextChanged(object sender, TextChangedEventArgs e) {
            if (isLoading) return;
            if (!hasPropertyChanges) {
                ItemMgr.HasChanges = true;
                hasPropertyChanges = true;
            }
        }

        private void txtItemSearch_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) updateSearchFilter();
        }

        //-----------------------------

        private void createItemList() {
            if (ItemMgr != null) {
                CollectionViewSource src_view = new CollectionViewSource() {Source = ItemMgr.Items};
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
            ItemData data = (ItemData)item;
            if (data.ID != null && data.ID.Contains(search)) return true;
            if (data.Name != null && data.Name.Contains(search)) return true;
            return false;
        }

        private void setSelectedItem(ItemData item) {
            this.selected_item = item;
            //
            if (item == null) clearData();
            else {
                try {loadItemData(item);}
                catch (Exception) {
                    MessageBox.Show("Failed to load Item data! May be corrupted.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        //-----------------------------

        private void setState(bool enabled) {
            txtItemSearch.IsEnabled = enabled;
            btnItemsSearch.IsEnabled = enabled;
            btnItemsAdd.IsEnabled = enabled;
            btnItemsClone.IsEnabled = enabled;
            lstItems.IsEnabled = enabled;
            //
            txtID.IsEnabled = enabled;
            txtName.IsEnabled = enabled;
            lstProperties.IsEnabled = enabled;
        }

        private string getCloneName(string name) {
            string f_name = name.Replace(' ', '_');
            int x = f_name.IndexOf("_clone_");
            if (x >= 0) f_name = f_name.Substring(0, x);
            //
            int i = 1;
            string n;
            bool exists = false;
            do {
                n = '['+f_name+"_clone_"+i+']';
                exists = (ItemMgr.FindItemByName(n) != null);
                i++;
            } while (exists);
            return n;
        }

        private int getNewID() {
            int i = NEW_NPC_ID;
            while (ItemMgr.FindItemByID(i.ToString()) != null) i++;
            return i;
        }

        private void checkSaveItem() {
            if (selected_item == null) return;
            //
            saveItemData(selected_item);
        }

        private void clearData() {
            isLoading = true;
            txtID.Text = null;
            txtName.Text = null;
            lstProperties.Clear();
            hasPropertyChanges = false;
            //
            isLoading = false;
        }

        private void loadItemData(ItemData item) {
            isLoading = true;
            txtID.Text = item.ID;
            txtName.Text = StringUtils.getString(item.Name);
            lstProperties.LoadItem(item);
            hasPropertyChanges = false;
            //
            isLoading = false;
        }

        private void saveItemData(ItemData data) {
            if (hasPropertyChanges) {
                Console.WriteLine("Saving Item Properties");
                data.ID = txtID.Text;
                data.Name = StringUtils.setString(txtName.Text);
                lstProperties.SaveItem(data);
                hasPropertyChanges = false;
                selected_item.UpdateDisplayValues();
                ItemMgr.HasChanges = true;
            }
        }
    }
}