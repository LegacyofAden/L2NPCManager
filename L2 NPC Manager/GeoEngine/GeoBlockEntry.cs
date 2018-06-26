using GeoEngine.Geo;
using GeoEngine.Utils;

namespace GeoEngine
{
    public class GeoBlockEntry
    {
        private GeoBlockEntry _next;
        private GeoBlockEntry _prev;
        private GeoBlock _key;
        private FastArrayList<GeoCell> _value;


        public GeoBlockEntry() {}

        //=========================
                
        public GeoBlockEntry getNext() {return _next;}
        public void setNext(GeoBlockEntry entry) {_next = entry;}
                
        public GeoBlockEntry getPrev() {return _prev;}
        public void setPrev(GeoBlockEntry entry) {_prev = entry;}
                
        public void remove() {
            getPrev().setNext(getNext());
            getNext().setPrev(getPrev());
            setKey(null);
            setValue(null);
        }
                
        public void addBefore(GeoBlockEntry entry) {
            setPrev(entry.getPrev());
            setNext(entry);
            //
            entry.getPrev().setNext(this);
            entry.setPrev(this);
        }
                
        public GeoBlock getKey() {return _key;}
                
        public void setKey(GeoBlock key) {_key = key;}
                
        public FastArrayList<GeoCell> getValue() {return _value;}
                
        public void setValue(FastArrayList<GeoCell> value) {_value = value;}
    }
}