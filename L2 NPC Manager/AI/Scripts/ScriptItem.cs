
namespace L2NPCManager.AI.Scripts
{
    public class ScriptItem
    {
        public string Name {get; set;}
        public string Source {get; set;}
        public string Data {get; set;}
        public bool IsDecompiled {get; set;}
        public bool IsModified {get; set;}
        public short Level {get; set;}

        //=============================

        public ClassDefinition GetClassDef() {
            string src = ((IsDecompiled || IsModified) ? Data : Source);
            int x = src.IndexOf("\r\n");
            if (x < 0) return null;
            string line = src.Substring(0, x);
            return ClassDefinition.FromString(src);
        }

        public string GetSuper() {
            ClassDefinition def = GetClassDef();
            return (def != null ? def.Super : null);
        }

        public string GetName() {
            ClassDefinition def = GetClassDef();
            return (def != null ? def.Name : null);
        }
    }
}