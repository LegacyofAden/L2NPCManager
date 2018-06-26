
namespace L2NPCManager.IO.Documents
{
    public class DocumentValue
    {
        public string Key {get; set;}
        public string Value {get; set;}


        public DocumentValue() {}

        public DocumentValue(string tag, char separator = '=') {
            FromString(tag, separator);
        }

        public DocumentValue(string key, string value) {
            this.Key = key;
            this.Value = value;
        }

        //=============================

        public DocumentValue Clone() {
            DocumentValue item = new DocumentValue();
            item.Key = Key;
            item.Value = Value;
            return item;
        }

        public void FromString(string value, char separator = '=') {
            int x = value.IndexOf(separator);
            if (x >= 0) {
                Key = value.Substring(0, x);
                Value = value.Substring(x + 1);
            } else {
                Key = value;
                Value = null;
            }
        }

        public override string ToString() {return ToString('=');}
        public string ToString(char separator = '=') {
            string r = Key;
            if (Value != null) r += separator+Value;
            return r;
        }
    }
}