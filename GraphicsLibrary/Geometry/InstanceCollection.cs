using GraphicsLibrary.Primitives;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace GraphicsLibrary.Geometry
{
    public class InstanceCollection<VertexType> where VertexType : struct
    {
        public delegate VertexType UpdateEvent<ModelType>(ModelType model);

        private GeometricPrimitive mesh;
        private VertexDeclaration decleration;
        private DynamicVertexBuffer instanceBuffer;
        private Effect effect;
        private VertexType[] positions;
        private VertexBufferBinding[] bindings;
        private int count, size;
        private int vCount, iCount;


        public InstanceCollection(VertexDeclaration decleration) {
            this.decleration = decleration;
        }

        //=============================

        public void Load(GeometricPrimitive mesh, Effect effect) {
            this.mesh = mesh;
            this.effect = effect;
            //
            vCount = mesh.VertexCount;
            iCount = mesh.IndexCount / 3;
            //
            bindings = new VertexBufferBinding[2];
            bindings[0] = new VertexBufferBinding(mesh.VertexBuffer, 0, 0);
        }

        public void Update<ModelType>(GraphicsDevice graphics, IEnumerable<ModelType> source, UpdateEvent<ModelType> ev) where ModelType : Model {
            count = source.Count();
            if (count == 0) return;
            //
            if (count > size || instanceBuffer == null) {
                if (instanceBuffer != null) instanceBuffer.Dispose();
                instanceBuffer = new DynamicVertexBuffer(graphics, decleration, count, BufferUsage.WriteOnly);
                bindings[1] = new VertexBufferBinding(instanceBuffer, 0, 1);
            }
            //
            if (count > size) {
                positions = new VertexType[count];
                size = count;
            }
            //
            for (int i = 0; i < count; i++) {
                positions[i] = ev(source.ElementAt(i));
            }
            //
            instanceBuffer.SetData(positions, 0, count);
        }

        public void Render(GraphicsDevice graphics) {
            if (count == 0) return;
            //
            graphics.SetVertexBuffers(bindings);
            graphics.Indices = mesh.IndexBuffer;
            //
            effect.CurrentTechnique.Passes[0].Apply();
            //
            graphics.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, vCount, 0, iCount, count);
            //
            graphics.SetVertexBuffer(null);
        }
    }
}