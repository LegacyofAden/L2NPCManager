using L2NPCManager.AI.Scripts;
using L2NPCManager.Data;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace L2NPCManager.AI
{
    public class ScriptCompiler
    {
        public ScriptManager ScriptMgr {get; set;}
        public CompilerParser Result {get; private set;}

        private string compiler_path, compiler_filename;
        private string input_filename, output_filename;

        private static RegexReplacement[] regex;


        public ScriptCompiler() {
            compiler_path = Path.Combine(Environment.CurrentDirectory, "AI\\Compiler");
            compiler_filename = Path.Combine(compiler_path, "nasc.exe");
            input_filename = Path.Combine(compiler_path, "ai.nasc");
            output_filename = Path.Combine(compiler_path, "ai.obj");
        }

        //=============================

        private bool checkDuplicateEventHandlers(ScriptCollection modified) {
            bool r = true;
            Match m, mx;
            MatchCollection matches;
            string exp = @"\bEventHandler\s+([\w]+)";
            foreach (ScriptItem script in modified) {
                matches = Regex.Matches(script.Data, exp);
                int c = matches.Count;
                for (int i = 0; i < c; i++) {
                    m = matches[i];
                    for (int x = 0; x < i; x++) {
                        mx = matches[x];
                        if (mx.Groups[1].Value == m.Groups[1].Value) {
                            CompilerError e = new CompilerError();
                            e.Message = "Duplicate EventHandler \""+mx.Groups[1]+"\" found in \""+script.Name+"\"!";
                            Result.Errors.Add(e);
                            r = false;
                            break;
                        }
                    }
                }
            }
            return r;
        }

        public void Run() {
            ScriptCollection modified = getModified();
            Result = new CompilerParser();
            if (modified.Count > 0) {
                if (!checkDuplicateEventHandlers(modified)) return;
                //
                createRegex();
                modified.SortByLevel();
                writeItems(modified);
                compileItems();
                //
                Result.Load();
                if (!Result.IsSuccess) return;
                //
                ScriptItem x_script;
                ScriptParser parser = new ScriptParser();
                parser.Load(output_filename);
                foreach (ScriptItem script in parser.Items) {
                    x_script = ScriptMgr.Get(script.Name);
                    if (x_script != null) {
                        x_script.Source = script.Source;
                    }
                }
                //
                ScriptMgr.Header = parser.Header;
            } else {
                Result.IsSuccess = true;
            }
        }

        //-----------------------------

        private ScriptCollection getModified() {
            ScriptCollection r = new ScriptCollection();
            foreach (ScriptItem script in ScriptMgr.Items) {
                if (!script.IsModified) continue;
                r.AddNotExists(script);
                script.Level = 0;
                //
                string super_name = script.GetSuper();
                if (string.IsNullOrEmpty(super_name)) continue;
                ScriptItem super_script = ScriptMgr.Get(super_name);
                while (super_script != null) {
                    r.AddNotExists(super_script);
                    super_script.Level = 0;
                    //
                    super_name = super_script.GetSuper();
                    if (string.IsNullOrEmpty(super_name)) break;
                    super_script = ScriptMgr.Get(super_name);
                }
            }
            return r;
        }

        private void writeItems(ScriptCollection items) {
            using (FileStream stream = File.Open(input_filename, FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(stream, Encoding.Unicode)) {
                int c = items.Count;
                for (int i = 0; i < c; i++) {
                    if (i > 0) {
                        writer.WriteLine();
                        writer.WriteLine();
                    }
                    //
                    writeScript(writer, items[i].Data);
                }
            }
        }

        private void compileItems() {
            using (Process process = new Process()) {
                process.StartInfo = new ProcessStartInfo() {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = compiler_filename,
                    WorkingDirectory = compiler_path,
                    Arguments = "--quiet \""+input_filename+"\"",
                    RedirectStandardInput = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
                process.ErrorDataReceived += process_ErrorDataReceived;
                process.OutputDataReceived += process_OutputDataReceived;
                process.Start();
                //
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                //
                process.WaitForExit();
            }
        }

        private static void createRegex() {
            if (regex != null) return;
            //
            regex = new RegexReplacement[] {
                new RegexReplacement(@"\b(switch)\s*\(", "select ("),
                new RegexReplacement(@"\b(IsEventServer\(\))[\b]*", "0"),
            };
        }

        //-----------------------------

        private void process_OutputDataReceived(object sender, DataReceivedEventArgs e) {
            Console.WriteLine("Compiler: "+e.Data);
        }

        private void process_ErrorDataReceived(object sender, DataReceivedEventArgs e) {
            Console.WriteLine("Compiler: "+e.Data);
        }

        private static string getDirective(string index) {
            if (index == "1") return "set_compiler_opt base_event_type(@NTYPE_NPC_EVENT)";
            if (index == "2") return "set_compiler_opt base_event_type(@NTYPE_MAKER_EVENT)";
            throw new Exception("Invalid compiler directive ["+index+"]!");
        }

        private static void writeScript(StreamWriter writer, string data) {
            if (data == null) throw new ArgumentNullException("data", "Script cannot be empty!");
            int x = data.IndexOf("\r\n");
            if (x < 0) throw new Exception("Invalid class data!");
            //
            string header = data.Substring(0, x);
            string body = data.Substring(x);
            ClassDefinition def = ClassDefinition.FromString(header);
            if (def == null) throw new Exception("Invalid class data!");
            string directive = getDirective(def.Type);
            def.Type = null;
            //
            writer.WriteLine(directive);
            writer.WriteLine();
            writer.Write(def.ToString());
            //
            RegexReplacement r;
            int c = regex.Length;
            for (int i = 0; i < c; i++) {
                r = regex[i];
                body = r.Expression.Replace(body, r.Output);
            }
            //
            writer.Write(body);
        }
    }
}