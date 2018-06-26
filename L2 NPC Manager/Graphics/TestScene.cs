using GeoEngine.Geo;
using GraphicsLibrary;
using GraphicsLibrary.Geometry;
using L2NPCManager.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using L2NPCManager.Data.NpcPosition;
using GEO = GeoEngine.Geo.GeoEngine;

namespace L2NPCManager.Graphics
{
    public class TestScene : IDisposable
    {
        private const int REGION_SIZE = GEO.GEO_REGION_SIZE;
        private const float MOVE_SPEED = 40f;

        public delegate void ProgressEvent(int value);

        public Camera Camera;
        private Stopwatch watch;
        private List<GeoModel> models;
        private GeoScene vis_tree;
        private Color background;

        public BoundingBox Bounds {get {return vis_tree.Bounds;}}


        public TestScene() {
            watch = new Stopwatch();
            Camera = new Camera();
            models = new List<GeoModel>();
            vis_tree = new GeoScene();
            //
            UpdateSettings();
        }

        public void Dispose() {
            models.Clear();
        }

        //=============================

        public void LoadContent(GraphicsServices service) {
            if (!watch.IsRunning) {
                Camera.Init(50f, service.Graphics.Viewport);
                //
                vis_tree.Load(service);
                //
                watch.Start();
            }
        }

        public void LoadRegion(GeoRegion region, ProgressEvent ev) {
            vis_tree.ClearTree();
            models.Clear();
            //
            setTreeBounds(region);
            //
            int x, y, i = 0;
            int max = REGION_SIZE * REGION_SIZE;
            GeoBlock block;
            Vector3 min = new Vector3();
            for (y = 0; y < REGION_SIZE; y++) {
                for (x = 0; x < REGION_SIZE; x++) {
                    block = region.getBlockByBlockXY(x, y);
                    foreach (GeoModel m in createBlock(block)) {
                        if (i == 0) {
                            min = m.Position;
                        } else {
                            if (m.Position.X < min.X) min.X = m.Position.X;
                            if (m.Position.Y > min.Y) min.Y = m.Position.Y;
                            if (m.Position.Z < min.Z) min.Z = m.Position.Z;
                        }
                        //
                        i++;
                    }
                    //
                    if (ev != null) {
                        float p = (y*REGION_SIZE + x) / (float)max;
                        ev((int)(p * 100f));
                    }
                }
            }
            //
            Camera.Rotation.X = 45f;
            Camera.Position.X = min.X - 20f;
            Camera.Position.Z = min.Z - 20f;
            Camera.Position.Y = min.Y + 20f;
            Camera.UpdateWorldView();
        }

        public void SetDomains(List<DomainData> data) {
            //
        }

        public void UpdateSettings() {
            background = Settings.D3D_Background;
            Camera.ZFar = Settings.D3D_ViewRange;
            Camera.Speed = Settings.D3D_MoveSpeed;
            //
            Camera.FogColor = background.ToVector3();
            Camera.FogRange.X = Camera.ZFar * 0.74f;
            Camera.FogRange.Y = Camera.ZFar * 0.98f;
        }

        //-----------------------------

        private void setTreeBounds(GeoRegion region) {
            Vector3 min = new Vector3();
            Vector3 max = new Vector3();
            Vector2 geo_min = new Vector2();
            Vector2 geo_max = new Vector2();
            region.GetBounds(ref geo_min, ref geo_max);
            min.X = geo_min.X;
            min.Y = GEO.HEIGHT_MIN_VALUE;
            min.Z = geo_min.Y;
            max.X = geo_max.X;
            max.Y = GEO.HEIGHT_MAX_VALUE;
            max.Z = geo_max.Y;
            vis_tree.SetTreeBounds(ref min, ref max);
        }

        private IEnumerable<GeoModel> createBlock(GeoBlock block) {
            float ox, oy, oz;
            if (block.getType() == GEO.GEO_BLOCK_TYPE_FLAT) {
                GeoCell cell = block.nGetCell(0, 0, 0);
                //
                float xx = GEO.getWorldX(cell.getGeoX());
                float yy = GEO.getWorldY(cell.getGeoY());
                float zz = GEO.getHeight(cell.getHeight());
                yield return appendModel(xx, zz, yy, true);
            } else if (block.getType() == GEO.GEO_BLOCK_TYPE_COMPLEX) {
                GeoModel m;
                GeoCell cell;
                GeoCell[] cells = block.getCells();
                int c = cells.Length;
                for (int i = 0; i < c; i++) {
                    cell = cells[i];
                    ox = GEO.getWorldX(cell.getGeoX());
                    oz = GEO.getWorldY(cell.getGeoY());
                    oy = GEO.getHeight(cell.getHeight());
                    m = appendModel(ox, oy, oz, false);
                    updateMask(m, cell);
                    yield return m;
                }
            } else if (block.getType() == GEO.GEO_BLOCK_TYPE_MULTILAYER) {
                int c = block.getMaxLayerCount();
                //
                GeoModel m;
                int x, y;
                int min_x = block.getGeoX();
                int max_x = block.getMaxGeoX();
                int min_y = block.getGeoY();
                int max_y = block.getMaxGeoY();
                for (y = min_y; y <= max_y; y++) {
                    for (x = min_x; x <= max_x; x++) {
                        GeoCell[] cells = block.nGetLayers(x, y);
                        foreach (GeoCell cell in cells) {
                            ox = GEO.getWorldX(cell.getGeoX());
                            oz = GEO.getWorldY(cell.getGeoY());
                            oy = GEO.getHeight(cell.getHeight());
                            m = appendModel(ox, oy, oz, false);
                            updateMask(m, cell);
                            yield return m;
                        }
                    }
                }
            }
        }

        private void updateMask(GeoModel model, GeoCell cell) {
            short nswe = cell.getNSWE();
            if ((nswe & GEO.NSWE_ALL) == GEO.NSWE_ALL) model.Mask = Vector4.Zero;
            else {
                model.Mask.X = ((nswe & GEO.NORTH) == GEO.NORTH ? 0f : 1f);
                model.Mask.Y = ((nswe & GEO.EAST)  == GEO.EAST  ? 0f : 1f);
                model.Mask.Z = ((nswe & GEO.SOUTH) == GEO.SOUTH ? 0f : 1f);
                model.Mask.W = ((nswe & GEO.WEST)  == GEO.WEST  ? 0f : 1f);
            }
        }

        private GeoModel appendModel(float x, float y, float z, bool is_large) {
            float size_xz = (is_large ? 8f*GeoScene.BLOCK_SIZE : GeoScene.BLOCK_SIZE);
            //
            GeoModel i = new GeoModel();
            i.IsLarge = is_large;
            i.Position.X = x;
            i.Position.Y = y;
            i.Position.Z = z;
            i.SetBounds(size_xz, GeoScene.BLOCK_SIZE, size_xz);
            i.UpdateWorld();
            models.Add(i);
            //
            vis_tree.InsertTree(i);
            return i;
        }

        public void Render(GraphicsDevice graphics) {
            graphics.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, background, 1f, 0);
            //
            Camera.UpdateBounds(graphics.Viewport);
            //
            vis_tree.Render(graphics, Camera);
        }
    }
}