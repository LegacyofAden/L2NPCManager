using Microsoft.Xna.Framework;

namespace L2NPCManager.Data
{
    public struct Bounds2D
    {
        public float MinX {get; set;}
        public float MaxX {get; set;}
        public float MinY {get; set;}
        public float MaxY {get; set;}

        //=============================

        public bool Intersects(BoundingBox box) {
            return !(box.Min.X > MaxX || box.Max.X < MinX || box.Min.Y > MaxY || box.Max.Y < MinY);
        }
    }
}