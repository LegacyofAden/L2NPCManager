using System;

namespace GeoEngine.Exceptions
{
    public class GeoDataNotFoundException : Exception
    {
        public GeoDataNotFoundException(int geoX, int geoY) : base("GeoData not found at geoX: "+geoX+", geoY: "+geoY) {}
    }
}