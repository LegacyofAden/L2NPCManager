using System.IO;

namespace GeoEngine.Utils
{
    public class GeoStreamReader : GeoReader
    {
        private Stream stream;


        private GeoStreamReader(Stream stream) {
            this.stream = stream;
        }

        //=============================

        public static GeoStreamReader wrap(Stream stream) {
            return new GeoStreamReader(stream);
        }
        
        public byte get() {
            int read;
            //
            try {
                read = stream.ReadByte();
                if (read == -1) throw new EndOfStreamException();
            }
            catch (IOException e) {throw e;}
            //
            return (byte)read;
        }
        
        public short getShort() {
            return (short)(get() & 0xFF | get() << 8 & 0xFF00); 
        }
    }
}