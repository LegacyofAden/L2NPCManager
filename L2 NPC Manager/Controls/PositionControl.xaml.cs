using GeoEngine.Geo;
using GraphicsLibrary;
using GraphicsLibrary.Controls;
using L2NPCManager.Data;
using L2NPCManager.Data.NpcPosition;
using L2NPCManager.Graphics;
using L2NPCManager.IO;
using L2NPCManager.IO.Workers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Engine = GeoEngine.Geo.GeoEngine;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;

namespace L2NPCManager.Controls
{
    public partial class PositionControl : UserControl, IDisposable
    {
        public GeoManager GeoMgr {get; set;}

        private GraphicsServices services;
        private GraphicsDeviceControl dx;
        private LoadWorker loadTask;
        private bool is_active, is_created;
        private bool is_loading, is_loaded;
        private double start_x, start_y;
        private bool is_left_captured, is_right_captured;
        private TestScene scene;
        private Vector2 move_force;

        private int region_x, region_y;
        private bool isMapVisible;


        public PositionControl() {
            InitializeComponent();
            //
            if (!DesignerProperties.GetIsInDesignMode(this)) {
                map.Visibility = Visibility.Collapsed;
                //
                gridBase.Children.Add(dx = new GraphicsDeviceControl());
                dx.SetValue(Grid.RowProperty, 1);
                dx.SetValue(Grid.ColumnProperty, 1);
                dx.LoadContent += dx_LoadContent;
                dx.RenderXna += dx_RenderXna;
                dx.WindowCreated += dx_WindowCreated;
                dx.HwndLButtonDown += dx_HwndLButtonDown;
                dx.HwndLButtonUp += dx_HwndLButtonUp;
                dx.HwndRButtonDown += dx_HwndRButtonDown;
                dx.HwndRButtonUp += dx_HwndRButtonUp;
                dx.HwndMouseMove += dx_HwndMouseMove;
                dx.Visibility = Visibility.Collapsed;
                //
                region_x = 12;
                region_y = 12;
                //
                try {
                    Engine.init();
                    GeoBlockSelector.init();
                }
                catch (Exception error) {
                    Console.WriteLine("Failed to initialize GeoEngine! "+error.Message);
                }
                //
                try {scene = new TestScene();}
                catch (Exception error) {
                    Console.WriteLine("Failed to initialize Scene! "+error.Message);
                }
            }
        }

        public void Dispose() {
            if (scene != null) scene.Dispose();
            if (services != null) services.Dispose();
        }

        //=============================

        public void Start() {
            if (!is_active) {
                if (is_created) dx.Start();
                if (is_loaded) dx.Visibility = Visibility.Visible;
                UpdateSettings();
                is_active = true;
            }
        }

        public void Stop() {
            dx.Visibility = Visibility.Collapsed;
            is_active = false;
        }

        public void UpdateSettings() {
            scene.UpdateSettings();
        }

        public void LoadRegion() {
            is_loading = true;
            lblLoading.Visibility = Visibility.Visible;
            dx.Visibility = Visibility.Collapsed;
            progress.Value = 0;
            map.IsEnabled = false;
            //
            loadTask = new LoadWorker();
            loadTask.Scene = scene;
            loadTask.GeoMgr = GeoMgr;
            loadTask.X = region_x;
            loadTask.Y = region_y;
            loadTask.OnComplete += loadTask_OnComplete;
            loadTask.Run(loadTask_Progress);
        }

        //-----------------------------

        private void btnMap_Click(object sender, RoutedEventArgs e) {
            isMapVisible = !isMapVisible;
            map.Visibility = (isMapVisible ? Visibility.Visible : Visibility.Collapsed);
        }

        private void dx_LoadContent(object sender, GraphicsServiceEventArgs e) {
            services = new GraphicsServices();
            services.Initialize(e.Service);
            scene.LoadContent(services);
            //
            LoadRegion();
        }

        private void loadTask_Progress() {
            progress.Value = loadTask.GetProgress();
        }

        private void loadTask_OnComplete(bool cancelled, Exception error) {
            is_loading = false;
            is_loaded = true;
            lblLoading.Visibility = Visibility.Collapsed;
            dx.Visibility = Visibility.Visible;
            map.IsEnabled = true;
            //
            if (!cancelled) {
                if (error != null) {
                    MessageBox.Show("Failed to load scene! "+error.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                } else {
                    dx.Visibility = Visibility.Visible;
                    //...
                }
            }
        }

        private void dx_RenderXna(object sender, GraphicsDeviceEventArgs e) {
            if (is_loading || !is_loaded) {
                e.Device.Clear(Color.Transparent);
                return;
            }
            //
            KeyboardState s = Keyboard.GetState();
            move_force.X = v(s[Keys.W]) - v(s[Keys.S]);
            move_force.Y = v(s[Keys.A]) - v(s[Keys.D]);
            //
            if (!move_force.Equals(Vector2.Zero)) {
                scene.Camera.MoveRel(ref move_force);
                scene.Camera.UpdateWorldView();
            }
            //
            scene.Render(e.Device);
        }

        private void map_OnSelect(int x, int y) {
            region_x = x - 10;
            region_y = y - 10;
            LoadRegion();
        }

        private float v(KeyState s) {return (s == KeyState.Down ? 1f : 0f);}

        private void dx_WindowCreated(object sender, EventArgs e) {
            if (is_active && !is_created) dx.Start();
            is_created = true;
        }

        private void dx_HwndLButtonDown(object sender, HwndMouseEventArgs e) {
            start_x = e.Position.X;
            start_y = e.Position.Y;
            dx.CaptureMouse();
            is_left_captured = true;
        }

        private void dx_HwndLButtonUp(object sender, HwndMouseEventArgs e) {
            is_left_captured = false;
            dx.ReleaseMouseCapture();
        }

        private void dx_HwndRButtonDown(object sender, HwndMouseEventArgs e) {
            start_x = e.Position.X;
            start_y = e.Position.Y;
            dx.CaptureMouse();
            is_right_captured = true;
        }

        private void dx_HwndRButtonUp(object sender, HwndMouseEventArgs e) {
            is_right_captured = false;
            dx.ReleaseMouseCapture();
        }

        private void dx_HwndMouseMove(object sender, HwndMouseEventArgs e) {
            if (is_left_captured) {
                float offset_x = (float)(e.Position.X - start_x) * 0.08f;
                float offset_y = (float)(e.Position.Y - start_y) * 0.06f;
                //
                Camera cam = scene.Camera;
                cam.Rotation.X = cycle(cam.Rotation.X - offset_x, -180f, 180f);
                cam.Rotation.Y = clamp(cam.Rotation.Y + offset_y, -88f, 88f);
                cam.UpdateWorldView();
            } else if (is_right_captured) {
                float offset_x = (float)(e.Position.X - start_x) * 0.04f;
                float offset_y = (float)(e.Position.Y - start_y) * 0.04f;
                //
                Camera cam = scene.Camera;
                cam.MoveRel(-offset_x, 0f, -offset_y);
                cam.UpdateWorldView();
            }
        }

        private float cycle(float value, float min, float max) {
            while (value < min) value += (max - min);
            while (value > max) value -= (max - min);
            return value;
        }

        private float clamp(float value, float min, float max) {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        //=============================

        class LoadWorker : ProgressWorker {
            public int X {get; set;}
            public int Y {get; set;}
            public TestScene Scene {get; set;}
            public GeoManager GeoMgr {get; set;}

            protected override void Process() {
                Engine geo = Engine.getInstance();
                geo.unload();
                geo.reloadGeo(X, Y, false);
                //
                GeoRegion region = geo.getActiveRegion();
                Scene.LoadRegion(region, progress_event);
                //
                List<DomainData> domains = getDomains();
                Scene.SetDomains(domains);
            }

            private List<DomainData> getDomains() {
                List<DomainData> r = new List<DomainData>();
                BoundingBox sceneBounds = Scene.Bounds;
                //
                string source;
                Bounds2D bounds = new Bounds2D();
                List<Vector4> points = new List<Vector4>();
                foreach (DomainData i in GeoMgr.Domains) {
                    source = i.GetValue(DomainData.VAR_BOUNDS, null);
                    if (string.IsNullOrEmpty(source)) continue;
                    //
                    points.Clear();
                    if (parseBounds(source, ref points, ref bounds)) {
                        if (!bounds.Intersects(sceneBounds)) continue;
                        //
                        i.Points = points.ToArray();
                        i.Bounds = bounds;
                        r.Add(i);
                    }
                }
                //
                return r;
            }

            private bool parseBounds(string source, ref List<Vector4> points, ref Bounds2D bounds) {
                source = StringUtils.Trim(source, "{", "}");
                string[] tags = StringUtils.Split(source, ';');
                //
                Vector4 p = new Vector4();
                string p_src;
                string[] p_tags;
                int c = tags.Length;
                for (int i = 0; i < c; i++) {
                    p_src = StringUtils.Trim(tags[i], "{", "}");
                    p_tags = p_src.Split(';');
                    if (p_tags.Length != 4) continue;
                    //
                    try {
                        p.X = float.Parse(p_tags[0]);
                        p.Y = float.Parse(p_tags[1]);
                        p.Z = float.Parse(p_tags[2]);
                        p.W = float.Parse(p_tags[3]);
                    }
                    catch (Exception) {return false;}
                    //
                    if (i == 0) {
                        bounds.MinX = bounds.MaxX = p.X;
                        bounds.MinY = bounds.MaxY = p.Y;
                    } else {
                        if (p.X < bounds.MinX) bounds.MinX = p.X;
                        if (p.X > bounds.MaxX) bounds.MaxX = p.X;
                        if (p.Y < bounds.MinY) bounds.MinY = p.Y;
                        if (p.Y > bounds.MaxY) bounds.MaxY = p.Y;
                    }
                    //
                    points.Add(p);
                }
                //
                return true;
            }

            private void progress_event(int value) {
                setProgress(value);
            }
        }
    }
}