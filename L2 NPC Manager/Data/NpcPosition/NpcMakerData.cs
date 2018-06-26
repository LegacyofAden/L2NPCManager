using L2NPCManager.IO.Documents;
using System.IO;

namespace L2NPCManager.Data.NpcPosition
{
    public class NpcMakerData : ItemBase
    {
        public const string TAG_START = "npcmaker_begin";
        public const string TAG_END = "npcmaker_end";

        public const string VAR_INITIAL_SPAWN = "initial_spawn";
        public const string VAR_SPAWN_TIME = "spawn_time";
        public const string VAR_MAXIMUM_NPC = "maximum_npc";

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