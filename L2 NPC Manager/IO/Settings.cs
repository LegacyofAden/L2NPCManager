using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace L2NPCManager.IO
{
    public class Settings
    {
        private const string FILENAME = "Settings.xml";
        private const string TAG_ROOT = "settings";
        private const string TAG_ITEM = "item";
        private const string VAR_NAME = "name";

        private const string KEY_DATAPATH = "data-path";
        private const string KEY_SCRIPT_FONTSIZE = "script-font-size";
        private const string KEY_3D_BACKGROUND = "3D-background";
        private const string KEY_3D_MOVESPEED = "3D-move-speed";
        private const string KEY_3D_VIEWRANGE = "3D-view-range";

        private static string filename;
        private static XmlDocument document;
        private static Dictionary<string, string> items;


        static Settings() {
            document = new XmlDocument();
            items = new Dictionary<string, string>();
            //
            filename = Path.Combine(Paths.BasePath, FILENAME);
        }

        //=============================

        public static string Get(string key) {
            return Get(key, null);
        }
        public static string Get(string key, string default_value) {
            string r;
            if (items.TryGetValue(key, out r)) return r;
            return default_value;
        }
        public static void Set(string key, string value) {items[key] = value;}

        public static void Load() {
            items.Clear();
            if (!File.Exists(filename)) return;
            //
            string name, value;
            document.Load(filename);
            XmlElement root = document.DocumentElement;
            foreach (XmlElement node in root.ChildNodes) {
                try {
                    name = node.GetAttribute(VAR_NAME);
                    value = node.InnerText;
                    items.Add(name, value);
                }
                catch (Exception) {
                    Console.WriteLine("Failed to load setting ["+node.ToString()+"]!");
                }
            }
        }

        public static void Save() {
            XmlElement root = document.DocumentElement;
            if (root == null) {
                document.CreateXmlDeclaration("1.0", null, null);
                document.AppendChild(root = document.CreateElement(TAG_ROOT));
            } else {
                root.RemoveAll();
            }
            //
            XmlElement node;
            foreach (KeyValuePair<string, string> entry in items) {
                node = document.CreateElement(TAG_ITEM);
                node.SetAttribute(VAR_NAME, entry.Key);
                node.InnerText = entry.Value;
                root.AppendChild(node);
            }
            //
            string path = Path.GetDirectoryName(filename);
            if (!Directory.Exists(path)) {
                try {Directory.CreateDirectory(path);}
                catch (Exception error) {throw new Exception("Failed to create Settings folder! "+error.Message);}
            }
            document.Save(filename);
        }

        //-----------------------------

        public static string DataPath {
            get {return Settings.Get(Settings.KEY_DATAPATH);}
            set {Settings.Set(Settings.KEY_DATAPATH, value);}
        }

        public static int FontSize {
            get {return parseInt(Settings.Get(Settings.KEY_SCRIPT_FONTSIZE), 12);}
            set {Settings.Set(Settings.KEY_SCRIPT_FONTSIZE, value.ToString());}
        }

        public static float D3D_MoveSpeed {
            get {return parseFloat(Settings.Get(Settings.KEY_3D_MOVESPEED), 50f);}
            set {Settings.Set(Settings.KEY_3D_MOVESPEED, value.ToString());}
        }

        public static float D3D_ViewRange {
            get {return parseFloat(Settings.Get(Settings.KEY_3D_VIEWRANGE), 3000f);}
            set {Settings.Set(Settings.KEY_3D_VIEWRANGE, value.ToString());}
        }

        public static Color D3D_Background {
            get {return parseColor(Settings.Get(Settings.KEY_3D_BACKGROUND), Color.CornflowerBlue);}
            set {Settings.Set(Settings.KEY_3D_BACKGROUND, writeColor(value));}
        }

        //-----------------------------

        private static int parseInt(string value, int default_value) {
            if (string.IsNullOrEmpty(value)) return default_value;
            try {return int.Parse(value);} catch {return default_value;}
        }

        private static float parseFloat(string value, float default_value) {
            if (string.IsNullOrEmpty(value)) return default_value;
            try {return float.Parse(value);} catch {return default_value;}
        }

        private static Color parseColor(string value, Color default_value) {
            if (string.IsNullOrEmpty(value)) return default_value;
            //
            try {
                string[] tags = value.Split(',');
                if (tags.Length != 4) throw new ArgumentException("Invalid color value ["+value+"]!");
                //
                Color col = new Color();
                col.R = byte.Parse(tags[0]);
                col.G = byte.Parse(tags[1]);
                col.B = byte.Parse(tags[2]);
                col.A = byte.Parse(tags[3]);
                return col;
            }
            catch {return default_value;}
        }

        private static string writeColor(Color value) {
            StringBuilder data = new StringBuilder();
            data.Append(value.R.ToString());
            data.Append(',');
            data.Append(value.G.ToString());
            data.Append(',');
            data.Append(value.B.ToString());
            data.Append(',');
            data.Append(value.A.ToString());
            return data.ToString();
        }
    }
}