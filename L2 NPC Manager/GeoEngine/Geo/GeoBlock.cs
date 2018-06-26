using GeoEngine.Utils;
using System;

namespace GeoEngine.Geo
{
    public abstract class GeoBlock
    {
        public static GeoBlock[] EMPTY_ARRAY = new GeoBlock[0];
        
        private GeoRegion _region;
        private int _geoX;
        private int _geoY;
        
        
        protected GeoBlock(int geoX, int geoY) {
            _geoX = geoX;
            _geoY = geoY;
        }

        //=============================
        
        public int getGeoX() {return _geoX;}
        
        public int getGeoY() {return _geoY;}
        
        public GeoRegion getRegion() {return _region;}
        
        public GeoBlock setRegion(GeoRegion region) {
            _region = region;
            return this;
        }
        
        public int getBlockX() {return GeoEngine.getBlockXY(_geoX);}
        
        public int getBlockY() {return GeoEngine.getBlockXY(_geoY);}
        
        public int getMaxGeoX() {
            return _geoX + GeoEngine.GEO_BLOCK_SHIFT - 1;
        }
        
        public int getMaxGeoY() {
            return _geoY + GeoEngine.GEO_BLOCK_SHIFT - 1;
        }
        
        public string getStringType() {
            return getStringType(getType());
        }
        
        public static string getStringType(byte type) {
            switch (type) {
                case GeoEngine.GEO_BLOCK_TYPE_FLAT: return "Flat";
                case GeoEngine.GEO_BLOCK_TYPE_COMPLEX: return "Complex";
                case GeoEngine.GEO_BLOCK_TYPE_MULTILAYER: return "MultiLayer";
            }
            throw new ArgumentException("Invalid Block Type!", "type");
        }
        
        public override string ToString() {
            return getStringType(getType())+" "+GeoEngine.getBlockXY(_geoX)+", "+GeoEngine.getBlockXY(_geoY);
        }
        
        public GeoCell nGetCell(int geoX, int geoY, int z) {
            return nGetCellByLayer(geoX, geoY, nGetLayer(geoX, geoY, z));
        }

        //-----------------------------
        
        public abstract byte getType();
        
        public abstract int nGetLayerCount(int geoX, int geoY);
        
        public abstract int nGetLayer(int geoX, int geoY, int z);
        
        public abstract GeoCell nGetCellByLayer(int geoX, int geoY, int layer);
        
        public abstract GeoCell[] nGetLayers(int geoX, int geoY);
        
        public abstract int getMaxLayerCount();
        
        public abstract GeoCell addLayer(int geoX, int geoY, short heightAndNSWE);
        
        public abstract int removeCells(params GeoCell[] cells);
        
        public abstract void writeTo(GeoWriter writer, bool l2j);
        
        public abstract int getRequiredCapacity(bool l2j);
        
        public abstract bool dataEquals(GeoReader reader);
        
        public abstract GeoCell[] getCells();
        
        public abstract short getMinHeight();
        
        public abstract short getMaxHeight();
        
        public abstract void updateMinMaxHeight(short height, short oldHeight);
        
        public abstract void updateLayerFor(GeoCell cell);
        
        public abstract void unload();
    }
}