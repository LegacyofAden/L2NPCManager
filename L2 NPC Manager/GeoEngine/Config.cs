using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Text;

namespace GeoEngine
{
    public class Config
    {
        private static string CONFIG_FILE = "./G3DEditor.ini";
        public static int NSWE_TEXTURE_ID = 5;

        private static ConfigProperties PROPERTIES = new ConfigProperties();
        public static Color DEFAULT_COLOR_GUI_SELECTED = Color.Yellow;
        public static Color DEFAULT_COLOR_FLAT_NORMAL = Color.Blue;
        public static Color DEFAULT_COLOR_FLAT_HIGHLIGHTED = Color.Cyan;
        public static Color DEFAULT_COLOR_FLAT_SELECTED = Color.Magenta;
        public static Color DEFAULT_COLOR_COMPLEX_NORMAL = Color.Green;
        public static Color DEFAULT_COLOR_COMPLEX_HIGHLIGHTED = Color.Cyan;
        public static Color DEFAULT_COLOR_COMPLEX_SELECTED = Color.Magenta;
        public static Color DEFAULT_COLOR_MULTILAYER_NORMAL = Color.Red;
        public static Color DEFAULT_COLOR_MULTILAYER_HIGHLIGHTED = Color.Cyan;
        public static Color DEFAULT_COLOR_MULTILAYER_SELECTED = Color.Magenta;
        public static Color DEFAULT_COLOR_MULTILAYER_NORMAL_SPECIAL = mix(Color.White, Color.Red, 1.125f);
        public static Color DEFAULT_COLOR_MULTILAYER_HIGHLIGHTED_SPECIAL = mix(Color.White, Color.Cyan, 1.125f);
        public static Color DEFAULT_COLOR_MULTILAYER_SELECTED_SPECIAL = mix(Color.White, Color.Magenta, 1.125f);
        
        //public static string PATH_TO_GEO_FILES = "./data/geodata/";
        public static bool TERRAIN_DEFAULT_ON = false;
        public static bool V_SYNC = true;
        public static bool USE_TRANSPARENCY = true;
        public static bool USE_MULTITHREADING = true;
        public static bool DRAW_OUTLINE = false;
        
        public static Color COLOR_GUI_SELECTED = DEFAULT_COLOR_GUI_SELECTED;
        public static Color COLOR_FLAT_NORMAL = DEFAULT_COLOR_FLAT_NORMAL;
        public static Color COLOR_FLAT_HIGHLIGHTED = DEFAULT_COLOR_FLAT_HIGHLIGHTED;
        public static Color COLOR_FLAT_SELECTED = DEFAULT_COLOR_FLAT_SELECTED;
        public static Color COLOR_COMPLEX_NORMAL = DEFAULT_COLOR_COMPLEX_NORMAL;
        public static Color COLOR_COMPLEX_HIGHLIGHTED = DEFAULT_COLOR_COMPLEX_HIGHLIGHTED;
        public static Color COLOR_COMPLEX_SELECTED = DEFAULT_COLOR_COMPLEX_SELECTED;
        public static Color COLOR_MULTILAYER_NORMAL = DEFAULT_COLOR_MULTILAYER_NORMAL;
        public static Color COLOR_MULTILAYER_HIGHLIGHTED = DEFAULT_COLOR_MULTILAYER_HIGHLIGHTED;
        public static Color COLOR_MULTILAYER_SELECTED = DEFAULT_COLOR_MULTILAYER_SELECTED;
        public static Color COLOR_MULTILAYER_NORMAL_SPECIAL = DEFAULT_COLOR_MULTILAYER_NORMAL_SPECIAL;
        public static Color COLOR_MULTILAYER_HIGHLIGHTED_SPECIAL = DEFAULT_COLOR_MULTILAYER_HIGHLIGHTED_SPECIAL;
        public static Color COLOR_MULTILAYER_SELECTED_SPECIAL = DEFAULT_COLOR_MULTILAYER_SELECTED_SPECIAL;
                
        //=============================
        
        public static void load() {
            try {
                if (File.Exists(CONFIG_FILE)) {
                    PROPERTIES.Load(CONFIG_FILE);
                    //
                    //PATH_TO_GEO_FILES = PROPERTIES.GetProperty("PATH_TO_GEO_FILES", PATH_TO_GEO_FILES);
                    TERRAIN_DEFAULT_ON = bool.Parse(PROPERTIES.GetProperty("TERRAIN_DEFAULT_ON", TERRAIN_DEFAULT_ON.ToString()));
                    V_SYNC = bool.Parse(PROPERTIES.GetProperty("V_SYNC", V_SYNC.ToString()));
                    USE_TRANSPARENCY = bool.Parse(PROPERTIES.GetProperty("USE_TRANSPARENCY", USE_TRANSPARENCY.ToString()));
                    USE_MULTITHREADING = bool.Parse(PROPERTIES.GetProperty("USE_MULTITHREADING", USE_MULTITHREADING.ToString()));
                    DRAW_OUTLINE = bool.Parse(PROPERTIES.GetProperty("DRAW_OUTLINE", DRAW_OUTLINE.ToString()));
                    //
                    COLOR_FLAT_NORMAL = parseColor(PROPERTIES.GetProperty("COLOR_FLAT_NORMAL", COLOR_FLAT_NORMAL.ToString()));
                    COLOR_FLAT_HIGHLIGHTED = parseColor(PROPERTIES.GetProperty("COLOR_FLAT_HIGHLIGHTED", COLOR_FLAT_HIGHLIGHTED.ToString()));
                    COLOR_FLAT_SELECTED = parseColor(PROPERTIES.GetProperty("COLOR_FLAT_SELECTED", COLOR_FLAT_SELECTED.ToString()));
                    COLOR_COMPLEX_NORMAL = parseColor(PROPERTIES.GetProperty("COLOR_COMPLEX_NORMAL", COLOR_COMPLEX_NORMAL.ToString()));
                    COLOR_COMPLEX_HIGHLIGHTED = parseColor(PROPERTIES.GetProperty("COLOR_COMPLEX_HIGHLIGHTED", COLOR_COMPLEX_HIGHLIGHTED.ToString()));
                    COLOR_COMPLEX_SELECTED = parseColor(PROPERTIES.GetProperty("COLOR_COMPLEX_SELECTED", COLOR_COMPLEX_SELECTED.ToString()));
                    COLOR_MULTILAYER_NORMAL = parseColor(PROPERTIES.GetProperty("COLOR_MULTILAYER_NORMAL", COLOR_MULTILAYER_NORMAL.ToString()));
                    COLOR_MULTILAYER_HIGHLIGHTED = parseColor(PROPERTIES.GetProperty("COLOR_MULTILAYER_HIGHLIGHTED", COLOR_MULTILAYER_HIGHLIGHTED.ToString()));
                    COLOR_MULTILAYER_SELECTED = parseColor(PROPERTIES.GetProperty("COLOR_MULTILAYER_SELECTED", COLOR_MULTILAYER_SELECTED.ToString()));
                    //
                    COLOR_MULTILAYER_NORMAL_SPECIAL = parseColor(PROPERTIES.GetProperty("COLOR_MULTILAYER_NORMAL_SPECIAL", COLOR_MULTILAYER_NORMAL_SPECIAL.ToString()));
                    COLOR_MULTILAYER_HIGHLIGHTED_SPECIAL = parseColor(PROPERTIES.GetProperty("COLOR_MULTILAYER_HIGHLIGHTED_SPECIAL", COLOR_MULTILAYER_HIGHLIGHTED_SPECIAL.ToString()));
                    COLOR_MULTILAYER_SELECTED_SPECIAL = parseColor(PROPERTIES.GetProperty("COLOR_MULTILAYER_SELECTED_SPECIAL", COLOR_MULTILAYER_SELECTED_SPECIAL.ToString()));
                    //
                    NSWE_TEXTURE_ID = int.Parse(PROPERTIES.GetProperty("NSWE_TEXTURE_ID", NSWE_TEXTURE_ID.ToString()));
                }
            }
            catch (Exception error) {
                Console.WriteLine(error);
            }
            finally {
                checkConfigs();
            }
        }
        
        private static void checkConfigs() {
            //if (!Directory.Exists(PATH_TO_GEO_FILES)) {
            //    PATH_TO_GEO_FILES = "./data/geodata/";
            //}
        }
        
        public static void save() {
            PROPERTIES.Clear();
            //PROPERTIES.Put("PATH_TO_GEO_FILES", PATH_TO_GEO_FILES.ToString());
            PROPERTIES.Put("TERRAIN_DEFAULT_ON", TERRAIN_DEFAULT_ON.ToString());
            PROPERTIES.Put("V_SYNC", V_SYNC.ToString());
            PROPERTIES.Put("USE_TRANSPARENCY", USE_TRANSPARENCY.ToString());
            PROPERTIES.Put("USE_MULTITHREADING", USE_MULTITHREADING.ToString());
            PROPERTIES.Put("DRAW_OUTLINE", DRAW_OUTLINE.ToString());
            //
            PROPERTIES.Put("COLOR_FLAT_NORMAL", COLOR_FLAT_NORMAL.ToString());
            PROPERTIES.Put("COLOR_FLAT_HIGHLIGHTED", COLOR_FLAT_HIGHLIGHTED.ToString());
            PROPERTIES.Put("COLOR_FLAT_SELECTED", COLOR_FLAT_SELECTED.ToString());
            PROPERTIES.Put("COLOR_COMPLEX_NORMAL", COLOR_COMPLEX_NORMAL.ToString());
            PROPERTIES.Put("COLOR_COMPLEX_HIGHLIGHTED", COLOR_COMPLEX_HIGHLIGHTED.ToString());
            PROPERTIES.Put("COLOR_COMPLEX_SELECTED", COLOR_COMPLEX_SELECTED.ToString());
            PROPERTIES.Put("COLOR_MULTILAYER_NORMAL", COLOR_MULTILAYER_NORMAL.ToString());
            PROPERTIES.Put("COLOR_MULTILAYER_HIGHLIGHTED", COLOR_MULTILAYER_HIGHLIGHTED.ToString());
            PROPERTIES.Put("COLOR_MULTILAYER_SELECTED", COLOR_MULTILAYER_SELECTED.ToString());
            //
            PROPERTIES.Put("COLOR_MULTILAYER_NORMAL_SPECIAL", COLOR_MULTILAYER_NORMAL_SPECIAL.ToString());
            PROPERTIES.Put("COLOR_MULTILAYER_HIGHLIGHTED_SPECIAL", COLOR_MULTILAYER_HIGHLIGHTED_SPECIAL.ToString());
            PROPERTIES.Put("COLOR_MULTILAYER_SELECTED_SPECIAL", COLOR_MULTILAYER_SELECTED_SPECIAL.ToString());
            //
            PROPERTIES.Put("NSWE_TEXTURE_ID", NSWE_TEXTURE_ID.ToString());
            //
            PROPERTIES.save(CONFIG_FILE);
        }

        private static string writeColor(Color value) {
            StringBuilder data = new StringBuilder();
            data.Append(value.A);
            data.Append(',');
            data.Append(value.R);
            data.Append(',');
            data.Append(value.G);
            data.Append(',');
            data.Append(value.B);
            return data.ToString();
        }

        private static Color parseColor(string value) {
            string[] tags = value.Split(',');
            int len = tags.Length;
            //
            return new Color() {
                A = (len > 0 ? byte.Parse(tags[0]) : (byte)0),
                R = (len > 1 ? byte.Parse(tags[1]) : (byte)0),
                G = (len > 2 ? byte.Parse(tags[2]) : (byte)0),
                B = (len > 3 ? byte.Parse(tags[3]) : (byte)0),
            };
        }

        private static Color mix(Color c1, Color c2, float ratio) {
            return new Color(
                mix(c1.A, c2.A, ratio),
                mix(c1.R, c2.R, ratio),
                mix(c1.G, c2.G, ratio),
                mix(c1.B, c2.B, ratio));
        }

        private static byte mix(byte v1, byte v2, float ratio) {
            return (byte)Math.Min(((v1 * ratio) + v2) / 2f, 255f);
        }

        //=============================
        
        class ConfigProperties : PropertiesBase {
            public ConfigProperties() {}

            private void load(string file) {
                using (FileStream stream = File.Open(file, FileMode.Open)) {
                    base.Clear();
                    base.Load(stream);
                }
            }
            
            public void save(string filename) {
                using (FileStream stream = File.Open(filename, FileMode.Create, FileAccess.Write)) {
                    try {base.Save(stream, "G3DEditor Config");}
                    catch (Exception error) {Console.WriteLine(error);}
                }
            }
        }
    }
}