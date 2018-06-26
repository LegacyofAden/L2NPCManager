using System;
using System.Collections.Generic;
using System.IO;

namespace L2NPCManager.AI
{
    public class CompilerParser
    {
        public bool IsSuccess {get; set;}
        public List<CompilerError> Errors {get; private set;}

        private string filename;


        public CompilerParser() {
            filename = Path.Combine(Environment.CurrentDirectory, "AI\\Compiler\\compile_msg.txt");
            Errors = new List<CompilerError>();
        }

        //=============================

        public void Load() {
            using (FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read))
            using (StreamReader reader = new StreamReader(stream)) {
                // Skip Header
                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();
                //
                string line;
                int index = 0;
                while ((line = reader.ReadLine()) != null) {
                    if (index == 0 && line.Equals("Success!!!")) {
                        IsSuccess = true;
                        continue;
                    } else if (line.StartsWith("Fail to compile")) continue;
                    //
                    Errors.Add(CompilerError.FromString(line));
                    index++;
                }
            }
        }
    }
}