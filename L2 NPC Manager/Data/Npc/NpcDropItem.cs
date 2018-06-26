using L2NPCManager.IO;
using System.ComponentModel;
using System.Text;

namespace L2NPCManager.Data.Npc
{
    public class NpcDropItem : INotifyPropertyChanged
    {
        private const string ITEM_NAME_DEFAULT = "???";

        public event PropertyChangedEventHandler PropertyChanged;

        private string name, min, max, chance;
        private string display_name;
        private int min_val, max_val;
        private decimal chance_val;

        public string DisplayName {
            get {return display_name;}
            set {display_name = value; invokePropertyChanged("Name");}
        }
        public int Min {
            get {return min_val;}
            set {min_val = value; invokePropertyChanged("Min");}
        }
        public int Max {
            get {return max_val;}
            set {max_val = value; invokePropertyChanged("Max");}
        }
        public decimal Chance {
            get {return chance_val;}
            set {chance_val = value; invokePropertyChanged("Chance");}
        }

        //=============================

        public NpcDropItem Clone() {
            NpcDropItem item = new NpcDropItem();
            item.name = this.name;
            item.min = this.min;
            item.max = this.max;
            item.chance = this.chance;
            item.min_val = this.min_val;
            item.max_val = this.max_val;
            item.chance_val = this.chance_val;
            item.UpdateDisplayValues();
            return item;
        }

        public void Load(string data) {
            string inner_data = StringUtils.Trim(data, "{", "}");
            string[] tags = inner_data.Split(';');
            int c = tags.Length;
            //
            if (c > 0) name = tags[0];
            if (c > 1) min = tags[1];
            if (c > 2) max = tags[2];
            if (c > 3) chance = tags[3];
            //
            display_name = StringUtils.getString(name);
            try {min_val = int.Parse(min);} catch {}
            try {max_val = int.Parse(max);} catch {}
            try {chance_val = decimal.Parse(chance);} catch {}
        }

        public void Save(StringBuilder data) {
            string n = (display_name != null ? display_name : string.Empty);
            name = StringUtils.setString(n);
            min = min_val.ToString();
            max = max_val.ToString();
            chance = chance_val.ToString();
            //
            data.Append(name);
            data.Append(';');
            data.Append(min);
            data.Append(';');
            data.Append(max);
            data.Append(';');
            data.Append(chance);
        }

        //-----------------------------

        public void UpdateDisplayValues() {
            if (!string.IsNullOrEmpty(name)) {
                DisplayName = StringUtils.getString(name);
            } else {
                DisplayName = ITEM_NAME_DEFAULT;
            }
        }

        private void invokePropertyChanged(string name) {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}