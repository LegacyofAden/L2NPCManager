using L2NPCManager.AI.Scripts;
using L2NPCManager.IO.Workers;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace L2NPCManager.AI.Workers
{
    public class ScriptDecompileWorker : ProgressWorker
    {
        public string Result {get; private set;}

        private string source;
        private string decompiler_filename;


        public ScriptDecompileWorker(string source) {
            this.source = source;
            decompiler_filename = Path.Combine(Environment.CurrentDirectory, "AI\\Decompiler\\denasc.exe");
        }

        //=========================

        protected override void Process() {
            StringBuilder data = new StringBuilder();
            decompileFile(source, data);
            Result = data.ToString();
        }

        private void decompileFile(string script_input, StringBuilder script_output) {
            string args = "--nasc --stdout -l";
            //
            using (Process process = new Process()) {
                process.StartInfo = new ProcessStartInfo() {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = decompiler_filename,
                    WorkingDirectory = Environment.CurrentDirectory,
                    Arguments = args,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
                process.ErrorDataReceived += process_ErrorDataReceived;
                process.Start();
                //
                process.BeginErrorReadLine();
                //
                using (StreamWriter writer = process.StandardInput) {
                    writer.Write(script_input);
                    writer.Flush();
                }
                //
                bool is_first = true;
                string line;
                StreamReader reader = process.StandardOutput;
                while (!reader.EndOfStream) {
                    if (is_first) is_first = false;
                    else script_output.AppendLine();
                    line = reader.ReadLine();
                    processLine(script_output, line);
                }
                //
                process.WaitForExit();
            }
        }

        private void process_ErrorDataReceived(object sender, DataReceivedEventArgs e) {
            Console.WriteLine("Decompiler: "+e.Data);
        }

        private void processLine(StringBuilder data, string line) {
            line = fixLine(line);
            //
            int i, len = line.Length;
            if (line.StartsWith("class ")) {
                ClassDefinition def = ClassDefinition.FromString(line);
                if (def != null && def.Super == "(null)") {
                    def.Super = null;
                    data.Append(def.ToString());
                    return;
                }
            }
            //
            i = 0;
            while (i < len) {
                if (tryReplace(data, ref line, ref i, "gg::", null)) continue;
                if (tryReplace(data, ref line, ref i, "myself::", "myself.")) continue;
                if (tryReplace(data, ref line, ref i, "::", ".")) continue;
                //
                if (tryReplace(data, ref line, ref i, "for( ", "for (")) continue;
                if (tryReplace(data, ref line, ref i, "for(", "for (")) continue;
                if (tryReplace(data, ref line, ref i, "for", "for")) continue;
                if (tryReplace(data, ref line, ref i, "while( ", "while (")) continue;
                if (tryReplace(data, ref line, ref i, "while(", "while (")) continue;
                if (tryReplace(data, ref line, ref i, "while", "while")) continue;
                if (tryReplace(data, ref line, ref i, "switch( ", "switch (")) continue;
                if (tryReplace(data, ref line, ref i, "switch(", "switch (")) continue;
                if (tryReplace(data, ref line, ref i, "switch", "switch")) continue;
                if (tryReplace(data, ref line, ref i, "if( ", "if (")) continue;
                if (tryReplace(data, ref line, ref i, "if(", "if (")) continue;
                if (tryReplace(data, ref line, ref i, "( ", "(")) continue;
                if (tryReplace(data, ref line, ref i, " )", ")")) continue;
                //
                data.Append(line[i]);
                i++;
            }
        }

        private string fixLine(string src) {
            int c = src.Length;
            for (int i = 1; i < c; i += 2) {
                if (src[i-1] != ' ' || src[i] != ' ') {
                    if (i == 0) return src;
                    //
                    string r = src.Substring(0, i).Replace("  ", "\t");
                    r += src.Substring(i);
                    return r;
                }
            }
            //
            return src.Replace("  ", "\t");
        }

        private bool tryReplace(StringBuilder data, ref string line, ref int pos, string old_tag, string new_tag) {
            int len = line.Length;
            int c = old_tag.Length;
            if (pos+c > len) return false;
            //
            for (int i = 0; i < c; i++) {
                if (line[pos+i] != old_tag[i]) return false;
            }
            //
            if (new_tag != null) data.Append(new_tag);
            pos = pos + c;
            return true;
        }
    }
}