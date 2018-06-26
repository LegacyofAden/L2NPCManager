
namespace GeoEngine.Utils
{
    public class GeoByteBuffer : GeoReader, GeoWriter
    {
        private byte[] _data;
        private int _position;
        

        public static GeoByteBuffer allocate(int capacity) {
            return new GeoByteBuffer(capacity);
        }
        
        private GeoByteBuffer(int capacity) {
            _data = new byte[capacity];
        }
        
        public int position() {
            return _position;
        }
        
        public int capacity() {
            return _data.Length;
        }
        
        public void clear() {
            _position = 0;
        }
        
        public byte get() {
            return _data[_position++];
        }
        
        public short getShort() {
            return (short)(get() & 0xFF | get() << 8 & 0xFF00); 
        }
        
        public void put(byte value) {
            _data[_position++] = value;
        }
        
        public void putShort(short value) {
            _data[_position++] = (byte) (value & 0xFF);
            _data[_position++] = (byte) (value >> 8 & 0xFF);
        }
    }
}