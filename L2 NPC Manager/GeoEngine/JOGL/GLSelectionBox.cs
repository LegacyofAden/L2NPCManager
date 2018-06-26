using GeoEngine.Geo;
using GeoEngine.Utils;

namespace GeoEngine.JOGL
{
    public class GLSelectionBox
    {
        private static int MIN_HEIGHT = 10;
        private static int MAX_HEIGHT = 1000;
        
        //private int _geoX;
        //private int _geoY;
        //private int _geoZ;
        private int _height;
        private bool _infHeight;
        

        public GLSelectionBox() {
            _height = MIN_HEIGHT;
        }

        //public void dispose(GL2 gl) {}

        //=============================
        
        //public void init(GL2 gl) {}

        public void toggleInfHeight() {
            setInfHeight(!isInfHeight());
        }
        
        public void setInfHeight(bool infHeight) {
            _infHeight = infHeight;
        }
        
        public bool isInfHeight() {
            return _infHeight;
        }
        
        //public bool isInside(GeoCell cell) {
        //    if (_geoX == int.MinValue) return false;
        //    if (cell.getBlock().getGeoX() != _geoX) return false;
        //    if (cell.getBlock().getGeoY() != _geoY) return false;
        //    if (_infHeight) return true;
        //    //
        //    int height = cell.getHeight();
        //    if (height < _geoZ - _height || height > _geoZ + _height) return false;
        //    return true;
        //}
        
        public void getAllCellsInside(GeoCell reference, GeoCell[] cells, FastArrayList<GeoCell> store) {
            if (_infHeight) {
                store.addAll(cells);
            } else {
                int height;
                int geoZ = reference.getHeight();
                foreach (GeoCell cell in cells) {
                    height = cell.getHeight();
                    if (height >= geoZ - _height && height <= geoZ + _height) store.add(cell);
                }
            }
        }
        
        public void addHeight(int height) {
            int newHeight = _height + height;
            if (newHeight < MIN_HEIGHT) newHeight = MIN_HEIGHT;
            else if (newHeight > MAX_HEIGHT) newHeight = MAX_HEIGHT;
            _height = newHeight;
        }
                
        //public void render(GL2 gl, GeoCell cell) {
        //    if (cell == null || cell.getBlock().getType() != Engine.GEO_BLOCK_TYPE_MULTILAYER) {
        //        _geoX = int.MinValue;
        //        return;
        //    }
        //    //
        //    float height = (isInfHeight() ? short.MaxValue : _height) / 16f;
        //    _geoX = cell.getBlock().getGeoX();
        //    _geoY = cell.getBlock().getGeoY();
        //    _geoZ = cell.getHeight();
        //    //
        //    gl.glPushMatrix();
        //    GLState.glColor4f(gl, GLColor.WHITE);
        //    gl.glTranslatef(_geoX, _geoZ / 16f, _geoY);
        //    //
        //    gl.glBegin(GL2.GL_LINE_LOOP);
        //    gl.glVertex3f(0f, -height, 0f);
        //    gl.glVertex3f(0f, -height, 8f);
        //    gl.glVertex3f(8f, -height, 8f);
        //    gl.glVertex3f(8f, -height, 0f);
        //    gl.glEnd();
        //    //
        //    gl.glBegin(GL2.GL_LINE_LOOP);
        //    gl.glVertex3f(0f, height, 0f);
        //    gl.glVertex3f(0f, height, 8f);
        //    gl.glVertex3f(8f, height, 8f);
        //    gl.glVertex3f(8f, height, 0f);
        //    gl.glEnd();
        //    //
        //    gl.glBegin(GL2.GL_LINES);
        //    gl.glVertex3f(0f, -height, 0f);
        //    gl.glVertex3f(0f, height, 0f);
        //    gl.glVertex3f(8f, -height, 0f);
        //    gl.glVertex3f(8f, height, 0f);
        //    gl.glVertex3f(0f, -height, 8f);
        //    gl.glVertex3f(0f, height, 8f);
        //    gl.glVertex3f(8f, -height, 8f);
        //    gl.glVertex3f(8f, height, 8f);
        //    gl.glEnd();
        //    //
        //    gl.glPopMatrix();
        //}
    }
}