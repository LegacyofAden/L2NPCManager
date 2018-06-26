using GeoEngine.Geo.Blocks;
using GeoEngine.Utils;
using Microsoft.Xna.Framework;
using System;
using System.IO;

namespace GeoEngine.Geo
{
    public class GeoRegion
    {
        private string _file;
        private int _regionX;
        private int _regionY;
        private int _minGeoX;
        private int _maxGeoX;
        private int _minGeoY;
        private int _maxGeoY;
        private GeoBlock[,] _geoBlocks;
        private GeoByteBuffer[,] _geoBlocksData;


        public static byte getType(GeoReader reader, bool l2j) {
            if (l2j) return reader.get();
            switch (reader.getShort()) {
                case 0x0000: return GeoEngine.GEO_BLOCK_TYPE_FLAT;
                case 0x0040: return GeoEngine.GEO_BLOCK_TYPE_COMPLEX;
                default: return GeoEngine.GEO_BLOCK_TYPE_MULTILAYER;
            }
        }
        
        public static void putType(GeoWriter writer, bool l2j, byte type) {
            if (l2j) {
                writer.put(type);
            } else {
                switch (type) {
                    case GeoEngine.GEO_BLOCK_TYPE_FLAT:
                        writer.putShort((short) 0x0000);
                        break;
                    case GeoEngine.GEO_BLOCK_TYPE_COMPLEX:
                        writer.putShort((short) 0x0040);
                        break;
                    case GeoEngine.GEO_BLOCK_TYPE_MULTILAYER:
                        writer.putShort((short) 0x0080); // TODO check this
                        break;
                    default:
                        throw new ArgumentException("Unkown type: "+type);
                }
            }
        }
                
        public GeoRegion(int regionX, int regionY, GeoReader reader, bool l2j, string file) {
            _file = file;
            _regionX = regionX;
            _regionY = regionY;
            _minGeoX = GeoEngine.getGeoXY(regionX, 0);
            _maxGeoX = GeoEngine.getGeoXY(regionX, GeoEngine.GEO_REGION_SIZE - 1);
            _minGeoY = GeoEngine.getGeoXY(regionY, 0);
            _maxGeoY = GeoEngine.getGeoXY(regionY, GeoEngine.GEO_REGION_SIZE - 1);
            _geoBlocks = new GeoBlock[GeoEngine.GEO_REGION_SIZE, GeoEngine.GEO_REGION_SIZE];
            _geoBlocksData = new GeoByteBuffer[GeoEngine.GEO_REGION_SIZE, GeoEngine.GEO_REGION_SIZE];
            //
            GeoBlock block;
            GeoByteBuffer writer;
            for (int blockX = 0, blockY; blockX < GeoEngine.GEO_REGION_SIZE; blockX++) {
                for (blockY = 0; blockY < GeoEngine.GEO_REGION_SIZE; blockY++) {
                    block = readBlock(blockX, blockY, reader, l2j);
                    _geoBlocks[blockX, blockY] = block;
                    writer = GeoByteBuffer.allocate(block.getRequiredCapacity(true));
                    block.writeTo(writer, true);
                    _geoBlocksData[blockX, blockY] = writer;
                }
            }
        }
        
        private GeoBlock readBlock(int blockX, int blockY, GeoReader reader, bool l2j) {
            int geoX = GeoEngine.getGeoXY(_regionX, blockX);
            int geoY = GeoEngine.getGeoXY(_regionY, blockY);
            int type = getType(reader, l2j);
            switch (type) {
                case GeoEngine.GEO_BLOCK_TYPE_FLAT:
                    return new GeoBlockFlat(reader, geoX, geoY, l2j).setRegion(this);
                case GeoEngine.GEO_BLOCK_TYPE_COMPLEX:
                    return new GeoBlockComplex(reader, geoX, geoY, l2j).setRegion(this);
                case GeoEngine.GEO_BLOCK_TYPE_MULTILAYER:
                    return new GeoBlockMultiLayer(reader, geoX, geoY, l2j).setRegion(this);
                default:
                    throw new ArgumentException("Unknown type: "+type);
            }
        }
        
        public void convertBlock(GeoBlock block, byte type) {
            int blockX = block.getBlockX();
            int blockY = block.getBlockY();
            GeoBlock convertedBlock;
            //
            switch (type) {
                case GeoEngine.GEO_BLOCK_TYPE_FLAT:
                    convertedBlock = GeoBlockFlat.convertFrom(block).setRegion(this);
                    break;
                case GeoEngine.GEO_BLOCK_TYPE_COMPLEX:
                    convertedBlock = GeoBlockComplex.convertFrom(block).setRegion(this);
                    break;
                case GeoEngine.GEO_BLOCK_TYPE_MULTILAYER:
                    convertedBlock = GeoBlockMultiLayer.convertFrom(block).setRegion(this);
                    break;
                default:
                    throw new ArgumentException("Unkown type: "+type, "type");
            }
            //
            block.unload();
            _geoBlocks[blockX, blockY] = convertedBlock;
        }
        
        public void convertBlock(int blockX, int blockY, byte type) {
            convertBlock(_geoBlocks[blockX, blockY], type);
        }
        
        public void restoreBlock(GeoBlock block) {
            int blockX = block.getBlockX();
            int blockY = block.getBlockY();
            GeoByteBuffer reader = _geoBlocksData[blockX, blockY];
            reader.clear();
            block.unload();
            _geoBlocks[blockX, blockY] = readBlock(blockX, blockY, reader, true);
        }
        
        public void restoreBlock(int blockX, int blockY) {
            restoreBlock(_geoBlocks[blockX, blockY]);
        }
        
        public string getFile() {return _file;}
        
        public GeoBlock[,] getGeoBlocks() {return _geoBlocks;}
        
        public int getRegionX() {return _regionX;}
        
        public int getRegionY() {return _regionY;}
        
        public int getGeoX(int blockX) {
            return GeoEngine.getGeoXY(getRegionX(), blockX);
        }
        
        public int getGeoY(int blockY) {
            return GeoEngine.getGeoXY(getRegionY(), blockY);
        }
        
        public string getName() {
            return (getRegionX() + 10) + "_" + (getRegionY() + 10);
        }
        
        public byte nGetType(int geoX, int geoY) {
            int blockX = GeoEngine.getBlockXY(geoX);
            int blockY = GeoEngine.getBlockXY(geoY);
            return _geoBlocks[blockX, blockY].getType();
        }
        
        public GeoCell nGetCellChecked(int geoX, int geoY, int x) {
            if (geoX < _minGeoX || geoX > _maxGeoX || geoY < _minGeoY || geoY > _maxGeoY) return null;
            return nGetCell(geoX, geoY, x);
        }
        
        public GeoCell nGetCell(int geoX, int geoY, int x) {
            int blockX = GeoEngine.getBlockXY(geoX);
            int blockY = GeoEngine.getBlockXY(geoY);
            return _geoBlocks[blockX, blockY].nGetCell(geoX, geoY, x);
        }
        
        public GeoCell nGetCellByLayer(int geoX, int geoY, int layer) {
            int blockX = GeoEngine.getBlockXY(geoX);
            int blockY = GeoEngine.getBlockXY(geoY);
            return _geoBlocks[blockX, blockY].nGetCellByLayer(geoX, geoY, layer);
        }
        
        public GeoBlock getBlock(int geoX, int geoY) {
            int blockX = GeoEngine.getBlockXY(geoX);
            int blockY = GeoEngine.getBlockXY(geoY);
            return _geoBlocks[blockX, blockY];
        }
        
        public GeoBlock getBlockByBlockXY(int blockX, int blockY) {
            return _geoBlocks[blockX, blockY];
        }
        
        public void setBlock(int geoX, int geoY, GeoBlock block) {
            int blockX = GeoEngine.getBlockXY(geoX);
            int blockY = GeoEngine.getBlockXY(geoY);
            _geoBlocks[blockX, blockY] = block;;
        }
        
        public int nGetLayerCount(int geoX, int geoY) {
            int blockX = GeoEngine.getBlockXY(geoX);
            int blockY = GeoEngine.getBlockXY(geoY);
            return _geoBlocks[blockX, blockY].nGetLayerCount(geoX, geoY);
        }
        
        public GeoCell addLayer(int geoX, int geoY, short heightAndNSWE) {
            int blockX = GeoEngine.getBlockXY(geoX);
            int blockY = GeoEngine.getBlockXY(geoY);
            return _geoBlocks[blockX, blockY].addLayer(geoX, geoY, heightAndNSWE);
        }
        
        public void saveTo(Stream stream, bool l2j) { //, DialogSave observ) {
            if (!l2j) {
                Util.writeByte(_regionX + 10, stream);
                Util.writeByte(_regionY + 10, stream);
                //
                // TODO put real data here
                Util.writeBytes(new byte[16], stream);
            }
            //
            GeoWriter writer = GeoStreamWriter.wrap(stream);
            for (int blockX = 0, blockY; blockX < GeoEngine.GEO_REGION_SIZE; blockX++) {
                for (blockY = 0; blockY < GeoEngine.GEO_REGION_SIZE; blockY++) {
                    _geoBlocks[blockX, blockY].writeTo(writer, l2j);
                    //observ.updateProgressRegion(blockX * GeoEngine.GEO_REGION_SIZE + blockY, "["+blockX+"-"+blockY+"]");
                }
            }
        }
        
        public void unload() {
            for (int blockX = GeoEngine.GEO_REGION_SIZE, blockY; blockX-- > 0;) {
                for (blockY = GeoEngine.GEO_REGION_SIZE; blockY-- > 0;) {
                    _geoBlocks[blockX, blockY].unload();
                    _geoBlocks[blockX, blockY] = null;
                    _geoBlocksData[blockX, blockY] = null;
                }
            }
            _geoBlocks = null;
            _geoBlocksData = null;
        }
        
        public bool dataEqualFor(GeoBlock block) {
            GeoByteBuffer geoBlockData = _geoBlocksData[block.getBlockX(), block.getBlockY()];
            geoBlockData.clear();
            return block.dataEquals(geoBlockData);
        }
        
        public bool allDataEqual() {
            GeoByteBuffer geoBlockData;
            for (int blockX = GeoEngine.GEO_REGION_SIZE, blockY; blockX-- > 0;) {
                for (blockY = GeoEngine.GEO_REGION_SIZE; blockY-- > 0;) {
                    geoBlockData = _geoBlocksData[blockX, blockY];
                    geoBlockData.clear();
                    if (!_geoBlocks[blockX, blockY].dataEquals(geoBlockData)) return false;
                }
            }
            return true;
        }

        public void GetBounds(ref Vector2 min, ref Vector2 max) {
            min.X = GeoEngine.getWorldX(_minGeoX);
            min.Y = GeoEngine.getWorldY(_minGeoY);
            max.X = GeoEngine.getWorldX(_maxGeoX);
            max.Y = GeoEngine.getWorldY(_maxGeoY);
            // MAY need to add +1 to max geo values to get outer edge
        }
    }
}