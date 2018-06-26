using GeoEngine.Entity;
using GeoEngine.JOGL;
using GeoEngine.Swing;
using GeoEngine.Utils;

namespace GeoEngine.Geo
{
    public class GeoBlockSelector
    {
        private static GeoBlockSelector _instance;

        private GeoBlockEntry[] _selected;
        private GeoBlockEntry _head;
        private GeoBlockEntry _tail;
        private FastArrayList<GeoCell> _temp;

                
        public GeoBlockSelector() {
            _selected = new GeoBlockEntry[GeoEngine.GEO_REGION_SIZE * GeoEngine.GEO_REGION_SIZE];
            for (int i = _selected.Length; i-- > 0;) {
                _selected[i] = new GeoBlockEntry();
            }
            _head = new GeoBlockEntry();
            _tail = new GeoBlockEntry();
            getHead().setPrev(getHead());
            getHead().setNext(getTail());
            getTail().setPrev(getHead());
            getTail().setNext(getTail());
            _temp = new FastArrayList<GeoCell>();
        }

        //=============================
        // GLobal

        public static void init() {
            _instance = new GeoBlockSelector();
        }

        public static GeoBlockSelector getInstance() {
            return _instance;
        }

        private static void setStateOf(FastArrayList<GeoCell> cells, SelectionState state) {
            for (int i = cells.size(); i-- > 0; ) {
                cells.getUnsafe(i).setSelectionState(state);
            }
        }

        private static void setStateOf(GeoCell[] cells, SelectionState state) {
            for (int i = cells.Length; i-- > 0; ) {
                cells[i].setSelectionState(state);
            }
        }

        //-----------------------------
        
        private GeoBlockEntry getEntry(GeoBlock block) {
            return _selected[block.getBlockX() * GeoEngine.GEO_REGION_SIZE + block.getBlockY()];
        }
        
        public GeoBlockEntry getHead() {return _head;}
        
        public GeoBlockEntry getTail() {return _tail;}
        
        public bool[] getSelectedTypes() {
            bool[] selectedTypes = new bool[3];
            for (GeoBlockEntry e = getHead(); (e = e.getNext()) != getTail();) {
                selectedTypes[e.getKey().getType()] = true;
                if (selectedTypes[0] && selectedTypes[1] && selectedTypes[2]) break;
            }
            return selectedTypes;
        }
        
        public int getSelectedTypesNotEqual(byte type) {
            int count = 0;
            for (GeoBlockEntry e = getHead(); (e = e.getNext()) != getTail();) {
                if (e.getKey().getType() != type) count++;
            }
            return count;
        }
        
        public void convertSelectedToType(byte type) {
            GeoRegion region = GeoEngine.getInstance().getActiveRegion();
            if (region == null) return;
            //
            bool convertedAny = false;
            for (GeoBlockEntry e = getHead(), p; (e = e.getNext()) != getTail();) {
                if (e.getKey().getType() != type) {
                    region.convertBlock(e.getKey(), type);
                    convertedAny = true;
                    p = e.getPrev();
                    e.remove();
                    e = p;
                }
            }
            //
            if (convertedAny) {
                //GLDisplay.getInstance().getTerrain().checkNeedUpdateVBO(false, true);
                //GLDisplay.getInstance().getRenderSelector().forceUpdateGeoBlocks();
                updateGUI(null);
            }
        }
        
        public bool getSelectedDataEqual() {
            GeoRegion region = GeoEngine.getInstance().getActiveRegion();
            if (region == null) return false;
            //
            for (GeoBlockEntry e = getHead(); (e = e.getNext()) != getTail();) {
                if (!region.dataEqualFor(e.getKey())) return false;
            }
            return true;
        }
        
        public int getSelectedDataNotEqualCount() {
            GeoRegion region = GeoEngine.getInstance().getActiveRegion();
            if (region == null) return 0;
            //
            int count = 0;
            for (GeoBlockEntry e = getHead(); (e = e.getNext()) != getTail();) {
                if (!region.dataEqualFor(e.getKey())) count++;
            }
            return count;
        }
        
        public void restoreSelectedData() {
            GeoRegion region = GeoEngine.getInstance().getActiveRegion();
            if (region == null) return;
            //
            bool restoredAny = false;
            for (GeoBlockEntry e = getHead(), p; (e = e.getNext()) != getTail();) {
                if (!region.dataEqualFor(e.getKey())) {
                    region.restoreBlock(e.getKey());
                    restoredAny = true;
                    p = e.getPrev();
                    e.remove();
                    e = p;
                }
            }
            //
            if (restoredAny) {
                //GLDisplay.getInstance().getTerrain().checkNeedUpdateVBO(true, true);
                //GLDisplay.getInstance().getRenderSelector().forceUpdateGeoBlocks();
                updateGUI(null);
            }
        }
        
        public bool hasSelected() {
            return getHead().getNext() != getTail();
        }
        
        public bool isGeoCellSelected(GeoCell cell) {
            FastArrayList<GeoCell> selected = getEntry(cell.getBlock()).getValue();
            return selected != null && selected.contains(cell);
        }
        
        public void unselectAll() {
            for (GeoBlockEntry e = getHead(), p; (e = e.getNext()) != getTail();) {
                setStateOf(e.getKey().getCells(), SelectionState.NORMAL);
                p = e.getPrev();
                e.remove();
                e = p;
            }
        }
        
        private void selectGeoCellFlat(GeoCell cell, bool append) {
            GeoBlock block = cell.getBlock();
            GeoBlockEntry entry = getEntry(block);
            FastArrayList<GeoCell> selected = entry.getValue();
            //
            if (append) {
                if (selected != null) {
                    cell.setSelectionState(SelectionState.NORMAL);
                    entry.remove();
                    return;
                }
            } else {
                unselectAll();
                selected = null;
            }
            //
            cell.setSelectionState(SelectionState.SELECTED);
            selected = new FastArrayList<GeoCell>(block.getCells());
            entry.setKey(block);
            entry.setValue(selected);
            entry.addBefore(getTail());
        }
        
        private void selectGeoCellComplex(GeoCell cell, bool fullBlock, bool append) {
            GeoBlock block = cell.getBlock();
            GeoBlockEntry entry = getEntry(block);
            GeoCell[] cells = block.getCells();
            FastArrayList<GeoCell> selected = entry.getValue();
            //
            if (append) {
                if (selected != null) {
                    if (selected.size() == cells.Length) {
                        if (fullBlock) {
                            setStateOf(cells, SelectionState.NORMAL);
                            entry.remove();
                        } else {
                            if (selected.remove(cell)) {
                                cell.setSelectionState(SelectionState.HIGHLIGHTED);
                            }
                        }
                    } else {
                        if (fullBlock) {
                            selected.clear();
                            selected.addAll(cells);
                            setStateOf(cells, SelectionState.SELECTED);
                        } else {
                            if (selected.remove(cell)) {
                                if (selected.isEmpty()) {
                                    setStateOf(cells, SelectionState.NORMAL);
                                    entry.remove();
                                } else {
                                    cell.setSelectionState(SelectionState.HIGHLIGHTED);
                                }
                            } else {
                                selected.addLast(cell);
                                cell.setSelectionState(SelectionState.SELECTED);
                            }
                        }
                    }
                } else {
                    if (fullBlock) {
                        selected = new FastArrayList<GeoCell>(cells);
                    } else {
                        selected = new FastArrayList<GeoCell>(8);
                        selected.addLastUnsafe(cell);
                        setStateOf(cells, SelectionState.HIGHLIGHTED);
                    }
                    //
                    setStateOf(selected, SelectionState.SELECTED);
                    entry.setKey(block);
                    entry.setValue(selected);
                    entry.addBefore(getTail());
                }
            } else {
                unselectAll();
                //
                if (fullBlock) {
                    selected = new FastArrayList<GeoCell>(cells);
                } else {
                    selected = new FastArrayList<GeoCell>(8);
                    selected.addLastUnsafe(cell);
                    setStateOf(cells, SelectionState.HIGHLIGHTED);
                }
                //
                setStateOf(selected, SelectionState.SELECTED);
                entry.setKey(block);
                entry.setValue(selected);
                entry.addBefore(getTail());
            }
        }
        
        private void selectGeoCellMultiLayer(GeoCell cell, bool fullBlock, bool append) {
            GeoBlock block = cell.getBlock();
            GeoBlockEntry entry = getEntry(block);
            GeoCell[] cells = block.getCells();
            GLSelectionBox selectionBox = GLDisplay.getInstance().getSelectionBox();
            FastArrayList<GeoCell> selected = entry.getValue();
            //
            if (append) {
                if (selected != null) {
                    if (fullBlock) {
                        if (selectionBox.isInfHeight()) {
                            if (selected.size() == cells.Length) {
                                setStateOf(cells, SelectionState.NORMAL);
                                entry.remove();
                            } else {
                                selected.clear();
                                selected.addAll(cells);
                                setStateOf(cells, SelectionState.SELECTED);
                            }
                        } else {
                            _temp.clear();
                            selectionBox.getAllCellsInside(cell, cells, _temp);
                            //
                            if (selected.containsAll(_temp)) {
                                if (selected.size() == _temp.size()) {
                                    setStateOf(cells, SelectionState.NORMAL);
                                    entry.remove();
                                } else {
                                    setStateOf(_temp, SelectionState.NORMAL);
                                    selected.removeAll(_temp);
                                }
                            } else {
                                setStateOf(_temp, SelectionState.SELECTED);
                                selected.addAllIfAbsent(_temp);
                            }
                        }
                    } else {
                        if (selected.remove(cell)) {
                            if (selected.isEmpty()) {
                                setStateOf(cells, SelectionState.NORMAL);
                                entry.remove();
                            } else {
                                cell.setSelectionState(SelectionState.HIGHLIGHTED);
                            }
                        } else {
                            selected.addLast(cell);
                            cell.setSelectionState(SelectionState.SELECTED);
                        }
                    }
                } else {
                    if (fullBlock) {
                        if (selectionBox.isInfHeight()) {
                            selected = new FastArrayList<GeoCell>(cells);
                        } else {
                            selected = new FastArrayList<GeoCell>();
                            selectionBox.getAllCellsInside(cell, cells, selected);
                            if (!selected.isEmpty()) {
                                setStateOf(cells, SelectionState.HIGHLIGHTED);
                            }
                        }
                    } else {
                        selected = new FastArrayList<GeoCell>(8);
                        selected.addLastUnsafe(cell);
                        setStateOf(cells, SelectionState.HIGHLIGHTED);
                    }
                    //
                    if (!selected.isEmpty()) {
                        setStateOf(selected, SelectionState.SELECTED);
                        entry.setKey(block);
                        entry.setValue(selected);
                        entry.addBefore(getTail());
                    }
                }
            } else {
                unselectAll();
                //
                if (fullBlock) {
                    if (selectionBox.isInfHeight()) {
                        selected = new FastArrayList<GeoCell>(cells);
                    } else {
                        selected = new FastArrayList<GeoCell>();
                        selectionBox.getAllCellsInside(cell, cells, selected);
                    }
                } else {
                    selected = new FastArrayList<GeoCell>(8);
                    selected.addLastUnsafe(cell);
                    setStateOf(cells, SelectionState.HIGHLIGHTED);
                }
                //
                if (!selected.isEmpty()) {
                    setStateOf(selected, SelectionState.SELECTED);
                    entry.setKey(block);
                    entry.setValue(selected);
                    entry.addBefore(getTail());
                }
            }
        }
        
        public void selectGeoCell(GeoCell cell, bool fullBlock, bool append) {
            switch (cell.getBlock().getType()) {
                case GeoEngine.GEO_BLOCK_TYPE_FLAT:
                    selectGeoCellFlat(cell, append);
                    break;
                case GeoEngine.GEO_BLOCK_TYPE_COMPLEX:
                    selectGeoCellComplex(cell, fullBlock, append);
                    break;
                case GeoEngine.GEO_BLOCK_TYPE_MULTILAYER:
                    selectGeoCellMultiLayer(cell, fullBlock, append);
                    break;
            }
            //
            updateGUI(cell);
        }
        
        public void unselectGeoCell(GeoCell cell) {
            GeoBlock block = cell.getBlock();
            GeoBlockEntry entry = getEntry(block);
            FastArrayList<GeoCell> selected = entry.getValue();
            if (selected != null && selected.remove(cell)) {
                if (selected.isEmpty()) {
                    setStateOf(block.getCells(), SelectionState.NORMAL);
                    entry.remove();
                    updateGUI(null);
                } else {
                    cell.setSelectionState(SelectionState.HIGHLIGHTED);
                    updateGUI(selected.getLastUnsafe());
                }
            }
        }
        
        private void updateGUI(GeoCell cell) {
            if (!hasSelected()) {
                FrameMain.getInstance().setSelectedGeoCell(null);
            } else {
                FastArrayList<GeoCell> selected = cell == null ? null : getEntry(cell.getBlock()).getValue();
                if (selected == null) {
                    selected = getTail().getPrev().getValue();
                }
                //
                if (cell != null && selected.contains(cell)) {
                    FrameMain.getInstance().setSelectedGeoCell(cell);
                } else {
                    FrameMain.getInstance().setSelectedGeoCell(selected.getLastUnsafe());
                }
            }
        }
        
        public void checkDeselection(int minBlockX, int maxBlockX, int minBlockY, int maxBlockY) {
            GeoCell cell = FrameMain.getInstance().getSelectedGeoCell();
            //
            GeoBlock block;
            for (GeoBlockEntry e = getHead(), p; (e = e.getNext()) != getTail();) {
                block = e.getKey();
                if (block.getBlockX() < minBlockX || block.getBlockX() >= maxBlockX || block.getBlockY() < minBlockY || block.getBlockY() >= maxBlockY) {
                    if (cell != null && cell.getBlock() == block) {
                        cell = null;
                        FrameMain.getInstance().setSelectedGeoCell(null);
                    }
                    //
                    setStateOf(block.getCells(), SelectionState.NORMAL);
                    p = e.getPrev();
                    e.remove();
                    e = p;
                }
            }
            //
            if (cell == null && hasSelected()) {
                FrameMain.getInstance().setSelectedGeoCell(getTail().getPrev().getValue().getLastUnsafe());
            }
        }
        
        public void unload() {
            for (GeoBlockEntry e = getHead(), p; (e = e.getNext()) != getTail();) {
                p = e.getPrev();
                e.remove();
                e = p;
            }
            //
            FrameMain f = FrameMain.getInstance();
            if (f != null) f.setSelectedGeoCell(null);
        }
    }
}