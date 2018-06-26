using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GraphicsLibrary.Primitives
{
    public abstract class GeometricPrimitive : IDisposable
    {
        public VertexBuffer VertexBuffer {get; private set;}
        public IndexBuffer IndexBuffer {get; private set;}
        public int VertexCount {get; private set;}
        public int IndexCount {get; private set;}

        private List<VertexTile> vertices;
        private List<ushort> indices;


        public GeometricPrimitive() {
            vertices = new List<VertexTile>();
            indices = new List<ushort>();
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        //=============================

        protected void AddVertex(Vector3 position, Vector2 texture, Vector3 normal) {
            vertices.Add(new VertexTile(position, texture, normal));
        }

        protected void AddIndex(int index) {
            if (index > ushort.MaxValue) throw new ArgumentOutOfRangeException("index");
            indices.Add((ushort)index);
        }

        protected int CurrentVertex {
            get {return vertices.Count;}
        }

        protected void InitializePrimitive(GraphicsDevice graphicsDevice) {
            VertexCount = vertices.Count;
            VertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexTile), VertexCount, BufferUsage.None);
            VertexBuffer.SetData(vertices.ToArray());
            //
            IndexCount = indices.Count;
            IndexBuffer = new IndexBuffer(graphicsDevice, typeof(ushort), IndexCount, BufferUsage.None);
            IndexBuffer.SetData(indices.ToArray());
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                if (VertexBuffer != null) VertexBuffer.Dispose();
                if (IndexBuffer != null) IndexBuffer.Dispose();
            }
        }
    }
}