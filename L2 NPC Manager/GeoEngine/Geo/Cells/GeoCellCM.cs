using GeoEngine.Swing;

namespace GeoEngine.Geo.Cells
{
    public class GeoCellCM : GeoCell
    {
        private byte _cellX;
        private byte _cellY;
        private short _heightAndNSWE;
        

        public GeoCellCM(GeoBlock block, short heightAndNSWE, int cellX, int cellY) : base(block) {
            _cellX = (byte)cellX;
            _cellY = (byte)cellY;
            _heightAndNSWE = heightAndNSWE;
        }

        //=============================
        
        public override bool isBig() {return false;}
        
        public override short getHeight() {
            return GeoEngine.getHeight(_heightAndNSWE);
        }
        
        public override short getNSWE() {
            return GeoEngine.getNSWE(_heightAndNSWE);
        }
        
        public override short getHeightAndNSWE() {return _heightAndNSWE;}
        
        public override void addHeight(short height)
        {
            short oldHeight = getHeight();
            _heightAndNSWE = GeoEngine.updateHeightOfHeightAndNSWE(_heightAndNSWE, (short) (getHeight() + height));
            getBlock().updateLayerFor(this);
            getBlock().updateMinMaxHeight(getHeight(), oldHeight);
            //
            if (FrameMain.getInstance().isSelectedGeoCell(this)) {
                FrameMain.getInstance().setSelectedGeoCell(this);
            }
        }
        
        public override int getGeoX() {
            return getBlock().getGeoX() + getCellX();
        }
        
        public override int getGeoY() {
            return getBlock().getGeoY() + getCellY();
        }
        
        public override int getCellX() {return _cellX;}
        public override int getCellY() {return _cellY;}
        
        public override void setHeightAndNSWE(short heightAndNSWE) {
            short oldHeight = getHeight();
            _heightAndNSWE = heightAndNSWE;
            getBlock().updateLayerFor(this);
            getBlock().updateMinMaxHeight(getHeight(), oldHeight);
            //
            if (FrameMain.getInstance().isSelectedGeoCell(this)) {
                FrameMain.getInstance().setSelectedGeoCell(this);
            }
        }

        public override void setNswe(short nswe) {
            _heightAndNSWE = GeoEngine.updateNSWEOfHeightAndNSWE(_heightAndNSWE, nswe);
            //
            if (FrameMain.getInstance().isSelectedGeoCell(this)) {
                FrameMain.getInstance().setSelectedGeoCell(this);
            }
        }
    }
}