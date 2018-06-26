using L2NPCManager.IO.Documents;
using System.IO;

namespace L2NPCManager.Data.NpcPosition
{
    public class NpcPositionData : ItemBase
    {
        public const string TAG_START = "npc_begin";
        public const string TAG_END = "npc_end";

        public const string VAR_POS = "pos";
        public const string VAR_TOTAL = "total";
        public const string VAR_RESPAWN = "respawn";
        public const string VAR_DBNAME = "dbname";
        public const string VAR_DBSAVING = "dbsaving";

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