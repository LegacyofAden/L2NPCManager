using ICSharpCode.AvalonEdit.Document;
using L2NPCManager.AI;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace L2NPCManager.Windows
{
    public partial class CompilerOutputWindow : Window
    {
        public CompilerParser Result {get; set;}

        private string filename;


        public CompilerOutputWindow() {
            InitializeComponent();
            //
            filename = Path.Combine(Environment.CurrentDirectory, "AI\\Compiler\\ai.nasc");
        }

        //=============================

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            lstErrors.ItemsSource = Result.Errors;
            //
            txtScript.Load(filename);
        }

        private void lstErrors_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            CompilerError error = (CompilerError)lstErrors.SelectedItem;
            if (error.Line > 0) {
                DocumentLine line = txtScript.Document.GetLineByNumber(error.Line);
                txtScript.Select(line.Offset, line.TotalLength);
                txtScript.ScrollToLine(error.Line);
            } else {
                txtScript.SelectionLength = 0;
            }
        }
    }
}