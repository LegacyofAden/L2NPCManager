
namespace GeoEngine
{
    public class PropertyItem
    {
        public string Key {get; set;}
        public string Value {get; set;}


        public PropertyItem() {}

        public PropertyItem(string key) {
            this.Key = key;
        }

        public PropertyItem(string key, string value) {
            this.Key = key;
            this.Value = value;
        }
    }
}