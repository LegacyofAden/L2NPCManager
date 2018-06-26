using System.Windows;
using System.Windows.Controls;

namespace L2NPCManager.Controls
{
    public class MapButton : Button
    {
        //public bool IsCurrentTile {get; set;}

        public int X {get; set;}
        public int Y {get; set;}


        public MapButton(int x, int y) {
            this.X = x;
            this.Y = y;
        }

        //=============================

        public static readonly DependencyProperty IsCurrentTileProperty =  DependencyProperty.Register("IsCurrentTile", typeof(bool), typeof(MapButton));

        public bool IsCurrentTile {
            get {return (bool)base.GetValue(IsCurrentTileProperty);}
            set {base.SetValue(IsCurrentTileProperty, value);}
        }
    }
}