using L2NPCManager.Data.Npc;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace L2NPCManager.Controls
{
    public partial class SpoilPanel : UserControl
    {
        public delegate void ChangedEvent(SpoilPanel sender);
        public event ChangedEvent OnChanged;

        private NpcSpoilGroup data;
        private decimal remainder;

        public bool HasChanges {get; private set;}


        public SpoilPanel() {
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

        private void btnAddItem_Click(object sender, RoutedEventArgs e) {
            data.Items.Add(new NpcDropItem());
            HasChanges = true;
            invokeChanged();
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            updateRemainder();
            HasChanges = true;
            invokeChanged();
        }

        private void btnAdjust_Click(object sender, RoutedEventArgs e) {
            MessageBoxResult r = MessageBox.Show("Balancing an NPC's Spoil group will cause players to have a 100% spoil chance to always receive some item in this group.\r\n\r\nAre you sure?", "Warning!", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (r == MessageBoxResult.Cancel) return;
            //
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

        public void Clear() {
            data = null;
            HasChanges = false;
            lstItems.ItemsSource = null;
            txtRemainder.Text = null;
        }

        public void Load(NpcSpoilGroup data) {
            this.data = data;
            HasChanges = false;
            //
            lstItems.ItemsSource = data.Items;
            data.Items.CollectionChanged += Items_CollectionChanged;
            //
            updateRemainder();
        }

        public NpcSpoilGroup Write() {
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

        private void adjustChances() {
            decimal sum = 0;
            NpcDropItem item;
            int c = data.Items.Count;
            for (int i = 0; i < c; i++) {
                sum += data.Items[i].Chance;
            }
            //
            remainder = 100 - sum;
            if (sum <= 0) return;
            //
            decimal f = 100m / sum;
            sum = 0;
            for (int i = 0; i < c; i++) {
                item = data.Items[i];
                //
                item.Chance = decimal.Truncate(item.Chance * f * 10000m) * 0.0001m;
                sum += item.Chance;
            }
            //
            HasChanges = true;
            invokeChanged();
            //
            remainder = 100m - sum;
            updateRemainderValue();
        }

        private void updateRemainderValue() {
            txtRemainder.Text = remainder.ToString("0.#####");
            if (remainder < 0.00001m) txtRemainder.Style = (Style)Resources["rem_style_invalid"];
            else txtRemainder.Style = (Style)Resources["rem_style_valid"];
        }

        private void invokeChanged() {
            if (OnChanged != null) OnChanged.Invoke(this);
        }
    }
}