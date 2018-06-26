using Microsoft.Xna.Framework.Graphics;
using System;

namespace GraphicsLibrary
{
    public class SmallBlockEffect : IDisposable
    {
        public Effect FX {get {return fx;}}

        private Effect fx;
        private EffectParameter fxView, fxProj;
        private EffectParameter fxFogRange, fxFogColor;
        private EffectParameter fxViewPos, fxSkin, fxMask;


        public void Dispose() {
            if (fx != null) fx.Dispose();
        }

        //=============================

        public void Load(GraphicsServices service, Texture2D skin, Texture2D mask) {
            fx = service.Content.Load<Effect>("block_small");
            fxView = fx.Parameters["matView"];
            fxProj = fx.Parameters["matProj"];
            fxViewPos = fx.Parameters["viewPos"];
            fxFogColor = fx.Parameters["fogColor"];
            fxFogRange = fx.Parameters["fogRange"];
            fxSkin = fx.Parameters["tex_skin"];
            fxMask = fx.Parameters["tex_mask"];
            //
            fxSkin.SetValue(skin);
            fxMask.SetValue(mask);
        }

        public void Update(GraphicsDevice graphics, Camera camera) {
            fxView.SetValue(camera.MatView);
            fxProj.SetValue(camera.MatProj);
            fxViewPos.SetValue(camera.Position);
            fxFogRange.SetValue(camera.FogRange);
            fxFogColor.SetValue(camera.FogColor);
        }
    }
}