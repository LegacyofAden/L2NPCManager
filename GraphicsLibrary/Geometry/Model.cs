using GraphicsLibrary.Primitives;
using Microsoft.Xna.Framework;

namespace GraphicsLibrary.Geometry
{
    public class Model
    {
        public GeometricPrimitive Mesh {get; set;}

        public Vector3 Position;
        public BoundingBox Bounds;
        public Matrix MatWorld;


        public Model() {
            Bounds = new BoundingBox();
        }

        //=============================

        public void SetBounds(float size_x, float size_y, float size_z) {
            Bounds.Min = Position;
            Bounds.Max.X = Position.X + size_x;
            Bounds.Max.Y = Position.Y + size_y;
            Bounds.Max.Z = Position.Z + size_z;
        }

        public void UpdateWorld() {
            Matrix.CreateTranslation(ref Position, out MatWorld);
        }
    }
}