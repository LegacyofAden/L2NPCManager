using L2NPCManager.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace L2NPCManager.Data.Npc
{
    public class NpcDropGroup
    {
        private const decimal min_fraction = 0.00001m;

        public ObservableCollection<NpcDropItem> Items;
        public string Chance {get; set;}


        public NpcDropGroup() {
            Items = new ObservableCollection<NpcDropItem>();
        }

        public NpcDropGroup Clone() {
            NpcDropGroup item = new NpcDropGroup();
            item.Chance = this.Chance;
            //
            int c = this.Items.Count;
            for (int i = 0; i < c; i++) {
                item.Items.Add(this.Items[i].Clone());
            }
            //
            return item;
        }

        //=============================
        // Read Items

        public static List<NpcDropGroup> ParseList(string data) {
            string inner_data = StringUtils.Trim(data, "{", "}");
            List<NpcDropGroup> items = new List<NpcDropGroup>();
            if (string.IsNullOrEmpty(inner_data)) return items;
            //
            NpcDropGroup group;
            int pos = 0;
            int len = inner_data.Length;
            while (pos < len && inner_data[pos] == '{') {
                group = read_group(inner_data, pos+1, out pos);
                items.Add(group);
                if (pos < len && inner_data[pos] == ';') pos++;
            }
            //
            return items;
        }

        private static NpcDropGroup read_group(string data, int p_start, out int p_end) {
            int pos = p_start+1;
            NpcDropItem item;
            NpcDropGroup group = new NpcDropGroup();
            int len = data.Length;
            while (pos < len && data[pos] == '{') {
                item = read_item(data, pos+1, out pos);
                group.Items.Add(item);
                if (pos < len && data[pos] == ';') pos++;
            }
            //
            if (pos < len && data[pos] == '}') pos++;
            if (pos < len && data[pos] == ';') pos++;
            //
            int x = data.IndexOf('}', pos);
            if (x < 0) throw new Exception("!");
            group.Chance = data.Substring(pos, x - pos);
            p_end = x+2;
            //
            return group;
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

        public static string WriteList(List<NpcDropGroup> items) {
            StringBuilder data = new StringBuilder();
            //
            data.Append('{');
            NpcDropGroup group;
            int gc = items.Count;
            for (int i = 0; i < gc; i++) {
                group = items[i];
                if (group.Items.Count == 0) continue;
                if (i > 0) data.Append(';');
                write_group(data, group);
            }
            data.Append('}');
            //
            return data.ToString();
        }

        private static void write_group(StringBuilder data, NpcDropGroup group) {
            data.Append('{');
            data.Append('{');
            //
            int c = group.Items.Count;
            if (c > 0) {
                NpcDropItem item;
                for (int i = 0; i < c; i++) {
                    item = group.Items[i];
                    if (i > 0) data.Append(';');
                    write_item(data, item);
                }
            }
            //
            data.Append('}');
            data.Append(';');
            data.Append(group.Chance);
            data.Append('}');
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
            return (remainder >= min_fraction);
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

        public bool AdjustRates(float adena_factor, bool adjust_min, bool adjust_max, ref int count) {
            if (adena_factor == 1f) return false;
            //
            bool has_item_changes;
            bool has_changes = false;
            NpcDropItem item;
            int c = Items.Count;
            for (int i = 0; i < c; i++) {
                item = Items[i];
                if (!item.DisplayName.Equals("adena", StringComparison.CurrentCultureIgnoreCase)) continue;
                //
                has_item_changes = false;
                if (adjust_min) {
                    item.Min = (int)(item.Min * adena_factor);
                    has_item_changes = true;
                }
                if (adjust_max) {
                    item.Max = (int)(item.Max * adena_factor);
                    has_item_changes = true;
                }
                if (has_item_changes) {
                    has_changes = true;
                    count++;
                }
            }
            return has_changes;
        }
    }
}