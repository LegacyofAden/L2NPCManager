using System.IO;
using System.Reflection;
using System.Xml;

namespace L2NPCManager.AI.Completion
{
    public class CompletionDocument
    {
        private const string FILENAME = "L2NPCManager.Resources.nasc_format.xml";

        private const string TAG_ITEM = "item";
        private const string TAG_CLASS = "class";
        private const string TAG_EVENT = "event";
        private const string TAG_FIELD = "field";
        private const string TAG_METHOD = "method";
        private const string TAG_PROPERTY = "property";
        private const string TAG_STRUCT = "struct";
        private const string VAR_NAME = "name";
        private const string VAR_DESCRIPTION = "description";
        private const string VAR_DEPRECATED = "deprecated";
        private const string VAR_PARAMS = "params";

        public CompletionCollection Items;
        private XmlDocument document;


        public CompletionDocument() {
            document = new XmlDocument();
            Items = new CompletionCollection();
        }

        //=============================

        private static CompletionDocument instance;

        public static CompletionDocument GetInstance() {
            if (instance == null) {
                instance = new CompletionDocument();
                instance.Load();
            }
            return instance;
        }

        public void Load() {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(FILENAME)) {
                if (stream == null) return;
                //
                document.Load(stream);
                XmlElement root = document.DocumentElement;
                if (root == null) return;
                //
                parseGroup(root.ChildNodes, Items);
            }
        }

        public CompletionCollection Find(string[] prefix) {
            if (prefix == null || prefix.Length == 0) return Items;
            //
            foreach (CompletionItem i in Items) {
                if (i.Text.Equals(prefix[0])) return i.Find(prefix, 1);
            }
            return Items;
        }

        //-----------------------------

        private void parseGroup(XmlNodeList node, CompletionCollection group) {
            CompletionItem i;
            foreach (XmlElement e in node) {
                i = new CompletionItem();
                i.Type = getType(e.Name);
                i.Text = e.GetAttribute(VAR_NAME);
                i.Description = e.GetAttribute(VAR_DESCRIPTION);
                i.IsDeprecated = e.GetAttribute(VAR_DEPRECATED).Equals("true");
                i.Params = e.GetAttribute(VAR_PARAMS);
                group.Add(i);
                //
                if (e.HasChildNodes) {
                    parseGroup(e.ChildNodes, i.Items);
                }
            }
        }

        private CompletionTypes getType(string value) {
            switch (value) {
                case TAG_CLASS: return CompletionTypes.Class;
                case TAG_EVENT: return CompletionTypes.Event;
                case TAG_FIELD: return CompletionTypes.Field;
                case TAG_METHOD: return CompletionTypes.Method;
                case TAG_PROPERTY: return CompletionTypes.Property;
                case TAG_STRUCT: return CompletionTypes.Struct;
                default: return CompletionTypes.None;
            }
        }
    }
}