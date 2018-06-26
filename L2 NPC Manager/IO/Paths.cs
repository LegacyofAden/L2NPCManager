using System;
using System.IO;
using System.Windows.Forms;

namespace L2NPCManager.IO
{
    public class Paths
    {
        public static string BasePath {get; private set;}
        public static string ResourcePath {get; private set;}
        public static string DataPath {get; set;}


        static Paths() {
            BasePath = Environment.CurrentDirectory;
            ResourcePath = Path.Combine(BasePath, "Resources");
        }

        //=============================

        public static string GetNPCData() {return Path.Combine(DataPath, "Script\\npcdata.txt");}
        public static string GetNpcPositionData() {return Path.Combine(DataPath, "Script\\npcpos.txt");}
        public static string GetItemData() {return Path.Combine(DataPath, "Script\\itemdata.txt");}
        public static string GetAIData() {return Path.Combine(DataPath, "Script\\ai.obj");}
        public static string GetGeoPath() {return Path.Combine(DataPath, "Geodata");}

        //-----------------------------

        public static bool GetDataPath() {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog() {
                Description = "Select L2 Directory",
                ShowNewFolderButton = false,
            }) {
                if (!string.IsNullOrEmpty(DataPath)) dialog.SelectedPath = DataPath;
                bool r = (dialog.ShowDialog() == DialogResult.OK);
                if (r) DataPath = dialog.SelectedPath;
                return r;
            }
        }
    }
}