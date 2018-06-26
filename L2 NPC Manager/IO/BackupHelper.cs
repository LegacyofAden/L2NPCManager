using System;
using System.IO;
using System.Text;

namespace L2NPCManager.IO
{
    public class BackupHelper
    {
        private const string FORMAT = @"yyy-MM-dd_HH.mm.ss";

        public static void Create(string filename) {
            if (!File.Exists(filename)) return;
            //
            string path = Path.GetDirectoryName(filename);
            path = Path.Combine(path, "L2NPCManager_Backups");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            //
            string name = getFilename(filename);
            string backup_filename = Path.Combine(path, name);
            //
            try {File.Copy(filename, backup_filename);}
            catch (Exception error) {
                Console.WriteLine("Failed to create backup! "+error.Message);
            }
        }

        private static string getFilename(string filename) {
            StringBuilder r = new StringBuilder();
            r.Append(DateTime.Now.ToString(FORMAT));
            r.Append('.');
            r.Append(Path.GetFileName(filename));
            r.Append(".bak");
            return r.ToString();
        }
    }
}