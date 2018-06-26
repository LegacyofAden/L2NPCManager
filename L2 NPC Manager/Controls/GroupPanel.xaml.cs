using L2NPCManager.Data.Npc;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace L2NPCManager.Controls
{
    public partial class GroupPanel : UserControl
    {
        public delegate void DeleteEvent(GroupPanel sender, NpcDropGroup data);
        public delegate void ChangedEvent(GroupPanel sender);
        public event DeleteEvent OnDelete;
        public event ChangedEvent OnChanged;

        private NpcDropGroup data;
        private decimal remainder;

        public bool HasChanges {get; private set;}


        public GroupPanel() {
            InitializeComponent();
        }

        //=============================

        private void lstItems_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e) {
            if (e.EditAction == DataGridEditAction.Commit) {
                HasChanges = true;
                invokeChanged();
                updateRemainder();
            }
        }

        private void txtChance_TextChanged(object sender, TextChangedEventArgs e) {
            HasChanges = true;
            invokeChanged();
        }

        private void btnAddItem_Click(object sender, RoutedEventArgs e) {
            data.Items.Add(new NpcDropItem() {Min = 1, Max = 1});
            HasChanges = true;
            invokeChanged();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e) {
            invokeDelete();
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            updateRemainder();
            HasChanges = true;
            invokeChanged();
        }

        private void btnAdjust_Click(object sender, RoutedEventArgs e) {
            decimal sum = 0m;
            if (data.GetSum(ref sum)) {
                data.BalanceRemainders(ref sum);
                HasChanges = true;
                invokeChanged();
                //
                remainder = 0;
                updateRemainderValue();
            }
        }

        private void btnApplyRemainder_Click(object sender, RoutedEventArgs e) {
            if (remainder == 0) return;
            //
            Button btn = (Button)e.Source;
            NpcDropItem item = (NpcDropItem)btn.DataContext;
            item.Chance += remainder;
            remainder = 0;
            updateRemainderValue();
        }

        //-----------------------------

        public void SetIndex(int index) {
            lblName.Content = "Group "+(index+1);
        }

        public void Load(NpcDropGroup data) {
            this.data = data;
            HasChanges = false;
            //
            txtChance.Text = data.Chance;
            lstItems.ItemsSource = data.Items;
            data.Items.CollectionChanged += Items_CollectionChanged;
            //
            updateRemainder();
        }

        public NpcDropGroup Write() {
            data.Chance = txtChance.Text;
            HasChanges = false;
            return data;
        }

        private void updateRemainder() {
            decimal sum = 0;
            int c = data.Items.Count;
            for (int i = 0; i < c; i++) {
                sum += data.Items[i].Chance;
            }
            //
            remainder = 100m - sum;
            updateRemainderValue();
        }

        //-----------------------------

        private void updateRemainderValue() {
            txtRemainder.Text = remainder.ToString("0.#####");
            if (remainder < -0.00001m) txtRemainder.Style = (Style)Resources["txt_invalid"];
            else if (remainder > 0.00001m) txtRemainder.Style = (Style)Resources["txt_extra"];
            else txtRemainder.Style = null; // (Style)Resources["txt_valid"];
        }

        private void invokeDelete() {
            if (OnDelete != null) OnDelete.Invoke(this, data);
        }

        private void invokeChanged() {
            if (OnChanged != null) OnChanged.Invoke(this);
        }
    }
}