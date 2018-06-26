using L2NPCManager.IO.Documents;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace L2NPCManager.Data.Npc
{
    public class NpcDocument : DocumentBase
    {
        public ObservableCollection<NpcData> Items;


        public NpcDocument() {
            Items = new ObservableCollection<NpcData>();
        }

        //=============================

        protected override void LoadItem(string[] tags) {
            if (matchesTag(ref tags, NpcData.TAG_START)) {
                NpcData data = new NpcData();
                readItem(ref data, ref tags, NpcData.TAG_START, NpcData.TAG_END);
                data.UpdateDisplayValues();
                Items.Add(data);
            }
        }

        protected override void SaveItems(StreamWriter writer) {
            long p = 0;
            writeItems<NpcData>(writer, Items, ref p, Items.Count);
        }

        public override void Clear() {
            Items.Clear();
        }

        public void CreateBackup(string filename) {
            if (filename.EndsWith(".txt.back", StringComparison.CurrentCultureIgnoreCase)) return;
            if (!File.Exists(filename)) return;
            //
            string backup_filename = filename+".back";
            if (File.Exists(backup_filename)) {
                // do not overwrite backup files
            } else {
                File.Copy(filename, backup_filename);
            }
        }
    }
}