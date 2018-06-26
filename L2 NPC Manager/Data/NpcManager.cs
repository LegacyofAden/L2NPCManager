using L2NPCManager.Data.Npc;
using System.Collections.ObjectModel;

namespace L2NPCManager.Data
{
    public class NpcManager
    {
        public bool HasChanges {get; set;}
        public NpcDocument Document {get; set;}
        public ObservableCollection<NpcData> Items {
            get {return Document != null ? Document.Items : null;}
        }

        public ObservableCollection<string> AvailableTypes;
        public ObservableCollection<string> AvailableClans;
        public ObservableCollection<string> AvailableIgnoreClans;
        public ObservableCollection<string> AvailableRace;
        public ObservableCollection<string> AvailableBaseAttackTypes;


        public NpcManager() {
            AvailableTypes = new ObservableCollection<string>();
            AvailableClans = new ObservableCollection<string>();
            AvailableIgnoreClans = new ObservableCollection<string>();
            AvailableRace = new ObservableCollection<string>();
            AvailableBaseAttackTypes = new ObservableCollection<string>();
        }

        //=============================

        public void Load(string filename, NpcDocument.ProgressChangedEvent ev) {
            if (Document == null) Document = new NpcDocument();
            else Document.Clear();
            //
            Document.OnProgressChanged += ev;
            Document.Load(filename);
            Document.OnProgressChanged -= ev;
            HasChanges = false;
            //
            foreach (NpcData i in Document.Items) {
                addAvailableType(i.Type);
                addAvailableClan(i.GetValue(NpcData.VAR_CLAN, null));
                addAvailableIgnoreClan(i.GetValue(NpcData.VAR_IGNORE_CLAN_LIST, null));
                addAvailableRace(i.GetValue(NpcData.VAR_RACE, null));
                addAvailableBaseAttackType(i.GetValue(NpcData.VAR_BASE_ATTACK_TYPE, null));
            }
        }

        public void Save(string filename, NpcDocument.ProgressChangedEvent ev) {
            Document.OnProgressChanged += ev;
            Document.Save(filename);
            Document.OnProgressChanged -= ev;
            HasChanges = false;
        }

        public void Clear() {
            AvailableTypes.Clear();
            AvailableClans.Clear();
            AvailableIgnoreClans.Clear();
            AvailableRace.Clear();
            AvailableBaseAttackTypes.Clear();
        }

        public NpcData FindItemByName(string name) {
            NpcData item;
            int c = Items.Count;
            for (int i = 0; i < c; i++) {
                item = Items[i];
                if (item.Name.Equals(name)) return item;
            }
            return null;
        }

        public NpcData FindItemByID(string id) {
            NpcData item;
            int c = Items.Count;
            for (int i = 0; i < c; i++) {
                item = Items[i];
                if (item.ID.Equals(id)) return item;
            }
            return null;
        }

        //-----------------------------

        public void addAvailableType(string value) {
            if (value != null && !AvailableTypes.Contains(value)) AvailableTypes.Add(value);
        }

        public void addAvailableClan(string value) {
            if (value != null && !AvailableClans.Contains(value)) AvailableClans.Add(value);
        }

        public void addAvailableIgnoreClan(string value) {
            if (value != null && !AvailableIgnoreClans.Contains(value)) AvailableIgnoreClans.Add(value);
        }

        public void addAvailableRace(string value) {
            if (value != null && !AvailableRace.Contains(value)) AvailableRace.Add(value);
        }

        public void addAvailableBaseAttackType(string value) {
            if (value != null && !AvailableBaseAttackTypes.Contains(value)) AvailableBaseAttackTypes.Add(value);
        }
    }
}