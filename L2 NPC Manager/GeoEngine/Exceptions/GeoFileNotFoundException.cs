using System;

namespace GeoEngine.Exceptions
{
    public class GeoFileNotFoundException : Exception
    {
        public GeoFileNotFoundException(string file, bool l2j) : base("Geo File "+(l2j ? "L2j not found '"+file+"'" : "L2Off not found")) {}
    }
}