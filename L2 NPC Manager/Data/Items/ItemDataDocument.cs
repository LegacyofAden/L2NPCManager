using L2NPCManager.IO.Documents;
using System.Collections.ObjectModel;
using System.IO;

namespace L2NPCManager.Data.Items
{
    public class ItemDataDocument : DocumentBase
    {
        public ObservableCollection<ItemData> Items;
        public ObservableCollection<SetData> SetItems;


        public ItemDataDocument() {
            Items = new ObservableCollection<ItemData>();
            SetItems = new ObservableCollection<SetData>();
        }

        //=============================

        protected override void LoadItem(string[] tags) {
            if (matchesTag(ref tags, ItemData.TAG_START)) {
                ItemData data = new ItemData();
                readItem<ItemData>(ref data, ref tags, ItemData.TAG_START, ItemData.TAG_END);
                data.UpdateDisplayValues();
                Items.Add(data);
            }
            //
            if (matchesTag(ref tags, SetData.TAG_START)) {
                SetData data = new SetData();
                readItem<SetData>(ref data, ref tags, SetData.TAG_START, SetData.TAG_END);
                data.UpdateDisplayValues();
                SetItems.Add(data);
            }
        }

        protected override void SaveItems(StreamWriter writer) {
            long p = 0;
            long total = Items.Count + SetItems.Count;
            writeItems<ItemData>(writer, Items, ref p, total);
            writeItems<SetData>(writer, SetItems, ref p, total);
        }

        public override void Clear() {
            Items.Clear();
            SetItems.Clear();
        }

        //-----------------------------

        public ItemData FindItemByName(string name) {
            ItemData item;
            int c = Items.Count;
            for (int i = 0; i < c; i++) {
                item = Items[i];
                if (item.Name.Equals(name)) return item;
            }
            return null;
        }

        public ItemData FindItemByID(string id) {
            ItemData item;
            int c = Items.Count;
            for (int i = 0; i < c; i++) {
                item = Items[i];
                if (item.ID.Equals(id)) return item;
            }
            return null;
        }
    }
}