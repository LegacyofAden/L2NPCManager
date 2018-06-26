using System.Collections.Generic;

namespace L2NPCManager.AI.Scripts
{
    public class ScriptCollection
    {
        private List<ScriptItem> items;


        public ScriptCollection() {
            items = new List<ScriptItem>();
        }

        //=============================

        public ScriptItem this[int index] {
            get {return items[index];}
        }

        public ScriptItem Get(string name) {
            return items.Find(i => i.Name.Equals(name));
        }

        public void Set(ScriptItem script) {
            if (string.IsNullOrEmpty(script.Name)) return;
            items.RemoveAll(i => i.Name.Equals(script.Name));
            items.Add(script);
        }

        public bool Contains(string class_name) {
            return items.Exists(i => i.Name.Equals(class_name));
        }

        public IEnumerator<ScriptItem> GetEnumerator() {
            return items.GetEnumerator();
        }

        public int Count {
            get {return items.Count;}
        }

        public void Clear() {items.Clear();}

        public void AddNotExists(ScriptItem script) {
            if (!items.Contains(script)) items.Add(script);
        }

        public List<ScriptItem> GetSource() {return items;}

        //-----------------------------

        public void SortByLevel() {
            int c, i, count;
            string super_name;
            ScriptItem script, super_script;
            short level;
            //
            level = 0;
            c = items.Count;
            do {
                count = 0;
                for (i = 0; i < c; i++) {
                    script = items[i];
                    //
                    if (level == 0) {
                        super_name = script.GetSuper();
                        if (string.IsNullOrEmpty(super_name) || super_name == "(null)") {
                            script.Level = 1;
                            count++;
                        }
                    } else {
                        if (script.Level != 0) continue;
                        //
                        super_name = script.GetSuper();
                        super_script = Get(super_name);
                        if (super_script != null && super_script.Level > 0) {
                            script.Level = (short)(super_script.Level + 1);
                            count++;
                        }
                    }
                }
                level++;
            }
            while (count > 0);
            //
            items.Sort(ClassLevelComparer);
        }

        private int ClassLevelComparer(ScriptItem a, ScriptItem b) {
            return a.Level.CompareTo(b.Level);
        }
    }
}