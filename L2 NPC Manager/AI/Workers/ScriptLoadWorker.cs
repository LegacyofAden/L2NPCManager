using L2NPCManager.AI.Scripts;
using L2NPCManager.IO;
using L2NPCManager.IO.Workers;

namespace L2NPCManager.AI.Workers
{
    public class ScriptLoadWorker : ProgressWorker
    {
        public HeaderDocument Header {get {return parser.Header;}}
        public ScriptCollection Items {get {return parser.Items;}}

        private string filename;
        private ScriptParser parser;


        public ScriptLoadWorker() {
            filename = Paths.GetAIData();
            parser = new ScriptParser();
            parser.OnProgress += parser_OnProgress;
        }

        //=========================

        protected override void Process() {
            parser.Load(filename);
        }

        private void parser_OnProgress(int progress, string class_name) {
            setProgress(progress);
        }
    }
}