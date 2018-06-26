using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace L2NPCManager.IO.Documents
{
    public abstract class DocumentBase
    {
        public delegate void ProgressChangedEvent(double progress);
        public event ProgressChangedEvent OnProgressChanged;


        public DocumentBase() {}

        //=============================

        public abstract void Clear();
        protected abstract void LoadItem(string[] tags);
        protected abstract void SaveItems(StreamWriter writer);

        public void Load(string filename) {
            Clear();
            //
            using (FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read))
            using (StreamReader reader = new StreamReader(stream, true)) {
                double length = (double)stream.Length;
                //
                string line;
                double progress;
                while ((line = reader.ReadLine()) != null) {
                    LoadItem(line.Split('\t'));
                    //
                    if (length > 0) {
                        progress = stream.Position / length;
                        invokeProgressChanged(progress);
                    }
                }
            }
        }

        public void Save(string filename) {
            using (FileStream stream = File.Open(filename, FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(stream, Encoding.Unicode)) {
                SaveItems(writer);
            }
        }

        protected List<DocumentValue> readItem<T>(ref T item, ref string[] tags, string tag_start, string tag_end) where T : ItemBase {
            List<DocumentValue> values = new List<DocumentValue>();
            //
            string tag;
            int c = tags.Length;
            for (int i = 0; i < c; i++) {
                tag = tags[i];
                //
                if (i == 0 && !tag.Equals(tag_start, StringComparison.CurrentCultureIgnoreCase)) continue;
                if (tag.Equals(tag_start)) continue;
                if (tag.Equals(tag_end)) continue;
                //
                item.ReadData(i, tag);
            }
            //
            return values;
        }

        protected void writeItems<T>(StreamWriter writer, ObservableCollection<T> items, ref long progress, long total) where T : ItemBase {
            int i, c = items.Count;
            for (i = 0; i < c; i++) {
                if (i > 0) writer.WriteLine();
                items[i].WriteData(writer);
                writer.Flush();
                //
                progress++;
                double p = progress / (double)total;
                invokeProgressChanged(progress);
            }
        }

        //public T FindItemByName<T>(ObservableCollection<T> items, string name) where T : ItemBase {
        //    T item;
        //    int c = items.Count;
        //    for (int i = 0; i < c; i++) {
        //        item = items[i];
        //        if (item.Name.Equals(name)) return item;
        //    }
        //    return null;
        //}

        //public T FindItemByID<T>(ObservableCollection<T> items, string id) where T : ItemBase {
        //    T item;
        //    int c = items.Count;
        //    for (int i = 0; i < c; i++) {
        //        item = items[i];
        //        if (item.ID.Equals(id)) return item;
        //    }
        //    return null;
        //}

        protected bool matchesTag(ref string[] tags, string start_tag) {
            return (tags.Length > 0 && tags[0].Equals(start_tag, StringComparison.CurrentCultureIgnoreCase));
        }

        //-----------------------------

        protected void invokeProgressChanged(double progress) {
            if (OnProgressChanged != null) OnProgressChanged.Invoke(progress);
        }
    }
}