using GraphicsLibrary.Geometry;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace GraphicsLibrary
{
    public class OctreeNode<T> where T : Model
    {
        private bool hasChildren;
        private BoundingBox Bounds, BoundsEx;
        private OctreeNode<T>[] subNodes;
        private List<T> models;
        private int level;


        public OctreeNode(int level) {
            this.level = level;
            Bounds = new BoundingBox();
            BoundsEx = new BoundingBox();
        }

        //=============================

        public void SetBounds(ref Vector3 min, ref Vector3 max) {
            BoundsEx.Min = Bounds.Min = min;
            BoundsEx.Max = Bounds.Max = max;
        }

        public bool Insert(T model, ref Vector3 center, int max_level) {
            ContainmentType c = Bounds.Contains(center);
            if (c == ContainmentType.Disjoint) return false;
            //
            if (level < max_level) {
                if (!hasChildren) createSubNodes(level+1);
                //
                OctreeNode<T> n;
                for (int i = 0; i < 8; i++) {
                    n = subNodes[i];
                    if (n.Insert(model, ref center, max_level)) {
                        if (n.BoundsEx.Min.X < BoundsEx.Min.X) BoundsEx.Min.X = n.BoundsEx.Min.X;
                        if (n.BoundsEx.Min.Y < BoundsEx.Min.Y) BoundsEx.Min.Y = n.BoundsEx.Min.Y;
                        if (n.BoundsEx.Min.Z < BoundsEx.Min.Z) BoundsEx.Min.Z = n.BoundsEx.Min.Z;
                        if (n.BoundsEx.Max.X > BoundsEx.Max.X) BoundsEx.Max.X = n.BoundsEx.Max.X;
                        if (n.BoundsEx.Max.Y > BoundsEx.Max.Y) BoundsEx.Max.Y = n.BoundsEx.Max.Y;
                        if (n.BoundsEx.Max.Z > BoundsEx.Max.Z) BoundsEx.Max.Z = n.BoundsEx.Max.Z;
                        return true;
                    }
                }
                return false;
            } else {
                if (models == null) models = new List<T>();
                models.Add(model);
                //
                if (model.Bounds.Min.X < BoundsEx.Min.X) BoundsEx.Min.X = model.Bounds.Min.X;
                if (model.Bounds.Min.Y < BoundsEx.Min.Y) BoundsEx.Min.Y = model.Bounds.Min.Y;
                if (model.Bounds.Min.Z < BoundsEx.Min.Z) BoundsEx.Min.Z = model.Bounds.Min.Z;
                if (model.Bounds.Max.X > BoundsEx.Max.X) BoundsEx.Max.X = model.Bounds.Max.X;
                if (model.Bounds.Max.Y > BoundsEx.Max.Y) BoundsEx.Max.Y = model.Bounds.Max.Y;
                if (model.Bounds.Max.Z > BoundsEx.Max.Z) BoundsEx.Max.Z = model.Bounds.Max.Z;
                //
                return true;
            }
        }

        public void Test(ref BoundingFrustum frustum, List<T> visible) {
            ContainmentType x;
            frustum.Contains(ref BoundsEx, out x);
            //
            if (!hasChildren) {
                if (models == null) return;
                if (x != ContainmentType.Disjoint) visible.AddRange(models);
            } else {
                if (x == ContainmentType.Contains) {
                    for (int i = 0; i < 8; i++) subNodes[i].AddAll(visible);
                } else if (x == ContainmentType.Intersects) {
                    for (int i = 0; i < 8; i++) subNodes[i].Test(ref frustum, visible);
                }
            }
        }

        public void AddAll(List<T> visible) {
            if (hasChildren) {
                for (int i = 0; i < 8; i++) subNodes[i].AddAll(visible);
            } else {
                if (models == null) return;
                visible.AddRange(models);
            }
        }

        //-----------------------------

        private void createSubNodes(int level) {
            BoundsEx = new BoundingBox();
            BoundsEx.Min = Bounds.Min;
            BoundsEx.Max = Bounds.Max;
            //
            Vector3 min, max, size;
            subNodes = new OctreeNode<T>[8];
            size = Bounds.Max - Bounds.Min;
            hasChildren = true;
            //
            min.X = Bounds.Min.X;
            min.Y = Bounds.Min.Y + size.Y/2f;
            min.Z = Bounds.Min.Z + size.Z/2f;
            max.X = Bounds.Max.X - size.X/2f;
            max.Y = Bounds.Max.Y;
            max.Z = Bounds.Max.Z;
            createNode(Nodes.TFL, ref min, ref max, level);
            //
            min.X = Bounds.Min.X + size.X/2f;
            min.Y = Bounds.Min.Y + size.Y/2f;
            min.Z = Bounds.Min.Z + size.Z/2f;
            max.X = Bounds.Max.X;
            max.Y = Bounds.Max.Y;
            max.Z = Bounds.Max.Z;
            createNode(Nodes.TFR, ref min, ref max, level);
            //
            min.X = Bounds.Min.X;
            min.Y = Bounds.Min.Y + size.Y/2f;
            min.Z = Bounds.Min.Z;
            max.X = Bounds.Max.X - size.X/2f;
            max.Y = Bounds.Max.Y;
            max.Z = Bounds.Max.Z - size.Z/2f;
            createNode(Nodes.TBL, ref min, ref max, level);
            //
            min.X = Bounds.Min.X + size.X/2f;
            min.Y = Bounds.Min.Y + size.Y/2f;
            min.Z = Bounds.Min.Z;
            max.X = Bounds.Max.X;
            max.Y = Bounds.Max.Y;
            max.Z = Bounds.Max.Z - size.Z/2f;
            createNode(Nodes.TBR, ref min, ref max, level);
            //
            min.X = Bounds.Min.X;
            min.Y = Bounds.Min.Y;
            min.Z = Bounds.Min.Z + size.Z/2f;
            max.X = Bounds.Max.X - size.X/2f;
            max.Y = Bounds.Max.Y - size.Y/2f;
            max.Z = Bounds.Max.Z;
            createNode(Nodes.BFL, ref min, ref max, level);
            //
            min.X = Bounds.Min.X + size.X/2f;
            min.Y = Bounds.Min.Y;
            min.Z = Bounds.Min.Z + size.Z/2f;
            max.X = Bounds.Max.X;
            max.Y = Bounds.Max.Y - size.Y/2f;
            max.Z = Bounds.Max.Z;
            createNode(Nodes.BFR, ref min, ref max, level);
            //
            min.X = Bounds.Min.X;
            min.Y = Bounds.Min.Y;
            min.Z = Bounds.Min.Z;
            max.X = Bounds.Max.X - size.X/2f;
            max.Y = Bounds.Max.Y - size.Y/2f;
            max.Z = Bounds.Max.Z - size.Z/2f;
            createNode(Nodes.BBL, ref min, ref max, level);
            //
            min.X = Bounds.Min.X + size.X/2f;
            min.Y = Bounds.Min.Y;
            min.Z = Bounds.Min.Z;
            max.X = Bounds.Max.X;
            max.Y = Bounds.Max.Y - size.Y/2f;
            max.Z = Bounds.Max.Z - size.Z/2f;
            createNode(Nodes.BBR, ref min, ref max, level);
        }

        private void createNode(Nodes node, ref Vector3 min, ref Vector3 max, int level) {
            OctreeNode<T> n = new OctreeNode<T>(level);
            n.BoundsEx.Min = n.Bounds.Min = min;
            n.BoundsEx.Max = n.Bounds.Max = max;
            subNodes[(int)node] = n;
        }

        private enum Nodes : byte {
            TFL = 0,
            TFR = 1,
            TBL = 2,
            TBR = 3,
            BFL = 4,
            BFR = 5,
            BBL = 6,
            BBR = 7,
        }
    }
}