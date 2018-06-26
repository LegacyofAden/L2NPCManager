using L2NPCManager.AI.Scripts;
using System;
using System.IO;
using System.Text;

namespace L2NPCManager.AI
{
    public class ScriptParser
    {
        public delegate void ProgressEvent(int progress, string class_name);
        public event ProgressEvent OnProgress;

        public HeaderDocument Header {get; private set;}
        public ScriptCollection Items {get; private set;}
        private volatile bool cancellationPending;
        private int prev_progress;


        public ScriptParser() {
            Header = new HeaderDocument();
            Items = new ScriptCollection();
        }

        //=============================

        public void Cancel() {
            cancellationPending = true;
        }

        public void Load(string filename) {
            using (FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read))
            using (StreamReader reader = new StreamReader(stream)) {
                long stream_length = stream.Length;
                string line;
                //
                line = reader.ReadLine();
                while (line != null) {
                    if (!string.IsNullOrEmpty(line)) Header.AddItem(line, ' ');
                    else break;
                    line = reader.ReadLine();
                    //
                    if (cancellationPending) return;
                }
                //
                ScriptItem script;
                StringBuilder data = null;
                while (line != null) {
                    if (string.IsNullOrEmpty(line)) {
                        line = reader.ReadLine();
                        continue;
                    }
                    //
                    if (data == null) data = new StringBuilder();
                    if (data.Length != 0) data.AppendLine();
                    data.Append(line);
                    //
                    if (line.Equals("class_end")) {
                        script = parseClass(data.ToString());
                        data.Clear();
                        //
                        updateProgress(stream.Position, stream_length, script.Name);
                    }
                    //
                    line = reader.ReadLine();
                    if (cancellationPending) return;
                }
            }
        }

        private ScriptItem parseClass(string data) {
            ScriptItem script = new ScriptItem();
            script.Source = data;
            script.IsDecompiled = false;
            script.Name = script.GetName();
            //
            if (string.IsNullOrEmpty(script.Name)) throw new Exception("Class not Found!");
            Items.Set(script);
            return script;
        }

        private void updateProgress(long value, long max, string class_name) {
            if (OnProgress == null) return;
            //
            int p = (int)(value / (float)max * 100f);
            if (p != prev_progress) {
                OnProgress.Invoke(p, class_name);
                prev_progress = p;
            }
        }
    }
}