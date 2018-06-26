using GeoEngine.Geo.Cells;
using GeoEngine.Utils;

namespace GeoEngine.Geo.Blocks
{
    public class GeoBlockFlat : GeoBlock
    {
        private GeoCell[] _cells;
        
        public static GeoBlockFlat convertFrom(GeoBlock block) {
            return new GeoBlockFlat(block);
        }
        
        public GeoBlockFlat(GeoReader reader, int geoX, int geoY, bool l2j) : base(geoX, geoY) {
            _cells = new GeoCell[] {new GeoCellFlat(this, reader.getShort())};
            //
            if (!l2j) reader.getShort();
        }
        
        private GeoBlockFlat(GeoBlock block) : base(block.getGeoX(), block.getGeoY()) {
            _cells = new GeoCell[] {new GeoCellFlat(this, block.getMinHeight())};
        }
        
        public override byte getType() {
            return GeoEngine.GEO_BLOCK_TYPE_FLAT;
        }
        
        public override int nGetLayer(int geoX, int geoY, int z) {
            return 0;
        }
        
        public override GeoCell nGetCellByLayer(int geoX, int geoY, int layer) {
            return _cells[0];
        }
        
        public override int nGetLayerCount(int geoX, int geoY) {
            return 1;
        }
        
        public override GeoCell[] nGetLayers(int geoX, int geoY) {
            return new GeoCell[] {_cells[0]};
        }
        
        public override void writeTo(GeoWriter writer, bool l2j) {
            GeoRegion.putType(writer, l2j, getType());
            writer.putShort(_cells[0].getHeight());
            if (!l2j) writer.putShort(_cells[0].getHeight());
        }
        
        public override int getRequiredCapacity(bool l2j) {
            return (l2j ? 3 : 5);
        }
        
        public override int getMaxLayerCount() {
            return 1;
        }
        
        public override GeoCell addLayer(int geoX, int geoY, short heightAndNSWE) {
            return null;
        }
        
        public override int removeCells(params GeoCell[] cells) {
            return 0;
        }
        
        public override GeoCell[] getCells() {
            return _cells;
        }
        
        public override short getMinHeight() {
            return _cells[0].getHeight();
        }
        
        public override short getMaxHeight() {
            return getMinHeight();
        }
        
        public override void updateMinMaxHeight(short newHeight, short oldHeight) {
            if (newHeight != oldHeight) {
                //GLDisplay.getInstance().getTerrain().checkNeedUpdateVBO(true, true);
            }
        }
        
        public override void unload() {
            for (int i = _cells.Length; i-- > 0;) {
                _cells[i].unload();
                _cells[i] = null;
            }
            _cells = null;
        }
        
        public override bool dataEquals(GeoReader reader) {
            if (getType() != GeoRegion.getType(reader, true)) return false;
            if (_cells[0].getHeight() != reader.getShort()) return false;
            return true;
        }
        
        public override void updateLayerFor(GeoCell cell) {}
    }
}