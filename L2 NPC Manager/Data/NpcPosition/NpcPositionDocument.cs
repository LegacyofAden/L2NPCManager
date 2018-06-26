using L2NPCManager.IO.Documents;
using System.Collections.ObjectModel;
using System.IO;

namespace L2NPCManager.Data.NpcPosition
{
    public class NpcPositionDocument : DocumentBase
    {
        public ObservableCollection<DomainData> Domains;
        public ObservableCollection<TerritoryData> Territories;
        public ObservableCollection<NpcMakerData> NpcMakers;
        public ObservableCollection<NpcMakerExData> NpcMakersEx;
        public ObservableCollection<NpcPositionData> Positions;
        public ObservableCollection<NpcPositionExData> PositionsEx;


        public NpcPositionDocument() {
            Domains = new ObservableCollection<DomainData>();
            Territories = new ObservableCollection<TerritoryData>();
            NpcMakers = new ObservableCollection<NpcMakerData>();
            NpcMakersEx = new ObservableCollection<NpcMakerExData>();
            Positions = new ObservableCollection<NpcPositionData>();
            PositionsEx = new ObservableCollection<NpcPositionExData>();
        }

        //=============================

        protected override void LoadItem(string[] tags) {
            if (matchesTag(ref tags, DomainData.TAG_START)) {
                DomainData data = new DomainData();
                readItem<DomainData>(ref data, ref tags, DomainData.TAG_START, DomainData.TAG_END);
                data.UpdateDisplayValues();
                Domains.Add(data);
            }
            //
            if (matchesTag(ref tags, TerritoryData.TAG_START)) {
                TerritoryData data = new TerritoryData();
                readItem<TerritoryData>(ref data, ref tags, TerritoryData.TAG_START, TerritoryData.TAG_END);
                data.UpdateDisplayValues();
                Territories.Add(data);
            }
            //
            if (matchesTag(ref tags, NpcMakerData.TAG_START)) {
                NpcMakerData data = new NpcMakerData();
                readItem<NpcMakerData>(ref data, ref tags, NpcMakerData.TAG_START, NpcMakerData.TAG_END);
                data.UpdateDisplayValues();
                NpcMakers.Add(data);
            }
            //
            if (matchesTag(ref tags, NpcMakerExData.TAG_START)) {
                NpcMakerExData data = new NpcMakerExData();
                readItem<NpcMakerExData>(ref data, ref tags, NpcMakerExData.TAG_START, NpcMakerExData.TAG_END);
                data.UpdateDisplayValues();
                NpcMakersEx.Add(data);
            }
            //
            if (matchesTag(ref tags, NpcPositionData.TAG_START)) {
                NpcPositionData data = new NpcPositionData();
                readItem<NpcPositionData>(ref data, ref tags, NpcPositionData.TAG_START, NpcPositionData.TAG_END);
                data.UpdateDisplayValues();
                Positions.Add(data);
            }
            //
            if (matchesTag(ref tags, NpcPositionExData.TAG_START)) {
                NpcPositionExData data = new NpcPositionExData();
                readItem<NpcPositionExData>(ref data, ref tags, NpcPositionExData.TAG_START, NpcPositionExData.TAG_END);
                data.UpdateDisplayValues();
                PositionsEx.Add(data);
            }
        }

        protected override void SaveItems(StreamWriter writer) {
            long p = 0;
            long total = Domains.Count + Territories.Count + NpcMakers.Count
              + NpcMakersEx.Count + Positions.Count + PositionsEx.Count;
            writeItems<DomainData>(writer, Domains, ref p, total);
            writeItems<TerritoryData>(writer, Territories, ref p, total);
            writeItems<NpcMakerData>(writer, NpcMakers, ref p, total);
            writeItems<NpcMakerExData>(writer, NpcMakersEx, ref p, total);
            writeItems<NpcPositionData>(writer, Positions, ref p, total);
            writeItems<NpcPositionExData>(writer, PositionsEx, ref p, total);
        }

        public override void Clear() {
            Domains.Clear();
            Territories.Clear();
            NpcMakers.Clear();
            NpcMakersEx.Clear();
            Positions.Clear();
            PositionsEx.Clear();
        }
    }
}