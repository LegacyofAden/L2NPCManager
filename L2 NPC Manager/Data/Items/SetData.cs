using L2NPCManager.IO.Documents;
using System.IO;

namespace L2NPCManager.Data.Items
{
    public class SetData : ItemBase
    {
        public const string TAG_START = "set_begin";
        public const string TAG_END = "set_end";

        public const string VAR_SLOT_CHEST = "slot_chest";
        public const string VAR_SLOT_LEGS = "slot_legs";
        public const string VAR_SLOT_GLOVES = "slot_gloves";

        //=============================

        public override void ReadData(int index, string value) {
            if (index == 1) {ID = value; return;}
            AddValue(new DocumentValue(value));
        }

        public override void WriteData(StreamWriter writer) {
            writer.Write(TAG_START);
            writer.Write('\t');
            writer.Write(ID);
            WriteValues(writer);
            writer.Write('\t');
            writer.Write(TAG_END);
        }
    }
}