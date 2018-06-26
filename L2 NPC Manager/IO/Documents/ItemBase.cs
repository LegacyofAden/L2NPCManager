using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace L2NPCManager.IO.Documents
{
    public abstract class ItemBase : INotifyPropertyChanged
    {
        private const string ITEM_NAME_DEFAULT = "???";

        public event PropertyChangedEventHandler PropertyChanged;

        public string Type, ID, Name;
        private string display_id, display_name;
        protected List<DocumentValue> values;

        public string DisplayID {
            get {return display_id;}
            set {display_id = value; invokePropertyChanged("DisplayID");}
        }

        public string DisplayName {
            get {return display_name;}
            set {display_name = value; invokePropertyChanged("DisplayName");}
        }


        public ItemBase() {
            values = new List<DocumentValue>();
        }

        //=============================

        public abstract void ReadData(int index, string value);
        public abstract void WriteData(StreamWriter writer);

        public void Clone(ItemBase new_item) {
            new_item.ID = ID;
            new_item.Name = Name;
            new_item.Type = Type;
            //
            int c = values.Count;
            for (int i = 0; i < c; i++) {
                new_item.values.Add(values[i].Clone());
            }
        }

        public void AddValue(DocumentValue item) {
            values.Add(item);
        }

        public void Read(List<DocumentValue> values) {
            this.values = values;
            UpdateDisplayValues();
        }

        public void WriteValues(StreamWriter writer) {
            string tag;
            int c = values.Count;
            for (int i = 0; i < c; i++) {
                tag = values[i].ToString();
                writer.Write('\t');
                writer.Write(tag);
            }
        }

        public void UpdateDisplayValues() {
            DisplayID = ID;
            //
            if (Name != null) {
                DisplayName = StringUtils.getString(Name);
            } else {
                DisplayName = ITEM_NAME_DEFAULT;
            }
        }

        //-----------------------------

        public string GetValue(string key, string default_value) {
            DocumentValue item = findValue(key);
            return (item != null ? item.Value : default_value);
        }
        public bool GetValue(string key, bool default_value) {
            DocumentValue item = findValue(key);
            return (item != null && item.Value.Equals("1"));
        }
        public string GetValue(int index, string default_value) {
            DocumentValue item = findValue(index);
            return (item != null ? item.Value : default_value);
        }

        public void SetValue(string key, bool? value) {
            SetValue(key, value.HasValue && value.Value ? "1" : "0");
        }
        public void SetValue(string key, string value) {
            DocumentValue item = findValue(key);
            if (item == null) {
                item = new DocumentValue();
                item.Key = key;
                item.Value = value;
                values.Add(item);
            } else {
                item.Value = value;
            }
        }

        protected DocumentValue findValue(int index) {
            return (index < values.Count ? values[index] : null);
        }
        protected DocumentValue findValue(string key) {
            DocumentValue item;
            int c = values.Count;
            for (int i = 0; i < c; i++) {
                item = values[i];
                if (item.Key.Equals(key)) return item;
            }
            return null;
        }

        //-----------------------------

        protected void invokePropertyChanged(string name) {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}