using GeoEngine.Exceptions;
using GeoEngine.Utils;
using L2NPCManager.IO;
using System;
using System.IO;

namespace GeoEngine.Geo
{
    public class GeoEngine
    {
        public const int CELL_SHIFT = 4; // 16 units each cell
        public const int GEO_BLOCK_SHIFT = 8; // 8 cells each block
        public const int GEO_REGION_SIZE = 1 << GEO_BLOCK_SHIFT;
        public const int GEO_REGION_MIN_FILE_SIZE = (GEO_REGION_SIZE * GEO_REGION_SIZE) * 3;
        public const int GEO_BLOCK_REGION_SHIFT = 11;
        
        public const byte GEO_BLOCK_TYPE_FLAT = 0;
        public const byte GEO_BLOCK_TYPE_COMPLEX = 1;
        public const byte GEO_BLOCK_TYPE_MULTILAYER = 2;
        
        public const short HEIGHT_MAX_VALUE = 16376;
        public const short HEIGHT_MIN_VALUE = -16384;
        
        public const int NSWE_MASK = 0x0000000F;
        public const int HEIGHT_MASK = 0x0000FFF0;
        
        public const byte EAST = (1 << 0);
        public const byte WEST = (1 << 1);
        public const byte SOUTH = (1 << 2);
        public const byte NORTH = (1 << 3);
        
        public const byte NEAST = ~EAST & NSWE_MASK;
        public const byte NWEST = ~WEST & NSWE_MASK;
        public const byte NSOUTH = ~SOUTH & NSWE_MASK;
        public const byte NNORTH = ~NORTH & NSWE_MASK;
        
        public const byte NORTHWEST = NORTH | WEST;
        public const byte NORTHEAST = NORTH | EAST;
        public const byte SOUTHWEST = SOUTH | WEST;
        public const byte SOUTHEAST = SOUTH | EAST;
        
        public const int MAP_MIN_X = -327680;
        public const int MAP_MAX_X = 229376;
        public const int MAP_MIN_Y = -262144;
        public const int MAP_MAX_Y = 294912;
        
        public const byte NSWE_NONE = 0;
        public const byte NSWE_ALL = EAST | WEST | SOUTH | NORTH;
        
        public const int GEOX_MIN = ((MAP_MIN_X - MAP_MIN_X) >> CELL_SHIFT);
        public const int GEOX_MAX = ((MAP_MAX_X - MAP_MIN_X) >> CELL_SHIFT);
        public const int GEOY_MIN = ((MAP_MIN_Y - MAP_MIN_Y) >> CELL_SHIFT);
        public const int GEOY_MAX = ((MAP_MAX_Y - MAP_MIN_Y) >> CELL_SHIFT);

        //=============================

        private static GeoEngine _instance;
        private GeoRegion _activeRegion;

        public GeoEngine() {}

        public void unload() {
            GeoRegion region = _activeRegion;
            if (region != null) {
                GeoBlockSelector.getInstance().unload();
                region.unload();
            }
            _activeRegion = null;
        }

        public void reloadGeo(int regionX, int regionY, bool l2j, string file) {
            if (file == null || !File.Exists(file)) throw new GeoFileNotFoundException(file, l2j);
            if (_activeRegion != null) throw new Exception("Geo must be unloaded first");
            //
            try {
                using (FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read)) {
                    GeoReader reader = GeoStreamReader.wrap(stream);
                    //
                    if (!l2j) {
                        for (int i = 18; i-- > 0;) reader.get();
                    }
                    _activeRegion = new GeoRegion(regionX, regionY, reader, l2j, file);
                }
            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw new GeoFileLoadException(file, l2j, e);
            }
        }
        
        public void reloadGeo(int regionX, int regionY, bool l2j) {
            string file = l2j
                    ? Path.Combine(Paths.GetGeoPath(), (regionX + 10)+'_'+(regionY + 10)+".l2j")
                    : searchL2OffGeoFile((regionX + 10), (regionY + 10));
            reloadGeo(regionX, regionY, l2j, file);
        }
        
        public GeoRegion getActiveRegion() {return _activeRegion;}

        //-----------------------------
                
        public static void init() {_instance = new GeoEngine();}
        public static GeoEngine getInstance() {return _instance;}

        //-----------------------------

        public static int getGeoX(int x) {return (x - MAP_MIN_X) >> CELL_SHIFT;}
        
        public static int getGeoY(int y) {return (y - MAP_MIN_Y) >> CELL_SHIFT;}
        
        public static int getWorldX(int geoX) {return (geoX << CELL_SHIFT) + MAP_MIN_X;}
        
        public static int getWorldY(int geoY) {return (geoY << CELL_SHIFT) + MAP_MIN_Y;}
        
        public static int getBlockXY(int geoXY) {return (geoXY >> 3) % GEO_REGION_SIZE;}
        
        public static int getCellXY(int geoXY) {return geoXY % GEO_BLOCK_SHIFT;}
        
        public static int getRegionXY(int geoXY) {return geoXY >> GEO_BLOCK_REGION_SHIFT;}
        
        public static int getCellIndex(int cellX, int cellY) {return (cellX << 3) + cellY;}
        
        public static int getGeoXY(int regionXY, int blockXY) {
            return (regionXY << GEO_BLOCK_REGION_SHIFT) + (blockXY << 3);
        }
        
        public static int getBlockIndex(int blockX, int blockY) {
            return (blockX << GEO_BLOCK_SHIFT) + blockY;
        }
        
        public static short getHeight(short height) {
            return (short)((height & HEIGHT_MASK) >> 1);
            //height &= HEIGHT_MASK;
            //height >>= 1;
            //return height;
        }
        
        public static short getHeight(int height) {
            return getHeight((short)height);
        }
        
        public static short getNSWE(short heightAndNSWE) {
            return (short) (heightAndNSWE & NSWE_MASK);
        }
        
        public static short getNSWE(int heightAndNSWE) {
            return (short) (heightAndNSWE & NSWE_MASK);
        }

        //-----------------------------

        public static short convertHeightToHeightAndNSWEALL(short height) {
            return (short)((height << 1) & HEIGHT_MASK | NSWE_ALL);
            //height <<= 1;
            //height &= HEIGHT_MASK;
            //height |= NSWE_ALL;
            //return height;
        }
        
        public static short updateHeightOfHeightAndNSWE(short heightAndNSWE, short height) {
            short x = (short)((height << 1) & HEIGHT_MASK);
            return (short)(x | getNSWE(heightAndNSWE));
            //height <<= 1;
            //height &= HEIGHT_MASK;
            //height |= getNSWE(heightAndNSWE);
            //return height;
        }
        
        public static short updateNSWEOfHeightAndNSWE(short heightAndNSWE, short NSWE) {
            short x = (short)(heightAndNSWE & HEIGHT_MASK);
            return (short)(x | NSWE);
            //heightAndNSWE &= HEIGHT_MASK;
            //heightAndNSWE |= NSWE;
            //return heightAndNSWE;
        }
        
        public static short getGeoHeightOfHeight(short height) {
            if (height <= HEIGHT_MIN_VALUE) return HEIGHT_MIN_VALUE;
            if (height >= HEIGHT_MAX_VALUE) return HEIGHT_MAX_VALUE;
            //
            return (short)(((height << 1) & HEIGHT_MASK) >> 1);
            //height <<= 1;
            //height &= HEIGHT_MASK;
            //height >>= 1;
            //return height;
        }
        
        public static bool layersValid(int layers) {
            return layers > 0 && layers <= byte.MaxValue;
        }
        
        public static string nameOfNSWE(int NSWE) {
            if ((NSWE & NSWE_ALL) == NSWE_ALL) return "NSWE";
            string nswe = "";
            //
            if ((NSWE & NORTH) == NORTH) nswe += "N";
            if ((NSWE & SOUTH) == SOUTH) nswe += "S";
            if ((NSWE & WEST) == WEST) nswe += "W";
            if ((NSWE & EAST) == EAST) nswe += "E";
            if (string.IsNullOrEmpty(nswe)) nswe = "NONE";
            return nswe;
        }
        
        public static bool checkNSWE(short NSWE, int x1, int y1, int x2, int y2) {
            if ((NSWE & NSWE_ALL) == NSWE_ALL) return true;
            //
            if (x2 > x1) {
                if ((NSWE & EAST) == 0) return false;
            } else if (x2 < x1) {
                if ((NSWE & WEST) == 0) return false;
            }
            //
            if (y2 > y1) {
                if ((NSWE & SOUTH) == 0) return false;
            } else if (y2 < y1) {
                if ((NSWE & NORTH) == 0) return false;
            }
            //
            return true;
        }
        
        public static bool hasGeoFile(int regionX, int regionY, bool l2j) {
            regionX += 10;
            regionY += 10;
            if (l2j) {
                string filename = Path.Combine(Paths.GetGeoPath(), regionX+'_'+regionY+".l2j");
                return File.Exists(filename);
            }
            return searchL2OffGeoFile(regionX, regionY) != null;
        }
        
        public static string searchL2OffGeoFile(int regionX, int regionY) {
            string geoFile = Paths.GetGeoPath();
            if (!Directory.Exists(geoFile)) return null;
            //
            string[] files = Directory.GetFiles(geoFile);
            foreach (string file in files) {
                if (!OFF_GEO_FILE_FILTER.process(file)) continue;
                int[] header = getL2OffHeader(file);
                if (header != null && header[0] == regionX && header[1] == regionY) return file;
            }
            return null;
        }
        
        public static bool hasValidL2OffHeader(string filename) {
            int[] header = getL2OffHeader(filename);
            return (header != null && header[0] >= 10 && header[0] <= 26 && header[1] >= 10 && header[1] <= 25);
        }

        public static int[] getL2OffHeader(string filename) {
            int[] result = null;
            using (FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read)) {
                try {result = new int[] {stream.ReadByte(), stream.ReadByte()};}
                catch (Exception) {}
            }
            return result;
        }
        
        public static int[] getHeaderOfL2jOrL2Off(string filename) {
            string name = Path.GetFileName(filename).ToLower();
            if (name.EndsWith(".dat")) return getL2OffHeader(filename);
            //
            name = name.Substring(0, name.LastIndexOf(".l2j"));
            string[] header = name.Split('_');
            return new int[] {
                int.Parse(header[0]),
                int.Parse(header[1]),
            };
        }
                
        private GeoRegion getGeoRegion(int geoX, int geoY) {
            GeoRegion region = _activeRegion;
            if (region == null) {
                throw new GeoDataNotLoadedException(geoX, geoY);
            }
            //
            int regionX = getRegionXY(geoX);
            int regionY = getRegionXY(geoY);
            if (regionX != region.getRegionX() || regionY != region.getRegionY()) {
                throw new GeoDataNotFoundException(geoX, geoY);
            }
            //
            return region;
        }
        
        public bool nHasGeo(int geoX, int geoY) {return getGeoRegion(geoX, geoY) != null;}
        
        public byte nGetType(int geoX, int geoY) {
            GeoRegion region = getGeoRegion(geoX, geoY);
            if (region != null) return region.nGetType(geoX, geoY);
            return GeoEngine.GEO_BLOCK_TYPE_FLAT;
        }
        
        public GeoCell nGetCell(int geoX, int geoY, int z) {
            GeoRegion region = getGeoRegion(geoX, geoY);
            if (region != null) return region.nGetCell(geoX, geoY, z);
            throw new GeoDataNotFoundException(geoX, geoY);
        }
        
        public GeoBlock getBlock(int geoX, int geoY) {
            GeoRegion region = getGeoRegion(geoX, geoY);
            if (region != null) return region.getBlock(geoX, geoY);
            throw new GeoDataNotFoundException(geoX, geoY);
        }
        
        public int nGetLayerCount(int geoX, int geoY) {
            GeoRegion region = getGeoRegion(geoX, geoY);
            if (region != null) {
                return region.nGetLayerCount(geoX, geoY);
            }
            return 1;
        }

        //=============================

        class OFF_GEO_FILE_FILTER {
            public static bool process(string filename) {
                if (!File.Exists(filename)) return false;
                if (!Path.GetExtension(filename).ToLower().Equals(".dat")) return false;
                if (!hasValidL2OffHeader(filename)) return false;
                return true;
            }
        }

        class GEO_FILE_FILTER {
            public static bool process(string filename) {
                if (!File.Exists(filename)) return false;
                if (Path.GetExtension(filename).ToLower().Equals(".dat")) {
                    return hasValidL2OffHeader(filename);
                }
                if (Path.GetExtension(filename).ToLower().Equals(".l2j")) {
                    // Pattern L2j_Pattern = Pattern.compile("\\d\\d_\\d\\d.l2j");
                    // return L2j_Pattern.matcher(name).matches();
                    return true;
                }
                return false;
            }
        }
    }
}