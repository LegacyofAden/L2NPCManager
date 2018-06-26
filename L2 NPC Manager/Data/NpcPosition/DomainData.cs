using L2NPCManager.IO.Documents;
using System.IO;
using Microsoft.Xna.Framework;

namespace L2NPCManager.Data.NpcPosition
{
    public class DomainData : ItemBase
    {
        public const string TAG_START = "domain_begin";
        public const string TAG_END = "domain_end";

        public const string VAR_DOMAIN_ID = "domain_id";
        public const string VAR_BOUNDS = "bounds";

        public Vector4[] Points;
        public Bounds2D Bounds;

        //=============================

        public override void ReadData(int index, string value) {
            if (index == 1) {Name = value; return;}
            if (index == 3) {
                AddValue(new DocumentValue(VAR_BOUNDS, value));
                return;
            }
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