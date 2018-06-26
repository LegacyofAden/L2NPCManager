using L2NPCManager.Data.Items;
using System.Windows;
using System.Windows.Controls;

namespace L2NPCManager.Controls
{
    public partial class ItemPropertiesPanel : UserControl
    {
        public delegate void ChangedEvent();
        public event ChangedEvent OnChanged;

        private ItemDataDocument document;
        private bool isLoading;
        private bool hasChanges;


        public ItemPropertiesPanel() {
            InitializeComponent();
        }

        //=============================

        public void Load(ItemDataDocument document) {
            this.document = document;
        }

        public void LoadItem(ItemData item) {
            isLoading = true;
            lstTypes.Text = item.Type;
            lstItemType.Text = item.GetValue(ItemData.VAR_ITEM_TYPE, null);
        }

        public void SaveItem(ItemData item) {
            item.Type = (string)lstTypes.Text;
            item.SetValue(ItemData.VAR_ITEM_TYPE, lstItemType.Text);
            //...
        }

        public void Clear() {
            isLoading = true;
            lstTypes.Text = null;
            lstItemType.Text = null;
            //...
        }

        //-----------------------------

        private void txt_TextChanged(object sender, TextChangedEventArgs e) {
            if (!isLoading) setHasChanges(true);
        }

        private void chk_Checked(object sender, RoutedEventArgs e) {
            if (!isLoading) setHasChanges(true);
        }

        private void lst_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (!isLoading) setHasChanges(true);
        }

        //-----------------------------

        private void setHasChanges(bool value) {
            if (value && !hasChanges) {
                if (OnChanged != null) OnChanged.Invoke();
                hasChanges = true;
            }
        }
    }
}