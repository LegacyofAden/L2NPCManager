using System;

namespace GeoEngine.Exceptions
{
    public class GeoDataNotLoadedException : Exception
    {
        public GeoDataNotLoadedException(int geoX, int geoY) : base("GeoData not loaded at geoX: "+geoX+", geoY: "+geoY) {}
    }
}