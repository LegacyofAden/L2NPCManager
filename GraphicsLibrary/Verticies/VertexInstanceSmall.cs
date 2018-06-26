using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraphicsLibrary.Verticies
{
    public struct VertexInstanceSmall : IVertexType
    {
        public Matrix Position;
        public Vector4 Mask;


        public VertexInstanceSmall(Matrix position, Vector4 mask) {
            this.Position = position;
            this.Mask = mask;
        }

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0,  VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3),
            new VertexElement(64, VertexElementFormat.Vector4, VertexElementUsage.Color,             0));

        VertexDeclaration IVertexType.VertexDeclaration {
            get {return VertexInstanceSmall.VertexDeclaration;}
        }
    }
}