using GeoEngine.Geo.Cells;
using GeoEngine.Utils;
using System;

namespace GeoEngine.Geo.Blocks
{
    public class GeoBlockComplex : GeoBlock
    {
        private GeoCell[] _cells;
        private short _minHeight;
        private short _maxHeight;

        //=============================

        public GeoBlockComplex(GeoReader reader, int geoX, int geoY, bool l2j) : base(geoX, geoY) {
            _cells = new GeoCell[GeoEngine.GEO_BLOCK_SHIFT * GeoEngine.GEO_BLOCK_SHIFT];
            for (int x = 0, y; x < GeoEngine.GEO_BLOCK_SHIFT; x++) {
                for (y = 0; y < GeoEngine.GEO_BLOCK_SHIFT; y++) {
                    _cells[indexOf(x, y)] = new GeoCellCM(this, reader.getShort(), x, y);
                }
            }
            calcMaxMinHeight();
        }
        
        private GeoBlockComplex(GeoBlockFlat block) : base(block.getGeoX(), block.getGeoY()) {
            _cells = new GeoCell[GeoEngine.GEO_BLOCK_SHIFT * GeoEngine.GEO_BLOCK_SHIFT];
            for (int x = 0, y; x < GeoEngine.GEO_BLOCK_SHIFT; x++) {
                for (y = 0; y < GeoEngine.GEO_BLOCK_SHIFT; y++) {
                    _cells[indexOf(x, y)] = new GeoCellCM(this, GeoEngine.convertHeightToHeightAndNSWEALL(block.getMinHeight()), x, y);
                }
            }
            calcMaxMinHeight();
        }
        
        private GeoBlockComplex(GeoBlockComplex block) : base(block.getGeoX(), block.getGeoY()) {
            _cells = new GeoCell[GeoEngine.GEO_BLOCK_SHIFT * GeoEngine.GEO_BLOCK_SHIFT];
            for (int x = 0, y; x < GeoEngine.GEO_BLOCK_SHIFT; x++) {
                for (y = 0; y < GeoEngine.GEO_BLOCK_SHIFT; y++) {
                    _cells[indexOf(x, y)] = new GeoCellCM(this, block.nGetCellByLayer(x, y, 0).getHeightAndNSWE(), x, y);
                }
            }
            calcMaxMinHeight();
        }
        
        private GeoBlockComplex(GeoBlockMultiLayer block) : base(block.getGeoX(), block.getGeoY()) {
                _cells = new GeoCell[GeoEngine.GEO_BLOCK_SHIFT * GeoEngine.GEO_BLOCK_SHIFT];
                for (int x = 0, y; x < GeoEngine.GEO_BLOCK_SHIFT; x++) {
                    for (y = 0; y < GeoEngine.GEO_BLOCK_SHIFT; y++) {
                        _cells[indexOf(x, y)] = new GeoCellCM(this, block.nGetCellByLayer(x, y, block.nGetLayerCount(x, y) - 1).getHeightAndNSWE(), x, y);
                    }
                }
                calcMaxMinHeight();
        }

        //=============================

        public static GeoBlockComplex convertFrom(GeoBlock block) {
            if (block is GeoBlockFlat) {
                return new GeoBlockComplex((GeoBlockFlat)block);
            }
            if (block is GeoBlockComplex) {
                return new GeoBlockComplex((GeoBlockComplex)block);
            }
            //
            return new GeoBlockComplex((GeoBlockMultiLayer)block);
        }
        
        private static int indexOf(int x, int y) {
            return x * GeoEngine.GEO_BLOCK_SHIFT + y;
        }
                
        public override byte getType() {
            return GeoEngine.GEO_BLOCK_TYPE_COMPLEX;
        }
        
        public override int nGetLayer(int geoX, int geoY, int z) {
            return 0;
        }
        
        public override GeoCell nGetCellByLayer(int geoX, int geoY, int layer) {
            int cellX = GeoEngine.getCellXY(geoX);
            int cellY = GeoEngine.getCellXY(geoY);
            return _cells[indexOf(cellX, cellY)];
        }
        
        public override int nGetLayerCount(int geoX, int geoY) {
            return 1;
        }

        public override GeoCell[] nGetLayers(int geoX, int geoY) {
            int cellX = GeoEngine.getCellXY(geoX);
            int cellY = GeoEngine.getCellXY(geoY);
            return new GeoCell[]{_cells[indexOf(cellX, cellY)]};
        }
        
        public void calcMaxMinHeight() {
            short minHeight = short.MaxValue, maxHeight = short.MinValue;
            for (int x = 0, y; x < GeoEngine.GEO_BLOCK_SHIFT; x++) {
                for (y = 0; y < GeoEngine.GEO_BLOCK_SHIFT; y++) {
                    GeoCell cell = _cells[indexOf(x, y)];
                    minHeight = Math.Min(cell.getHeight(), minHeight);
                    maxHeight = Math.Max(cell.getHeight(), maxHeight);
                }
            }
            _minHeight = minHeight;
            _maxHeight = maxHeight;
        }
        
        public override void writeTo(GeoWriter writer, bool l2j) {
            GeoRegion.putType(writer, l2j, getType());
            for (int x = 0, y; x < GeoEngine.GEO_BLOCK_SHIFT; x++) {
                for (y = 0; y < GeoEngine.GEO_BLOCK_SHIFT; y++) {
                    writer.putShort(_cells[indexOf(x, y)].getHeightAndNSWE());
                }
            }
        }
        
        public override int getRequiredCapacity(bool l2j) {
            return GeoEngine.GEO_BLOCK_SHIFT * GeoEngine.GEO_BLOCK_SHIFT * 2 + (l2j ? 1 : 2);
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
            return _minHeight;
        }
        
        public override short getMaxHeight() {
            return _maxHeight;
        }
        
        public override void updateMinMaxHeight(short newHeight, short oldHeight) {
            if (newHeight > _maxHeight) {
                _maxHeight = newHeight;
                //GLDisplay.getInstance().getTerrain().checkNeedUpdateVBO(false, true);
            } else if (newHeight < _minHeight) {
                _minHeight = newHeight;
                //GLDisplay.getInstance().getTerrain().checkNeedUpdateVBO(true, false);
            } else if (oldHeight == _maxHeight || oldHeight == _minHeight) {
                int oldMinHeight = _minHeight;
                int oldMaxHeight = _maxHeight;
                calcMaxMinHeight();
                //GLDisplay.getInstance().getTerrain().checkNeedUpdateVBO(_minHeight != oldMinHeight, _maxHeight != oldMaxHeight);
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
            //
            for (int x = 0, y; x < GeoEngine.GEO_BLOCK_SHIFT; x++) {
                for (y = 0; y < GeoEngine.GEO_BLOCK_SHIFT; y++) {
                    if (_cells[indexOf(x, y)].getHeightAndNSWE() != reader.getShort()) return false;
                }
            }
            return true;
        }
        
        public override void updateLayerFor(GeoCell cell) {}
    }
}