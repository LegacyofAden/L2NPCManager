using L2NPCManager.AI.Scripts;
using L2NPCManager.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace L2NPCManager.Windows
{
    public partial class NewScriptWindow : Window
    {
        public ScriptManager ScriptMgr {get; set;}
        public ScriptItem NewScript {get; private set;}

        private List<string> src_items;


        public NewScriptWindow() {
            InitializeComponent();
        }

        //=============================

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            txtName.Focus();
            //
            src_items = new List<string>();
            src_items.Add("None");
            if (ScriptMgr != null) {
                foreach (ScriptItem i in ScriptMgr.Items) {
                    src_items.Add(i.Name);
                }
            }
            lstParentClass.ItemsSource = src_items;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e) {
            if (ScriptMgr == null) throw new ArgumentNullException("ScriptMgr", "A script manager has not been assigned!");
            //
            string new_name = txtName.Text;
            if (!isNameValid(new_name)) {
                txtName.SelectAll();
                //txtName.Focus();
                return;
            }
            //
            // check if script name exists
            if (ScriptMgr.Contains(new_name)) {
                showError("Script name already exists!");
                txtName.SelectAll();
                //txtName.Focus();
                return;
            }
            //
            string super = lstParentClass.Text;
            if (super == "None") super = null;
            else {
                if (!ScriptMgr.Contains(super)) {
                    showError("Parent class not found!");
                    lstParentClass.Focus();
                    return;
                }
            }
            //
            ScriptItem script = new ScriptItem();
            script.Name = txtName.Text;
            script.Data = buildTemplate(new_name, super);
            script.IsDecompiled = true;
            script.IsModified = true;
            //
            ScriptMgr.Set(script);
            ScriptMgr.HasChanges = true;
            NewScript = script;
            DialogResult = true;
            //Close();
        }

        //private void lstParentClass_Populating(object sender, PopulatingEventArgs e) {
        //    string text = lstParentClass.Text;
        //    //
        //    IEnumerable<string> items = src_items.Where(i => i.Contains(text));
        //    lstParentClass.ItemsSource = items;
        //}

        //-----------------------------

        private bool isNameValid(string name) {
            int c = name.Length;
            for (int i = 0; i < c; i++) {
                if (i == 0) {
                    if (!char.IsLetter(name, 0)) {
                        showError("Script name must begin with a letter or underscore!");
                        return false;
                    }
                } else {
                    if (!char.IsLetterOrDigit(name, i) && !name[i].Equals('_')) {
                        showError("Script name may only contain letters, digits, and underscores!");
                        return false;
                    }
                }
            }
            //
            return true;
        }

        private string buildTemplate(string new_name, string super) {
            ClassDefinition def = new ClassDefinition();
            def.Type = "1";
            def.Name = new_name;
            def.Super = super;
            //
            StringBuilder data = new StringBuilder();
            data.AppendLine(def.ToString());
            data.AppendLine("{");
            data.AppendLine("  parameter:");
            data.AppendLine("    // Parameters here...");
            data.AppendLine();
            data.AppendLine("  property:");
            data.AppendLine("    // Properties here...");
            data.AppendLine();
            data.AppendLine("  handler:");
            data.AppendLine("    // Handlers here..");
            data.AppendLine();
            data.Append("}");
            return data.ToString();
        }

        private void showError(string message) {
            MessageBox.Show(message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}