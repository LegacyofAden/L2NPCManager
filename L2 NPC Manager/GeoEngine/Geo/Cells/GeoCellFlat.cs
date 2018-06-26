using GeoEngine.Geo.Blocks;
using GeoEngine.Swing;

namespace GeoEngine.Geo.Cells
{
    public class GeoCellFlat : GeoCell
    {
        private short _height;
        

        public GeoCellFlat(GeoBlockFlat block, short height) : base(block) {
            _height = GeoEngine.getGeoHeightOfHeight(height);
        }
        
        public override bool isBig() {return true;}
        
        public override short getHeight() {return _height;}
        
        public override short getNSWE() {return GeoEngine.NSWE_ALL;}
        
        public override short getHeightAndNSWE() {
            return GeoEngine.convertHeightToHeightAndNSWEALL(_height);
        }
        
        public override void addHeight(short height) {
            short oldHeight = getHeight();
            _height = GeoEngine.getGeoHeightOfHeight((short) (_height + height));
            getBlock().updateMinMaxHeight(_height, oldHeight);
            //
            if (FrameMain.getInstance().isSelectedGeoCell(this)) {
                FrameMain.getInstance().setSelectedGeoCell(this);
            }
        }
        
        public override int getGeoX() {return getBlock().getGeoX();}
        public override int getGeoY() {return getBlock().getGeoY();}
        
        public override int getCellX() {return 0;}
        public override int getCellY() {return 0;}
        
        public override void setHeightAndNSWE(short heightAndNSWE) {
            short oldHeight = getHeight();
            _height = GeoEngine.getHeight(heightAndNSWE);
            //
            getBlock().updateMinMaxHeight(_height, oldHeight);
            //
            if (FrameMain.getInstance().isSelectedGeoCell(this)) {
                FrameMain.getInstance().setSelectedGeoCell(this);
            }
        }
        
        public override void setNswe(short nswe) {
            if (FrameMain.getInstance().isSelectedGeoCell(this)) {
                FrameMain.getInstance().setSelectedGeoCell(this);
            }
        }
    }
}