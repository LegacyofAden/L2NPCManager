using System.Text;
using System.Collections.Generic;

namespace L2NPCManager.IO
{
    public class StringUtils
    {
        public static string getString(string value) {
            if (value == null) return null;
            return Trim(value, "[", "]").Replace('_', ' ');
        }

        public static string setString(string value) {
            if (value == null) return "[]";
            return Pad(value.Replace(' ', '_'), "[", "]");
        }

        public static string Trim(string value, string start_char, string end_char) {
            bool x1 = value.StartsWith(start_char);
            bool x2 = value.EndsWith(end_char);
            int i1 = (x1 ? 1 : 0);
            int i2 = (x2 ? 1 : 0);
            return value.Substring(i1, value.Length - i2 - i1);
        }

        public static string Pad(string value, string start_char, string end_char) {
            StringBuilder data = new StringBuilder();
            if (!value.StartsWith(start_char)) data.Append(start_char);
            data.Append(value);
            if (!value.EndsWith(end_char)) data.Append(end_char);
            return data.ToString();
        }

        public static string[] Split(string source, char separator) {
            List<string> r = new List<string>();
            //
            char c;
            int pos = 0, depth = 0;
            int len = source.Length;
            for (int i = 0; i < len; i++) {
                c = source[i];
                if (c == '{') depth++;
                else if (c == '}') depth--;
                else if (c == separator && depth == 0) {
                    r.Add(source.Substring(pos, i - pos));
                    pos = i + 1;
                }
            }
            //
            if (pos < len) {
                r.Add(source.Substring(pos, len - pos));
            }
            //
            return r.ToArray();
        }
    }
}