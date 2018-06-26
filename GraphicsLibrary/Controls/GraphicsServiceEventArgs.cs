using Microsoft.Xna.Framework.Graphics;
using System;

namespace GraphicsLibrary.Controls
{
    public class GraphicsServiceEventArgs : EventArgs
    {
        public IGraphicsDeviceService Service {get; private set;}


        public GraphicsServiceEventArgs(IGraphicsDeviceService service) {
            this.Service = service;
        }
    }
}