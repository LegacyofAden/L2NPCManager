using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraphicsLibrary
{
    public class Camera
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector2 FogRange;
        public Vector3 FogColor;

        public float FieldOfView {get; private set;}
        public float Aspect {get; private set;}
        public float ZNear {get; set;}
        public float ZFar {get; set;}
        public float Speed {get; set;}

        private BoundingFrustum frustum;
        private Matrix matRotation;
        private Matrix matWorld;
        private Matrix matView;
        private Matrix matProj;

        private Vector3 target, up;

        public Matrix MatView {get {return matView;}}
        public Matrix MatProj {get {return matProj;}}


        public Camera() {
            matRotation = Matrix.Identity;
            matWorld = Matrix.Identity;
            matView = Matrix.Identity;
            matProj = Matrix.Identity;
            //
            ZNear = 1f;
            ZFar = 1000f;
            Speed = 1f;
            frustum = new BoundingFrustum(Matrix.Identity);
            up = Vector3.UnitY;
        }

        //=============================

        public void Init(float fov, Viewport viewport) {
            FieldOfView = MathHelper.ToRadians(fov);
            //
            UpdateWorldView();
            UpdateBounds(viewport);
        }

        public void MoveRel(ref Vector2 force) {
            MoveRel(force.Y, 0f, force.X);
        }
        public void MoveRel(float x, float y, float z) {
            Matrix matRot;
            Vector3 offset;
            offset.X = x * Speed;
            offset.Y = y * Speed;
            offset.Z = z * Speed;
            getRotation(out matRot);
            Vector3.Transform(ref offset, ref matRot, out offset);
            Position += offset;
        }

        public void UpdateBounds(Viewport viewport) {
            Aspect = viewport.AspectRatio;
            updateProjection();
        }

        public void UpdateWorldView() {
            target.X = 0f;
            target.Y = 0f;
            target.Z = 1f;
            //
            Matrix matRot;
            getRotation(out matRot);
            Vector3.Transform(ref target, ref matRot, out target);
            target += Position;
            //
            Matrix.CreateLookAt(ref Position, ref target, ref up, out matView);
            //
            frustum.Matrix = matView * matProj;
        }

        private void updateProjection() {
            Matrix.CreatePerspectiveFieldOfView(FieldOfView, Aspect, ZNear, ZFar, out matProj);
        }

        private void getRotation(out Matrix matRot) {
            float x, y, z;
            x = MathHelper.ToRadians(Rotation.X);
            y = MathHelper.ToRadians(Rotation.Y);
            z = MathHelper.ToRadians(Rotation.Z);
            Matrix.CreateFromYawPitchRoll(x, y, z, out matRot);
        }
    }
}