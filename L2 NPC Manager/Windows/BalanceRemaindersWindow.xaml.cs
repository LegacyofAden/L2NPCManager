using L2NPCManager.Data;
using L2NPCManager.Data.Npc;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;

namespace L2NPCManager.Windows
{
    public partial class BalanceRemaindersWindow : Window
    {
        private BalanceTask balance_task;
        private PreviewWorker preview_task;


        public NpcManager NpcMgr {get; set;}
        public bool HasChanges {get; private set;}


        public BalanceRemaindersWindow() {
            InitializeComponent();
            //
            btnClose.Visibility = Visibility.Collapsed;
        }

        //=============================

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            if (balance_task != null && balance_task.IsBusy) {
                balance_task.CancelAsync();
            } else {
                Close();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void btnPreview_Click(object sender, RoutedEventArgs e) {
            btnPreview.IsEnabled = false;
            btnStart.IsEnabled = false;
            //
            preview_task = new PreviewWorker();
            preview_task.NpcMgr = NpcMgr;
            preview_task.ProgressChanged += preview_task_ProgressChanged;
            preview_task.RunWorkerCompleted += preview_task_RunWorkerCompleted;
            preview_task.RunWorkerAsync();
        }

        private void preview_task_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            progress.Value = e.ProgressPercentage;
        }

        private void preview_task_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            btnPreview.Visibility = Visibility.Collapsed;
            btnStart.IsEnabled = true;
            progress.Value = 0;
            //
            if (!e.Cancelled) {
                if (e.Error != null) {
                    lblCount.Text = "Error...";
                    MessageBox.Show("Failed to create preview! "+e.Error.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                } else {
                    lstItems.ItemsSource = preview_task.PreviewItems;
                    string ratio = preview_task.getCount().ToString("#,##0")+" of "+preview_task.Total.ToString("#,##0");
                    lblCount.Text = ratio+" items available";
                }
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e) {
            btnStart.IsEnabled = false;
            btnPreview.IsEnabled = false;
            HasChanges = true;
            //
            balance_task = new BalanceTask();
            balance_task.NpcMgr = NpcMgr;
            balance_task.ProgressChanged += task_ProgressChanged;
            balance_task.RunWorkerCompleted += task_RunWorkerCompleted;
            balance_task.RunWorkerAsync();
        }

        private void task_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            progress.Value = e.ProgressPercentage;
        }

        private void task_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            btnCancel.IsEnabled = false;
            btnStart.Visibility = Visibility.Collapsed;
            btnPreview.Visibility = Visibility.Collapsed;
            btnClose.Visibility = Visibility.Visible;
            //
            if (!e.Cancelled) {
                if (e.Error != null) {
                    MessageBox.Show("An error occured while balancing remainders! "+e.Error.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                } else {
                    string ratio = balance_task.getCount().ToString("#,##0")+" of "+balance_task.Total.ToString("#,##0");
                    MessageBox.Show("Balanced "+ratio+" total groups.", "Operation Successful!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            } else {
                Close();
            }
        }

        //=============================

        class BalanceTask : BackgroundWorker {
            public NpcManager NpcMgr {get; set;}
            public int Total {get; private set;}

            private int count;

            public BalanceTask() {
                WorkerReportsProgress = true;
                WorkerSupportsCancellation = true;
            }

            protected override void OnDoWork(DoWorkEventArgs e) {
                count = 0;
                int c = NpcMgr.Items.Count;
                for (int i = 0; i < c; i++) {
                    run_npc(NpcMgr.Items[i]);
                    //
                    float p = i / (float)c;
                    ReportProgress((int)(i * 100f));
                }
            }

            private void run_npc(NpcData data) {
                List<NpcDropGroup> items = data.GetDropGroups();
                if (items == null) return;
                //
                NpcDropGroup item;
                decimal sum = 0m;
                bool has_changes = false;
                int c = items.Count;
                for (int i = 0; i < c; i++) {
                    item = items[i];
                    if (item.GetSum(ref sum)) {
                        item.BalanceRemainders(ref sum);
                        has_changes = true;
                        count++;
                    }
                    Total++;
                }
                //
                if (has_changes) {
                    data.SetDropGroups(items);
                    NpcMgr.HasChanges = true;
                }
            }

            public int getCount() {return count;}
        }

        //-----------------------------

        class PreviewWorker : BackgroundWorker {
            public NpcManager NpcMgr {get; set;}
            public int Total {get; private set;}

            public ObservableCollection<PreviewItem> PreviewItems;
            private int count;

            public PreviewWorker() {
                WorkerReportsProgress = true;
                WorkerSupportsCancellation = true;
            }

            protected override void OnDoWork(DoWorkEventArgs e) {
                PreviewItems = new ObservableCollection<PreviewItem>();
                //
                count = 0;
                int c = NpcMgr.Items.Count;
                for (int i = 0; i < c; i++) {
                    run_npc(NpcMgr.Items[i]);
                    //
                    float p = i / (float)c;
                    ReportProgress((int)(i * 100f));
                }
            }

            private void run_npc(NpcData data) {
                List<NpcDropGroup> items = data.GetDropGroups();
                if (items == null) return;
                //
                PreviewItem preview_item;
                NpcDropGroup group, src_group;
                decimal sum = 0m;
                int cc, c = items.Count;
                for (int i = 0; i < c; i++) {
                    src_group = items[i];
                    if (src_group.GetSum(ref sum)) {
                        group = src_group.Clone();
                        group.BalanceRemainders(ref sum);
                        //
                        NpcDropItem item, src_item;
                        cc = group.Items.Count;
                        for (int y = 0; y < cc; y++) {
                            item = group.Items[y];
                            src_item = src_group.Items[y];
                            preview_item = new PreviewItem(data, item);
                            preview_item.ChancePre = src_item.Chance.ToString("#,##0.#####");
                            preview_item.ChancePost = item.Chance.ToString("#,##0.#####");
                            if (y > 0) preview_item.NpcName = null;
                            PreviewItems.Add(preview_item);
                        }
                        count++;
                    }
                    Total++;
                }
            }

            public int getCount() {return count;}
        }

        class PreviewItem {
            public string NpcName {get; set;}
            public string ItemName {get; set;}
            public string ChancePre {get; set;}
            public string ChancePost {get; set;}
            public bool Enabled {get; set;}


            public PreviewItem(NpcData data, NpcDropItem item) {
                NpcName = data.DisplayName;
                ItemName = item.DisplayName;
                Enabled = true;
            }
        }
    }
}