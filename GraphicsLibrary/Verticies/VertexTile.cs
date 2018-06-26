using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraphicsLibrary.Primitives
{
    public struct VertexTile : IVertexType
    {
        public Vector3 Position;
        public Vector2 Texture;
        public Vector3 Normal;


        public VertexTile(Vector3 position, Vector2 texture, Vector3 normal) {
            Position = position;
            Texture = texture;
            Normal = normal;
        }

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement( 0, VertexElementFormat.Vector3, VertexElementUsage.Position,          0),
            new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(20, VertexElementFormat.Vector3, VertexElementUsage.Normal,            0)
        );

        VertexDeclaration IVertexType.VertexDeclaration {
            get {return VertexTile.VertexDeclaration;}
        }
    }
}