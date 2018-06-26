using L2NPCManager.IO;
using System;
using System.Collections.ObjectModel;
using System.Text;

namespace L2NPCManager.Data.Npc
{
    public class NpcSpoilGroup
    {
        private const decimal min_fraction = 0.00001m;

        public ObservableCollection<NpcDropItem> Items;


        public NpcSpoilGroup() {
            Items = new ObservableCollection<NpcDropItem>();
        }

        //=============================
        // Read Items

        public static NpcSpoilGroup ParseList(string data) {
            string inner_data = StringUtils.Trim(data, "{", "}");
            NpcSpoilGroup items = new NpcSpoilGroup();
            if (string.IsNullOrEmpty(inner_data)) return items;
            //
            NpcDropItem item;
            int pos = 0;
            int len = inner_data.Length;
            while (pos < len && inner_data[pos] == '{') {
                item = read_item(inner_data, pos+1, out pos);
                items.Items.Add(item);
                if (pos < len && inner_data[pos] == ';') pos++;
            }
            //
            return items;
        }

        private static NpcDropItem read_item(string data, int p_start, out int p_end) {
            int x = data.IndexOf('}', p_start);
            if (x < 0) throw new Exception("Invalid Drop Group data!");
            string inner_data = data.Substring(p_start, x - p_start + 1);
            p_end = x+1;
            //
            inner_data = StringUtils.Trim(inner_data, "{", "}");
            NpcDropItem item = new NpcDropItem();
            item.Load(inner_data);
            return item;
        }

        //-----------------------------
        // Write Items

        public static string WriteList(NpcSpoilGroup items) {
            StringBuilder data = new StringBuilder();
            //
            data.Append('{');
            int gc = items.Items.Count;
            for (int i = 0; i < gc; i++) {
                if (i > 0) data.Append(';');
                write_item(data, items.Items[i]);
            }
            data.Append('}');
            //
            return data.ToString();
        }

        private static void write_item(StringBuilder data, NpcDropItem item) {
            data.Append('{');
            item.Save(data);
            data.Append('}');
        }

        //-----------------------------
        // Balance Remainders

        public bool GetSum(ref decimal sum) {
            int c = Items.Count;
            if (c == 0) return false;
            if (c == 1) return (Items[0].Chance != 100m);
            //
            sum = 0m;
            for (int i = 0; i < c; i++) {
                sum += Items[i].Chance;
            }
            decimal remainder = 100m - sum;
            return (remainder < min_fraction);
        }

        public void BalanceRemainders(ref decimal sum) {
            NpcDropItem item;
            int c = Items.Count;
            if (c == 0) return;
            if (c == 1) {
                Items[0].Chance = 100m;
                return;
            }
            //
            if (sum < min_fraction) {
                for (int i = 0; i < c; i++) Items[i].Chance = 1m;
                sum = c;
            }
            //
            decimal f = 100m / sum;
            sum = 0m;
            for (int i = 0; i < c; i++) {
                item = Items[i];
                //
                item.Chance = decimal.Truncate(item.Chance * f * 100000m) * min_fraction;
                sum += item.Chance;
            }
            //
            balanceFractions(sum);
        }

        private void balanceFractions(decimal sum) {
            decimal remainder = 100m - sum;
            if (Math.Abs(remainder) < min_fraction) return;
            //
            NpcDropItem item;
            int c = Items.Count;
            int x = (int)(remainder / min_fraction);
            for (int i = 0; i < x; i++) {
                item = Items[i % c];
                item.Chance = item.Chance + min_fraction;
            }
        }
    }
}