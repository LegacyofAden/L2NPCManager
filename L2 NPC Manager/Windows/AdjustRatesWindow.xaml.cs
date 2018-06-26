using L2NPCManager.Data;
using L2NPCManager.Data.Npc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;

namespace L2NPCManager.Windows
{
    public partial class AdjustRatesWindow : Window
    {
        private RatesTask task;

        public NpcManager NpcMgr {get; set;}
        public bool HasChanges {get; private set;}


        public AdjustRatesWindow() {
            InitializeComponent();
            //
            btnClose.Visibility = Visibility.Collapsed;
        }

        //=============================

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            if (task != null && task.IsBusy) {
                task.CancelAsync();
            } else {
                Close();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e) {
            float exp_f, sp_f, adena_f;
            try {exp_f = float.Parse(txtExp.Text);}
            catch (Exception) {
                MessageBox.Show("Invalid 'Exp' Factor!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                txtExp.SelectAll();
                return;
            }
            try {sp_f = float.Parse(txtSp.Text);}
            catch (Exception) {
                MessageBox.Show("Invalid 'Sp' Factor!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                txtSp.SelectAll();
                return;
            }
            try {adena_f = float.Parse(txtAdena.Text);}
            catch (Exception) {
                MessageBox.Show("Invalid 'Adena' Factor!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                txtAdena.SelectAll();
                return;
            }
            //
            btnStart.IsEnabled = false;
            txtExp.IsEnabled = false;
            txtSp.IsEnabled = false;
            txtAdena.IsEnabled = false;
            HasChanges = true;
            //
            task = new RatesTask(NpcMgr, exp_f, sp_f, adena_f);
            task.ProgressChanged += task_ProgressChanged;
            task.RunWorkerCompleted += task_RunWorkerCompleted;
            task.RunWorkerAsync();
        }

        private void task_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            progress.Value = e.ProgressPercentage;
        }

        private void task_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            btnCancel.IsEnabled = false;
            btnStart.Visibility = Visibility.Collapsed;
            btnClose.Visibility = Visibility.Visible;
            //
            if (!e.Cancelled) {
                if (e.Error != null) {
                    MessageBox.Show("An error occured while adjusting rates! "+e.Error.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                } else {
                    string text = "Updated "+task.getExpCount().ToString("#,##0")+" 'Exp' Values.\r\n";
                    text += "Updated "+task.getSpCount().ToString("#,##0")+" 'Sp' Values.\r\n";
                    text += "Updated "+task.getAdenaCount().ToString("#,##0")+" 'Adena' Values.\r\n";
                    MessageBox.Show(text, "Operation Successful!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            } else {
                Close();
            }
        }

        //=============================

        class RatesTask : BackgroundWorker {
            private NpcManager npc_mgr;
            public int Total {get; private set;}
            private float exp_f, sp_f, adena_f;
            private int exp_count;
            private int sp_count;
            private int adena_count;

            public RatesTask(NpcManager npc_mgr, float exp, float sp, float adena) {
                this.npc_mgr = npc_mgr;
                this.exp_f = exp;
                this.sp_f = sp;
                this.adena_f = adena;
                WorkerReportsProgress = true;
                WorkerSupportsCancellation = true;
            }

            protected override void OnDoWork(DoWorkEventArgs e) {
                exp_count = sp_count = adena_count = 0;
                //
                int c = npc_mgr.Items.Count;
                for (int i = 0; i < c; i++) {
                    run_npc(npc_mgr.Items[i]);
                    //
                    float p = i / (float)c;
                    ReportProgress((int)(i * 100f));
                }
            }

            private void run_npc(NpcData data) {
                if (data.AdjustRates(exp_f, sp_f, ref exp_count, ref sp_count)) {
                    npc_mgr.HasChanges = true;
                }
                //
                if (adena_f == 1f) return;
                //
                bool has_changes = false;
                List<NpcDropGroup> items = data.GetDropGroups();
                if (items == null) return;
                //
                int c = items.Count;
                for (int i = 0; i < c; i++) {
                    if (items[i].AdjustRates(adena_f, true, true, ref adena_count)) {
                        has_changes = true;
                    }
                }
                //
                if (has_changes) {
                    data.SetDropGroups(items);
                    npc_mgr.HasChanges = true;
                }
            }

            public int getExpCount() {return exp_count;}
            public int getSpCount() {return sp_count;}
            public int getAdenaCount() {return adena_count;}
        }
    }
}