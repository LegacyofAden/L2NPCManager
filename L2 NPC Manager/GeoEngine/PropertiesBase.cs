using System.Collections.Generic;
using System.IO;

namespace GeoEngine
{
    public class PropertiesBase
    {
        private List<PropertyItem> items;


        public PropertiesBase() {
            items = new List<PropertyItem>();
        }

        //=============================

        public void Load(string filename) {
            using (FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read)) {
                Load(stream);
            }
        }

        public void Save(string filename, string name) {
            using (FileStream stream = File.Open(filename, FileMode.Create, FileAccess.Write)) {
                Save(stream, name);
            }
        }

        public void Clear() {
            items.Clear();
        }

        public string GetProperty(string key, string default_value) {
            PropertyItem i = findItem(key);
            return (i != null ? i.Value : default_value);
        }

        public void Put(string key, string value) {
            PropertyItem i = findItem(key);
            if (i == null) items.Add(i = new PropertyItem(key));
            i.Value = value;
        }

        //-----------------------------

        private PropertyItem findItem(string key) {
            foreach (PropertyItem i in items) {
                if (i.Key.Equals(key)) return i;
            }
            return null;
        }

        public void Load(Stream stream) {
            //...
        }

        public void Save(Stream stream, string name) {
            //...
        }
    }
}