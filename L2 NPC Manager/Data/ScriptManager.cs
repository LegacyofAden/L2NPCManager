using L2NPCManager.AI;
using L2NPCManager.AI.Scripts;
using L2NPCManager.AI.Workers;

namespace L2NPCManager.Data
{
    public class ScriptManager
    {
        public static bool ENABLE_DECOMPILER = true;

        public bool HasChanges {get; set;}
        public ScriptCollection Items {get; set;}
        public HeaderDocument Header {get; set;}


        public ScriptManager() {
            Header = new HeaderDocument();
            Items = new ScriptCollection();
        }

        //=============================

        public int Count {get {return Items.Count;}}

        public void Clear() {
            Header.Clear();
            Items.Clear();
            HasChanges = false;
        }

        public ScriptItem Get(string class_name) {
            return Items.Get(class_name);
        }

        public void Set(ScriptItem script) {
            Items.Set(script);
        }

        public bool Contains(string class_name) {
            return Items.Contains(class_name);
        }

        public CompilerParser Compile(ScriptSaveWorker.ProgressEvent progress_event) {
            ScriptSaveWorker script_task = new ScriptSaveWorker();
            script_task.ScriptMgr = this;
            script_task.Run(progress_event);
            return script_task.CompilerResult;
        }
    }
}