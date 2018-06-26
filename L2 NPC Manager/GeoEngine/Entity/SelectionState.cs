using GeoEngine.Geo;
using Color = Microsoft.Xna.Framework.Color;
using Engine = GeoEngine.Geo.GeoEngine;

namespace GeoEngine.Entity
{
    public class SelectionState
    {
        public static float ALPHA = 0.7f;
        
        private Color _colorGuiSelected;
        private Color _colorFlat;
        private Color _colorComplex1;
        private Color _colorComplex2;
        private Color _colorMutliLayer1;
        private Color _colorMutliLayer2;
        private Color _colorMutliLayer1Special;
        private Color _colorMutliLayer2Special;
        

        private SelectionState(Color colorFlat, Color colorComplex, Color colorMutliLayer, Color colorMultiLayerSpecial) {
            _colorGuiSelected = getCol(Config.COLOR_GUI_SELECTED, ALPHA);
            _colorFlat = getCol(colorFlat, ALPHA);
            _colorComplex1 = getCol(colorComplex, ALPHA);
            _colorComplex2 = getCol(_colorComplex1, 0.85f, 0.85f, 0.85f);
            _colorMutliLayer1 = getCol(colorMutliLayer, ALPHA);
            _colorMutliLayer2 = getCol(_colorMutliLayer1, 0.85f, 0.85f, 0.85f);
            _colorMutliLayer1Special = getCol(colorMultiLayerSpecial, ALPHA);
            _colorMutliLayer2Special = getCol(_colorMutliLayer1Special, 0.85f, 0.85f, 0.85f);
        }

        //=============================
        
        public Color getColorGuiSelected() {
            return _colorGuiSelected;
        }

        public Color getColorFlat() {
            return _colorFlat;
        }

        public Color getColorComplex() {
            return _colorComplex1;
        }

        public Color getColorMultiLayer() {
            return _colorMutliLayer1;
        }
        
        public void updateSecondaryColors() {
            _colorComplex2.R = (byte)(_colorComplex1.R * 0.85f);
            _colorComplex2.G = (byte)(_colorComplex1.G * 0.85f);
            _colorComplex2.B = (byte)(_colorComplex1.B * 0.85f);
            //
            _colorMutliLayer2.R = (byte)(_colorMutliLayer1.R * 0.85f);
            _colorMutliLayer2.G = (byte)(_colorMutliLayer1.G * 0.85f);
            _colorMutliLayer2.B = (byte)(_colorMutliLayer1.B * 0.85f);
            //
            _colorMutliLayer2Special.R = (byte)(_colorMutliLayer1Special.R * 0.85f);
            _colorMutliLayer2Special.G = (byte)(_colorMutliLayer1Special.G * 0.85f);
            _colorMutliLayer2Special.B = (byte)(_colorMutliLayer1Special.B * 0.85f);
        }
        
        public Color getColorMultiLayerSpecial() {
            return _colorMutliLayer1Special;
        }

        public Color getColor(GeoBlock block, bool insideSelectionBox) {
            switch (block.getType()) {
                case Engine.GEO_BLOCK_TYPE_FLAT: return _colorFlat;
                case Engine.GEO_BLOCK_TYPE_COMPLEX:
                    return block.getBlockX() % 2 != block.getBlockY() % 2 ? _colorComplex2 : _colorComplex1;
                default:
                    if (insideSelectionBox) {
                        return block.getBlockX() % 2 != block.getBlockY() % 2 ? _colorMutliLayer2Special : _colorMutliLayer1Special;
                    } else {
                        return block.getBlockX() % 2 != block.getBlockY() % 2 ? _colorMutliLayer2 : _colorMutliLayer1;
                    }
            }
        }

        private static Color getCol(Color color, float alpha) {
            return new Color(color.R/255f, color.G/255f, color.B/255f, alpha);
        }

        private static Color getCol(Color color, float mul_r, float mul_g, float mul_b) {
            return new Color(color.R * mul_r/255f, color.G * mul_g/255f, color.B * mul_b/255f, color.A/255f);
        }

        //=============================

        public static SelectionState NORMAL = new SelectionState(Config.COLOR_FLAT_NORMAL, Config.COLOR_COMPLEX_NORMAL, Config.COLOR_MULTILAYER_NORMAL, Config.COLOR_MULTILAYER_NORMAL_SPECIAL);
        public static SelectionState HIGHLIGHTED = new SelectionState(Config.COLOR_FLAT_HIGHLIGHTED, Config.COLOR_COMPLEX_HIGHLIGHTED, Config.COLOR_MULTILAYER_HIGHLIGHTED, Config.COLOR_MULTILAYER_HIGHLIGHTED_SPECIAL);
        public static SelectionState SELECTED = new SelectionState(Config.COLOR_FLAT_SELECTED, Config.COLOR_COMPLEX_SELECTED, Config.COLOR_MULTILAYER_SELECTED, Config.COLOR_MULTILAYER_SELECTED_SPECIAL);
    }
}