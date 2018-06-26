using L2NPCManager.Data;
using L2NPCManager.IO;
using L2NPCManager.IO.Extensions;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace L2NPCManager.Windows
{
    public partial class ClansWindow : Window
    {
        public NpcManager NpcMgr {get; set;}
        public string Data {get; set;}
        public bool HasChanges {get; private set;}

        private ObservableCollection<string> source_items, items;


        public ClansWindow() {
            InitializeComponent();
            //
            source_items = new ObservableCollection<string>();
            items = new ObservableCollection<string>();
            //
            lstItemsSource.ItemsSource = source_items;
            lstItems.ItemsSource = items;
        }

        //=============================

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            getItems();
            getSourceItems();
        }

        private void btnAddNew_Click(object sender, RoutedEventArgs e) {
            string name = txtNewItem.Text;
            if (string.IsNullOrEmpty(name)) {
                txtNewItem.Focus();
                return;
            }
            //
            if (items.Contains(name)) {
                MessageBox.Show("Item already used!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //
            items.Add(name);
            txtNewItem.Clear();
            txtNewItem.Focus();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e) {
            IList move_items = lstItemsSource.SelectedItems;
            items.AddAll(move_items);
            source_items.RemoveAll(move_items);
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e) {
            IList move_items = lstItems.SelectedItems;
            source_items.AddAll(move_items);
            items.RemoveAll(move_items);
        }

        private void txtNewItem_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Return || e.Key == Key.Enter) {
                btnAddNew_Click(null, null);
                e.Handled = true;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e) {
            Data = ClanHelper.Write(items.ToArray());
            HasChanges = true;
            Close();
        }

        //-----------------------------

        private void getSourceItems() {
            source_items.Clear();
            if (NpcMgr == null) return;
            //
            string[] tags;
            foreach (string i in NpcMgr.AvailableClans) {
                tags = ClanHelper.Parse(i);
                foreach (string x in tags) {
                    if (source_items.Contains(x)) continue;
                    if (items.Contains(x)) continue;
                    source_items.Add(x);
                }
            }
        }

        private void getItems() {
            items.Clear();
            //
            string[] tags = ClanHelper.Parse(Data);
            foreach (string i in tags) {
                items.Add(i);
            }
        }
    }
}