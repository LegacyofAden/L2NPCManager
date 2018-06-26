using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System;

namespace L2NPCManager.Controls
{
    public partial class MapControl : UserControl
    {
        private const int TILE_X_START = 16;
        private const int TILE_X_END = 26;
        private const int TILE_Y_START = 10;
        private const int TILE_Y_END = 25;

        private const int BTN_SIZE = 64;

        public delegate void SelectionEvent(int x, int y);
        public event SelectionEvent OnSelect;

        private MapButton[,] buttons;
        private BitmapImage image;
        private float imgTileWidth, imgTileHeight;
        private bool tilesCreated;


        public MapControl() {
            InitializeComponent();
        }

        //=============================

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {
            if (!DesignerProperties.GetIsInDesignMode(this)) {
                loadImage();
                //
                if (!tilesCreated) createTiles();
            }
        }

        private void btn_Click(object sender, RoutedEventArgs e) {
            MapButton btn = (MapButton)sender;
            if (OnSelect != null) OnSelect.Invoke(btn.X, btn.Y);
        }

        //-----------------------------

        private void createTiles() {
            GridLength size = GridLength.Auto; // new GridLength(1, GridUnitType.Star);
            int width = TILE_X_END - TILE_X_START + 1;
            int height = TILE_Y_END - TILE_Y_START + 1;
            //
            for (int x = 0; x < width; x++) {
                grid.ColumnDefinitions.Add(new ColumnDefinition() {Width = size});
            }
            for (int y = 0; y < height; y++) {
                grid.RowDefinitions.Add(new RowDefinition() {Height = size});
            }
            //
            buttons = new MapButton[width, height];
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    createButton(x, y);
                }
            }
            //
            tilesCreated = true;
        }

        private void createButton(int x, int y) {
            int mx = TILE_X_START + x;
            int my = TILE_Y_START + y;
            MapButton btn = buttons[x, y] = new MapButton(mx, my);
            btn.Width = BTN_SIZE;
            btn.Height = BTN_SIZE;
            btn.Style = (Style)Resources["tileStyle"];
            btn.SetValue(Grid.ColumnProperty, x);
            btn.SetValue(Grid.RowProperty, y);
            btn.Click += btn_Click;
            btn.Content = getImage(x, y);
            grid.Children.Add(btn);
            //
            if (mx == 22 && my == 22) {
                btn.IsCurrentTile = true;
            }
        }

        private void loadImage() {
            image = new BitmapImage(new Uri("pack://application:,,,/Resources/geodata.jpg"));
            //
            int img_width = image.PixelWidth;
            int img_height = image.PixelHeight;
            int map_width = TILE_X_END - TILE_X_START + 1;
            int map_height = TILE_Y_END - TILE_Y_START + 1;
            //
            imgTileWidth = img_width / (float)map_width;
            imgTileHeight = img_height / (float)map_height;
        }

        private Image getImage(int x, int y) {
            Int32Rect rect = new Int32Rect();
            rect.X = (int)(x * imgTileWidth);
            rect.Y = (int)(y * imgTileHeight);
            rect.Width = (int)imgTileWidth;
            rect.Height = (int)imgTileHeight;
            Image img = new Image();
            img.Source = new CroppedBitmap(image, rect);
            return img;
        }
    }
}