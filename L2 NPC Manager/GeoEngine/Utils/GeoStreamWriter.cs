using System.IO;

namespace GeoEngine.Utils
{
    public class GeoStreamWriter : GeoWriter
    {
        private Stream stream;


        private GeoStreamWriter(Stream stream) {
            this.stream = stream;
        }

        //=============================
        
        public void put(byte value) {
            try {
                stream.WriteByte(value);
            }
            catch (IOException e) {throw e;}
        }
        
        public void putShort(short value) {
            try {
                stream.WriteByte((byte)(value & 0xFF));
                stream.WriteByte((byte)(value >> 8 & 0xFF));
            }
            catch (IOException e) {throw e;}
        }

        //-----------------------------
        
        public static GeoStreamWriter wrap(Stream stream) {
            return new GeoStreamWriter(stream);
        }
    }
}