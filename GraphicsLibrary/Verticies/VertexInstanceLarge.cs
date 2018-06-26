using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraphicsLibrary.Verticies
{
    public struct VertexInstanceLarge : IVertexType
    {
        public Matrix Position;


        public VertexInstanceLarge(Matrix position) {
            this.Position = position;
        }

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0,  VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3));

        VertexDeclaration IVertexType.VertexDeclaration {
            get {return VertexInstanceLarge.VertexDeclaration;}
        }
    }
}