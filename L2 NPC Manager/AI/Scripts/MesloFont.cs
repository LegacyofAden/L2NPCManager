using System;
using System.Windows.Media;

namespace L2NPCManager.AI.Scripts
{
    public class MesloFont
    {
        private static FontFamily font;
        private static bool is_loaded;

        //=============================

        public static FontFamily Get() {
            if (!is_loaded) {
                font = new FontFamily(new Uri("pack://application:,,,/"), "./Resources/Fonts/#Meslo LG L DZ");
                is_loaded = true;
            }
            return font;
        }
    }
}