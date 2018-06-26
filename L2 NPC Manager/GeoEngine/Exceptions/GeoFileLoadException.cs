using System;

namespace GeoEngine.Exceptions
{
    public class GeoFileLoadException : Exception
    {
        public GeoFileLoadException(string file, bool l2j, string couse) : base("Failed loading Geo File "+(l2j ? "L2j" : "L2Off")+" '"+file+"', "+couse) {}
        
        public GeoFileLoadException(string file, bool l2j, Exception couse) : base("Failed loading Geo File "+(l2j ? "L2j" : "L2Off")+" '"+file+"', "+couse.Message) {}
    }
}