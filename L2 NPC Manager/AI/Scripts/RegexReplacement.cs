using System.Text.RegularExpressions;

namespace L2NPCManager.AI.Scripts
{
    public class RegexReplacement
    {
        public Regex Expression {get; private set;}
        public string Output {get; private set;}


        public RegexReplacement(string expression, string output) {
            this.Expression = new Regex(expression, RegexOptions.Multiline);
            this.Output = output;
        }
    }
}