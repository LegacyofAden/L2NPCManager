using GeoEngine.Geo.Cells;
using GeoEngine.Utils;
using System;

namespace GeoEngine.Geo.Blocks
{
    public class GeoBlockMultiLayer : GeoBlock
    {
        private GeoCell[,][] _cells3D;
        private GeoCell[] _cells;
        private short _minHeight;
        private short _maxHeight;

        //=============================

        public GeoBlockMultiLayer(GeoReader reader, int geoX, int geoY, bool l2j) : base(geoX, geoY) {
            _cells3D = new GeoCell[GeoEngine.GEO_BLOCK_SHIFT, GeoEngine.GEO_BLOCK_SHIFT][];
            //
            int layers, count = 0;
            for (int x = 0; x < GeoEngine.GEO_BLOCK_SHIFT; x++) {
                for (int y = 0; y < GeoEngine.GEO_BLOCK_SHIFT; y++) {
                    layers = l2j ? reader.get() : reader.getShort();
                    if (!GeoEngine.layersValid(layers)) {
                        throw new Exception("Invalid layer count " + layers);
                    }
                    //
                    count += layers;
                    _cells3D[x,y] = new GeoCell[layers];
                    for (int i = layers; i-- > 0;) {
                        _cells3D[x, y][i] = new GeoCellCM(this, reader.getShort(), x, y);
                    }
                    //Util.quickSort(_cells3D[x,y], HeightComparator.Compare);
                }
            }
            //
            copyCells(count);
            calcMaxMinHeight();
        }
        
        private GeoBlockMultiLayer(GeoBlockFlat block) : base(block.getGeoX(), block.getGeoY()) {
            _cells3D = new GeoCell[GeoEngine.GEO_BLOCK_SHIFT, GeoEngine.GEO_BLOCK_SHIFT][];
            _cells = new GeoCell[GeoEngine.GEO_BLOCK_SHIFT * GeoEngine.GEO_BLOCK_SHIFT];
            //
            for (int x = 0; x < GeoEngine.GEO_BLOCK_SHIFT; x++) {
                for (int y = 0; y < GeoEngine.GEO_BLOCK_SHIFT; y++) {
                    GeoCell cell = new GeoCellCM(this, GeoEngine.convertHeightToHeightAndNSWEALL(block.getMinHeight()), x, y);
                    _cells3D[x, y] = new GeoCell[1];
                    _cells3D[x, y][0] = cell;
                    _cells[x * GeoEngine.GEO_BLOCK_SHIFT + y] = cell; 
                }
            }
            calcMaxMinHeight();
        }
        
        private GeoBlockMultiLayer(GeoBlockComplex block) : base(block.getGeoX(), block.getGeoY()) {
            _cells3D = new GeoCell[GeoEngine.GEO_BLOCK_SHIFT, GeoEngine.GEO_BLOCK_SHIFT][];
            _cells = new GeoCell[GeoEngine.GEO_BLOCK_SHIFT * GeoEngine.GEO_BLOCK_SHIFT];
            for (int x = 0; x < GeoEngine.GEO_BLOCK_SHIFT; x++) {
                for (int y = 0; y < GeoEngine.GEO_BLOCK_SHIFT; y++) {
                    GeoCell cell = new GeoCellCM(this, block.nGetCellByLayer(x, y, 0).getHeightAndNSWE(), x, y);
                    _cells3D[x, y] = new GeoCell[1];
                    _cells3D[x, y][0] = cell;
                    _cells[x * GeoEngine.GEO_BLOCK_SHIFT + y] = cell; 
                }
            }
            calcMaxMinHeight();
        }
        
        private GeoBlockMultiLayer(GeoBlockMultiLayer block) : base(block.getGeoX(), block.getGeoY()) {
            _cells3D = new GeoCell[GeoEngine.GEO_BLOCK_SHIFT, GeoEngine.GEO_BLOCK_SHIFT][];
            //
            int layers, count = 0;
            for (int x = 0; x < GeoEngine.GEO_BLOCK_SHIFT; x++) {
                for (int y = 0; y < GeoEngine.GEO_BLOCK_SHIFT; y++) {
                    layers = block.nGetLayerCount(x, y);
                    count += layers;
                    _cells3D[x, y] = new GeoCell[layers];
                    while (layers-- > 0) {
                        GeoCell cell = new GeoCellCM(this, block.nGetCellByLayer(x, y, layers).getHeightAndNSWE(), x, y);
                        _cells3D[x, y][layers] = cell;
                    }
                }
            }
            //
            copyCells(count);
            calcMaxMinHeight();
        }

        //=============================

        public static GeoBlockMultiLayer convertFrom(GeoBlock block) {
            if (block is GeoBlockFlat) {
                return new GeoBlockMultiLayer((GeoBlockFlat)block);
            }
            if (block is GeoBlockComplex) {
                return new GeoBlockMultiLayer((GeoBlockComplex)block);
            }
            return new GeoBlockMultiLayer((GeoBlockMultiLayer)block);
        }
                
        private void copyCells(int count) {
            _cells = new GeoCell[count];
            foreach (GeoCell[] cells1D in _cells3D) {
                foreach (GeoCell cell in cells1D) {
                    _cells[--count] = cell;
                }
            }
        }
                
        public override void updateLayerFor(GeoCell cell) {
            Util.quickSort(_cells3D[cell.getCellX(), cell.getCellY()], HeightComparator.Compare);
        }
        
        public override byte getType() {
            return GeoEngine.GEO_BLOCK_TYPE_MULTILAYER;
        }
        
        public override int nGetLayer(int geoX, int geoY, int z) {
            int cellX = GeoEngine.getCellXY(geoX);
            int cellY = GeoEngine.getCellXY(geoY);
            GeoCell[] heights = _cells3D[cellX, cellY];
            //
            short temp;
            int layer = 0, sub1, sub1Sq, sub2Sq = int.MaxValue;
            // from highest z (layer) to lowest z (layer)
            for (int i = heights.Length; i-- > 0;) {
                temp = heights[i].getHeightAndNSWE();
                sub1 = z - GeoEngine.getHeight(temp);
                sub1Sq = sub1 * sub1;
                if (sub1Sq < sub2Sq) {
                    sub2Sq = sub1Sq;
                    layer = i;
                } else {
                    break;
                }
            }
            return layer;
        }
        
        public override GeoCell nGetCellByLayer(int geoX, int geoY, int layer) {
            int cellX = GeoEngine.getCellXY(geoX);
            int cellY = GeoEngine.getCellXY(geoY);
            return _cells3D[cellX, cellY][layer];
        }
        
        public override int nGetLayerCount(int geoX, int geoY) {
            int cellX = GeoEngine.getCellXY(geoX);
            int cellY = GeoEngine.getCellXY(geoY);
            return _cells3D[cellX, cellY].Length;
        }

        public override GeoCell[] nGetLayers(int geoX, int geoY) {
            int cellX = GeoEngine.getCellXY(geoX);
            int cellY = GeoEngine.getCellXY(geoY);
            GeoCell[] layers = _cells3D[cellX, cellY];
            GeoCell[] result = new GeoCell[layers.Length];
            Array.Copy(layers, result, layers.Length);
            return result;
        }
        
        public void calcMaxMinHeight() {
            GeoCell[] heights;
            short height;
            short minHeight = short.MaxValue, maxHeight = short.MinValue;
            for (int x = GeoEngine.GEO_BLOCK_SHIFT, y, z; x-- > 0;) {
                for (y = GeoEngine.GEO_BLOCK_SHIFT; y-- > 0;) {
                    heights = _cells3D[x, y];
                    for (z = heights.Length; z-- > 0;) {
                        height = heights[z].getHeight();
                        minHeight = (short) Math.Min(height, minHeight);
                        maxHeight = (short) Math.Max(height, maxHeight);
                    }
                }
            }
            _minHeight = minHeight;
            _maxHeight = maxHeight;
        }
        
        public override void writeTo(GeoWriter writer, bool l2j) {
            GeoCell[] layers;
            GeoRegion.putType(writer, l2j, getType());
            for (int x = 0, y, z; x < GeoEngine.GEO_BLOCK_SHIFT; x++) {
                for (y = 0; y < GeoEngine.GEO_BLOCK_SHIFT; y++) {
                    layers = _cells3D[x, y];
                    if (l2j) {
                        writer.put((byte) layers.Length);
                    } else {
                        writer.putShort((short) layers.Length);
                    }
                    //
                    for (z = layers.Length; z-- > 0;) {
                        writer.putShort(layers[z].getHeightAndNSWE());
                    }
                }
            }
        }
        
        public override int getRequiredCapacity(bool l2j) {
            int capacity = l2j ? 1 : 2;
            for (int x = 0, y; x < GeoEngine.GEO_BLOCK_SHIFT; x++) {
                for (y = 0; y < GeoEngine.GEO_BLOCK_SHIFT; y++) {
                    capacity += l2j ? 1 : 2;
                    capacity += _cells3D[x, y].Length * 2;
                }
            }
            return capacity;
        }

        public override int getMaxLayerCount() {
            int maxLayerCount = 0;
            for (int x = GeoEngine.GEO_BLOCK_SHIFT, y; x-- > 0;) {
                for (y = GeoEngine.GEO_BLOCK_SHIFT; y-- > 0;) {
                    maxLayerCount = Math.Max(maxLayerCount, _cells3D[x, y].Length);
                }
            }
            return maxLayerCount;
        }

        public override GeoCell addLayer(int geoX, int geoY, short heightAndNSWE) {
            int cellX = GeoEngine.getCellXY(geoX);
            int cellY = GeoEngine.getCellXY(geoY);
            GeoCell cell = new GeoCellCM(this, heightAndNSWE, cellX, cellY);
            GeoCell[] layers = _cells3D[cellX, cellY];
            _cells3D[cellX, cellY] = Util.arrayAdd(layers, cell);
            _cells = Util.arrayAdd(_cells, cell);
            Util.quickSort(layers, HeightComparator.Compare);
            updateMinMaxHeight(cell.getHeight(), short.MinValue);
            return cell;
        }
        
        public override int removeCells(params GeoCell[] cells) {
            GeoCell[] layers;
            int layer, removed = 0;
            foreach (GeoCell cell in cells) {
                layers = _cells3D[cell.getCellX(), cell.getCellY()];
                if (layers.Length == 1) continue;
                //
                layer = Util.arrayLastIndexOf(layers, cell);
                if (layer == -1) {
                    throw new Exception("Smth went wrong dude: "+(cell.getBlock() == this));
                }
                //
                _cells3D[cell.getCellX(), cell.getCellY()] = Util.arrayRemoveAtUnsafe(layers, layer);
                _cells = Util.arrayRemoveLast(_cells, cell);
                GeoBlockSelector.getInstance().unselectGeoCell(cell);
                removed++;
            }
            calcMaxMinHeight();
            //GLDisplay.getInstance().getRenderSelector().forceUpdateFrustum();
            return removed;
        }

        public override GeoCell[] getCells() {return _cells;}
        
        public override short getMinHeight() {return _minHeight;}

        public override short getMaxHeight() {return _maxHeight;}
        
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
            //
            GeoCell[] cells1D;
            for (int i = _cells3D.GetLength(0), j, k; i-- > 0;) {
                for (j = _cells3D.GetLength(1); j-- > 0;) {
                    cells1D = _cells3D[i, j];
                    //
                    for (k = cells1D.Length; k-- > 0;) {
                        cells1D[k].unload();
                        cells1D[k] = null;
                    }
                    _cells3D[i, j] = null;
                }
            }
            _cells3D = null;
        }
        
        public override bool dataEquals(GeoReader reader) {
            if (getType() != GeoRegion.getType(reader, true)) return false;
            //
            GeoCell[] cells1D;
            for (int cellX = 0, cellY, layer; cellX < GeoEngine.GEO_BLOCK_SHIFT; cellX++) {
                for (cellY = 0; cellY < GeoEngine.GEO_BLOCK_SHIFT; cellY++) {
                    cells1D = _cells3D[cellX, cellY];
                    //
                    if (cells1D.Length != reader.get()) return false;
                    //
                    for (layer = cells1D.Length; layer-- > 0;) {
                        if (cells1D[layer].getHeightAndNSWE() != reader.getShort()) return false;
                    }
                }
            }
            return true;
        }

        //-----------------------------

        public class HeightComparator {
            public static int Compare(GeoCell cell_1, GeoCell cell_2) {
                return (cell_1.getHeight().CompareTo(cell_2.getHeight()));
            }
        }
    }
}