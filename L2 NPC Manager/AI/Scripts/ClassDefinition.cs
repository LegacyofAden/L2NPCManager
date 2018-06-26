using System.Text;
using System.Text.RegularExpressions;

namespace L2NPCManager.AI.Scripts
{
    public class ClassDefinition
    {
        private const string PATTERN = @"class\s+(\d)\s+([^\s\{]+)\s*\:?\s*([^\s\{]+)?\s*";

        public string Type {get; set;}
        public string Name {get; set;}
        public string Super {get; set;}

        //=============================

        public static ClassDefinition FromString(string source) {
            Match match = Regex.Match(source, PATTERN, RegexOptions.IgnoreCase);
            if (!match.Success) return null;
            return new ClassDefinition() {
                Type = match.Groups[1].Value,
                Name = match.Groups[2].Value,
                Super = match.Groups[3].Value,
            };
        }

        public override string ToString() {
            StringBuilder data = new StringBuilder();
            data.Append("class");
            if (!string.IsNullOrEmpty(Type)) {
                data.Append(' ');
                data.Append(Type);
            }
            if (!string.IsNullOrEmpty(Name)) {
                data.Append(' ');
                data.Append(Name);
            }
            if (!string.IsNullOrEmpty(Super)) {
                data.Append(" : ");
                data.Append(Super);
            }
            return data.ToString();
        }
    }
}