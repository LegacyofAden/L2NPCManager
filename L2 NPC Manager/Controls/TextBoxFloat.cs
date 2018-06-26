using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace L2NPCManager.Controls
{
    public class TextBoxFloat : TextBox
    {
        private Style base_style, invalid_style;

        [Browsable(true)]
        [Category("Common")]
        public int? Min {get; set;}

        [Browsable(true)]
        [Category("Common")]
        public int? Max {get; set;}

        [Browsable(true)]
        [Category("Common")]
        [DefaultValue(true)]
        public bool AllowEmpty {get; set;}


        public TextBoxFloat() {
            AcceptsTab = false;
            AcceptsReturn = false;
            TextWrapping = TextWrapping.NoWrap;
            //
            Initialized += Control_Initialized;
            PreviewTextInput += Control_PreviewTextInput;
            PreviewKeyDown += Control_PreviewKeyDown;
            TextChanged += Control_TextChanged;
        }

        //=============================

        private void Control_Initialized(object sender, EventArgs e) {
            base_style = Style;
            invalid_style = new Style(typeof(TextBox), base_style);
            invalid_style.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(Color.FromRgb(255, 200, 200))));
            invalid_style.Setters.Add(new Setter(Control.BorderBrushProperty, new SolidColorBrush(Color.FromRgb(255, 0, 0))));
        }

        private void Control_PreviewKeyDown(object sender, KeyEventArgs e) {
            bool allow_negative = (!Min.HasValue || Min.Value >= 0);
            if (e.Key == Key.Space || (!allow_negative && e.Key == Key.OemMinus)) e.Handled = true;
        }

        private void Control_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            foreach (char c in e.Text) {
                if (!(char.IsDigit(c) || c == '.')) {
                    e.Handled = true;
                    return;
                }
            }
        }

        private void Control_TextChanged(object sender, TextChangedEventArgs e) {
            checkValue();
        }

        //-----------------------------

        public new void Clear() {
            base.Clear();
            base.Style = base_style;
        }

        private void checkValue() {
            if (string.IsNullOrEmpty(Text)) {
                Style = (AllowEmpty ? base_style : invalid_style);
                return;
            }
            //
            float value;
            try {value = float.Parse(Text);}
            catch (Exception) {
                Style = invalid_style;
                return;
            }
            //
            Style = base_style;
            if (Min.HasValue && value < Min.Value) {
                Text = Min.Value.ToString();
            }
            if (Max.HasValue && value > Max.Value) {
                Text = Max.Value.ToString();
            }
        }
    }
}