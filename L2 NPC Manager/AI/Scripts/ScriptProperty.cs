using System.Text;
using System.Text.RegularExpressions;

namespace L2NPCManager.AI.Scripts
{
    public class ScriptProperty
    {
        public string ScriptName {get; set;}
        public string Params {get; set;}

        //=============================

        public static ScriptProperty FromString(string value) {
            string pattern = @"\{\[(\w+)\]\;?([\s\S]+)?\}";
            Match match = Regex.Match(value, pattern);
            if (!match.Success) return null;
            ScriptProperty r = new ScriptProperty();
            r.ScriptName = match.Groups[1].Value;
            r.Params = match.Groups[2].Value;
            return r;
        }

        public override string ToString() {
            StringBuilder data = new StringBuilder();
            data.Append("{[");
            data.Append(ScriptName);
            data.Append("]");
            //
            if (!string.IsNullOrEmpty(Params)) {
                data.Append(";");
                data.Append(Params);
            }
            //
            data.Append('}');
            return data.ToString();
        }
    }
}