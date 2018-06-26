using System;
using System.Linq;
using System.Text;

namespace L2NPCManager.IO
{
    public static class ClanHelper
    {
        private static char[] SEP = {';'};


        public static string Write(string[] items) {
            StringBuilder data = new StringBuilder();
            data.Append('{');
            string[] data_items = items.ToArray();
            int c = data_items.Length;
            for (int i = 0; i < c; i++) {
                if (i > 0) data.Append(';');
                data.Append('@');
                data.Append(data_items[i].Replace(' ', '_'));
            }
            data.Append('}');
            return data.ToString();
        }

        public static string[] Parse(string data) {
            if (string.IsNullOrEmpty(data)) return null;
            string[] data_items = StringUtils.Trim(data, "{", "}").Split(SEP, StringSplitOptions.RemoveEmptyEntries);
            int c = data_items.Length;
            for (int i = 0; i < c; i++) {
                data_items[i] = data_items[i].TrimStart('@').Replace('_', ' ');
            }
            return data_items;
        }
    }
}