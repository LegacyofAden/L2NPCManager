using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;

namespace L2NPCManager.AI.Completion
{
    public class CompletionIcons
    {
        private static Dictionary<CompletionTypes, BitmapImage> images;
        private static string path;


        static CompletionIcons() {
            images = new Dictionary<CompletionTypes, BitmapImage>();
            path = Path.Combine(Environment.CurrentDirectory, "Resources\\Images\\Code");
        }

        //=============================

        public static void PreLoad() {
            Get(CompletionTypes.Class);
            Get(CompletionTypes.Event);
            Get(CompletionTypes.Field);
            Get(CompletionTypes.Method);
            Get(CompletionTypes.Property);
        }

        public static BitmapImage Get(CompletionTypes type) {
            BitmapImage img;
            if (images.TryGetValue(type, out img)) return img;
            if (type == CompletionTypes.None) return null;
            string f = Path.Combine(path, getUri(type));
            //
            img = new BitmapImage();
            img.BeginInit();
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.UriSource = new Uri(f);
            img.EndInit();
            img.Freeze();
            return (images[type] = img);
        }

        private static string getUri(CompletionTypes type) {
            switch (type) {
                case CompletionTypes.Class: return "class.png";
                case CompletionTypes.Event: return "event.png";
                case CompletionTypes.Field: return "field.png";
                case CompletionTypes.Method: return "method.png";
                case CompletionTypes.Property: return "property.png";
                case CompletionTypes.Struct: return "field.png"; // "struct.png";
                default: return null;
            }
        }
    }
}