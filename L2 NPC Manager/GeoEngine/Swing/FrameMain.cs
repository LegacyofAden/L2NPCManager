using GeoEngine.Geo;
using System;
using System.Windows.Threading;

namespace GeoEngine.Swing
{
    public class FrameMain
    {
        private static FrameMain _instance;

        private GeoCell _selectedCell;
        private bool _waitForUpdate;        


        public static void init() {
            _instance = new FrameMain();
        }
        
        public static FrameMain getInstance() {return _instance;}

        //-----------------------------

        public void setSelectedGeoCell(GeoCell cell) {
            _selectedCell = cell;
            //
            if (!_waitForUpdate) {
                _waitForUpdate = true;
                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => {onSelectedCellUpdated();}));
            }
        }

        public GeoCell getSelectedGeoCell() {
            return _selectedCell;
        }
        
        public bool isSelectedGeoCell(GeoCell cell) {
            return (_selectedCell == cell);
        }

        //-----------------------------

        private void onSelectedCellUpdated() {
            _waitForUpdate = false;
            //_panelNswe.onSelectedCellUpdated();
            //_panelCellInfo.onSelectedCellUpdated();
            //_panelBlockConvert.onSelectedCellUpdated();
            //_panelLayers.onSelectedCellUpdated();
            //_panelDirectNswe.onSelectedCellUpdated();
        }
    }
}