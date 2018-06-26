
namespace GeoEngine.JOGL
{
    public class GLDisplay
    {
        private static GLDisplay _instance;

        private GLSelectionBox _selectionBox;
        

        public GLDisplay() {
            _selectionBox = new GLSelectionBox();
        }

        //=============================

        public static void init() {
            _instance = new GLDisplay();
        }
        
        public static GLDisplay getInstance() {return _instance;}

        public GLSelectionBox getSelectionBox() {return _selectionBox;}
    }
}