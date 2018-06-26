using L2NPCManager.IO.Documents;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace L2NPCManager.AI
{
    public class HeaderDocument
    {
        public const string VAR_SIZE_OF_POINTER = "SizeofPointer";
        public const string VAR_SHARED_FACTORY_VERSION = "SharedFactoryVersion";
        public const string VAR_NPCH_VERSION = "NPCHVersion";
        public const string VAR_NASC_VERSION = "NASCVersion";
        public const string VAR_NPC_EVENT_HVERSION = "NPCEventHVersion";
        public const string VAR_DEBUG = "Debug";

        public delegate void ProgressChangedEvent(double progress);
        public event ProgressChangedEvent OnProgressChanged;

        private ObservableCollection<DocumentValue> items;


        public HeaderDocument() {
            items = new ObservableCollection<DocumentValue>();
        }

        //=============================

        public void Load(string filename) {
            Clear();
            //
            using (FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read))
            using (StreamReader reader = new StreamReader(stream, true)) {
                double length = (double)stream.Length;
                //
                string line;
                double p;
                DocumentValue value;
                while ((line = reader.ReadLine()) != null) {
                    value = new DocumentValue();
                    value.FromString(line, ' ');
                    items.Add(value);
                    //
                    if (length > 0) {
                        p = stream.Position / length;
                        invokeProgressChanged(p);
                    }
                }
            }
        }

        public void Save(string filename) {
            using (FileStream stream = File.Open(filename, FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(stream, Encoding.Unicode)) {
                int c = items.Count;
                for (int i = 0; i < c; i++) {
                    if (i > 0) writer.WriteLine();
                    writer.Write(items[i].ToString(' '));
                    writer.Flush();
                    //
                    double p = i / (double)c;
                    invokeProgressChanged(p);
                }
            }
        }

        public void Clear() {items.Clear();}

        public int Count {get {return items.Count;}}

        public DocumentValue this[int index] {get {return items[index];}}

        public void AddItem(string key, string value) {
            items.Add(new DocumentValue(key, value));
        }

        public void AddItem(string line, char separator = '=') {
            items.Add(new DocumentValue(line, separator));
        }

        //-----------------------------

        protected void invokeProgressChanged(double progress) {
            if (OnProgressChanged != null) OnProgressChanged.Invoke(progress);
        }
    }
}