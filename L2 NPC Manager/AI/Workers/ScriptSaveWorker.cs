using L2NPCManager.AI.Scripts;
using L2NPCManager.Data;
using L2NPCManager.IO;
using System.IO;
using System.Text;

namespace L2NPCManager.AI.Workers
{
    public class ScriptSaveWorker
    {
        public delegate void ProgressEvent(int value);
        public event ProgressEvent OnProgress;

        public ScriptManager ScriptMgr {get; set;}
        public CompilerParser CompilerResult {get; set;}

        //=========================

        public void Run(ProgressEvent on_progress) {
            ScriptCompiler compiler = new ScriptCompiler();
            compiler.ScriptMgr = ScriptMgr;
            compiler.Run();
            //
            CompilerResult = compiler.Result;
            if (!compiler.Result.IsSuccess) return;
            //
            ScriptMgr.Items.SortByLevel();
            int total = ScriptMgr.Count;
            //
            string filename = Paths.GetAIData();
            using (FileStream stream = File.Open(filename, FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(stream, Encoding.Unicode)) {
                int c = ScriptMgr.Header.Count;
                for (int i = 0; i < c; i++) {
                    if (i > 0) writer.WriteLine();
                    writer.Write(ScriptMgr.Header[i].ToString(' '));
                }
                //
                writer.WriteLine();
                writer.WriteLine();
                writer.WriteLine();
                //
                int index = 0;
                foreach (ScriptItem script in ScriptMgr.Items) {
                    script.IsModified = false;
                    //
                    if (index > 0) {
                        writer.WriteLine();
                        writer.WriteLine();
                    }
                    //
                    writer.Write(script.Source);
                    //
                    index++;
                    int p = (int)(index / (float)total * 100f);
                    if (OnProgress != null) OnProgress.Invoke(p);
                }
            }
        }
    }
}