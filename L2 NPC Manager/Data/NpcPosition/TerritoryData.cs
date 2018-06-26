using L2NPCManager.IO.Documents;
using System.IO;

namespace L2NPCManager.Data.NpcPosition
{
    public class TerritoryData : ItemBase
    {
        public const string TAG_START = "territory_begin";
        public const string TAG_END = "territory_end";

        //=============================

        public override void ReadData(int index, string value) {
            if (index == 1) {Name = value; return;}
            AddValue(new DocumentValue(value));
        }

        public override void WriteData(StreamWriter writer) {
            writer.Write(TAG_START);
            writer.Write('\t');
            writer.Write(Name);
            WriteValues(writer);
            writer.Write('\t');
            writer.Write(TAG_END);
        }
    }
}