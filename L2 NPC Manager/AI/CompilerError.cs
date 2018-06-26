
namespace L2NPCManager.AI
{
    public class CompilerError
    {
        public string Message {get; set;}
        public int Line {get; set;}

        //=============================

        public static CompilerError FromString(string value) {
            CompilerError r = new CompilerError();
            r.Message = value;
            r.Line = -1;
            //
            int i = value.IndexOf('(');
            if (i >= 0) {
                int x = value.IndexOf(')', i+1);
                if (x >= 0) {
                    string val = value.Substring(i+1, x - i - 1);
                    try {r.Line = int.Parse(val);}
                    catch {}
                }
            }
            //
            return r;
        }

        public override string ToString() {return Message;}
    }
}