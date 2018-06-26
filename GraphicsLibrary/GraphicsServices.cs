using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GraphicsLibrary
{
    public class GraphicsServices : IDisposable
    {
        public ContentManager Content {get; private set;}
        public GraphicsDevice Graphics {get; private set;}

        private GameServiceContainer container;


        public GraphicsServices() {
            container = new GameServiceContainer();
        }

        public void Dispose() {
            if (Content != null) Content.Dispose();
        }

        //=============================

        public void Initialize(IGraphicsDeviceService graphics) {
            Graphics = graphics.GraphicsDevice;
            Content = new ContentManager(container, "Content");
            //
            container.AddService(typeof(IGraphicsDeviceService), graphics);
            container.AddService(typeof(ContentManager), Content);
        }
    }
}