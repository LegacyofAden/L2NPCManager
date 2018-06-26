using GeoEngine.Entity;

namespace GeoEngine.Geo
{
    public abstract class GeoCell
    {
        public static GeoCell[] EMPTY_ARRAY = new GeoCell[0];
        
        private GeoBlock _block;
        private SelectionState _selectionState;
        

        public GeoCell(GeoBlock block) {
            _block = block;
            _selectionState = SelectionState.NORMAL;
        }

        //=============================
        
        public GeoBlock getBlock() {return _block;}
        
        public SelectionState getSelectionState() {return _selectionState;}
        
        public void setSelectionState(SelectionState selectionState) {_selectionState = selectionState;}
        
        public float getRenderX() {return getGeoX();}

        public float getRenderY() {return getHeight() / 16f;}
        
        public float getRenderZ() {return getGeoY();}
        
        public void unload() {_block = null;}

        //-----------------------------
        
        public abstract bool isBig();
        
        public abstract short getHeight();
        
        public abstract short getNSWE();
        
        public abstract short getHeightAndNSWE();
        
        public abstract void addHeight(short height);
        
        public abstract int getGeoX();
        
        public abstract int getGeoY();
        
        public abstract int getCellX();
        
        public abstract int getCellY();
        
        public abstract void setHeightAndNSWE(short heightAndNSWE);
        
        public abstract void setNswe(short nswe);
    }
}