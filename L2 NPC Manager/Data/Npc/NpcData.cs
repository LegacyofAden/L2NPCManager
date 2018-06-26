using L2NPCManager.IO.Documents;
using System.Collections.Generic;
using System.IO;

namespace L2NPCManager.Data.Npc
{
    public class NpcData : ItemBase
    {
        public const string TAG_START = "npc_begin";
        public const string TAG_END = "npc_end";

        public const string VAR_LEVEL = "level";
        public const string VAR_ACQUIRE_EXP_RATE = "acquire_exp_rate";
        public const string VAR_ACQUIRE_SP = "acquire_sp";
        public const string VAR_UNSOWING = "unsowing";
        public const string VAR_CLAN = "clan";
        public const string VAR_IGNORE_CLAN_LIST = "ignore_clan_list";
        public const string VAR_CLAN_HELP_RANGE = "clan_help_range";
        public const string VAR_SLOT_CHEST = "slot_chest";
        public const string VAR_SLOT_RHAND = "slot_rhand";
        public const string VAR_SLOT_LHAND = "slot_lhand";
        public const string VAR_SHIELD_DEFENSE_RATE = "shield_defense_rate";
        public const string VAR_SHIELD_DEFENSE = "shield_defense";
        public const string VAR_SKILL_LIST = "skill_list";
        public const string VAR_NPC_AI = "npc_ai";
        public const string VAR_CATEGORY = "category";
        public const string VAR_RACE = "race";
        public const string VAR_SEX = "sex";
        public const string VAR_UNDYING = "undying";
        public const string VAR_CAN_BE_ATTACKED = "can_be_attacked";
        public const string VAR_CORPSE_TIME = "corpse_time";
        public const string VAR_NO_SLEEP_MODE = "no_sleep_mode";
        public const string VAR_AGRO_RANGE = "agro_range";
        public const string VAR_GROUND_HIGH = "ground_high";
        public const string VAR_GROUND_LOW = "ground_low";
        public const string VAR_EXP = "exp";
        public const string VAR_ORG_HP = "org_hp";
        public const string VAR_ORG_HP_REGEN = "org_hp_regen";
        public const string VAR_ORG_MP = "org_mp";
        public const string VAR_ORG_MP_REGEN = "org_mp_regen";
        public const string VAR_COLLISION_RADIUS = "collision_radius";
        public const string VAR_COLLISION_HEIGHT = "collision_height";
        public const string VAR_STR = "str";
        public const string VAR_INT = "int";
        public const string VAR_DEX = "dex";
        public const string VAR_WIT = "wit";
        public const string VAR_CON = "con";
        public const string VAR_MEN = "men";
        public const string VAR_BASE_ATTACK_TYPE = "base_attack_type";
        public const string VAR_BASE_ATTACK_RANGE = "base_attack_range";
        public const string VAR_BASE_DAMAGE_RANGE = "base_damage_range";
        public const string VAR_BASE_RAND_DAM = "base_rand_dam";
        public const string VAR_BASE_PHYSICAL_ATTACK = "base_physical_attack";
        public const string VAR_BASE_CRITICAL = "base_critical";
        public const string VAR_PHYSICAL_HIT_MODIFY = "physical_hit_modify";
        public const string VAR_BASE_ATTACK_SPEED = "base_attack_speed";
        public const string VAR_BASE_REUSE_DELAY = "base_reuse_delay";
        public const string VAR_BASE_MAGIC_ATTACK = "base_magic_attack";
        public const string VAR_BASE_DEFEND = "base_defend";
        public const string VAR_BASE_MAGIC_DEFEND = "base_magic_defend";
        public const string VAR_PHYSICAL_AVOID_MODIFY = "physical_avoid_modify";
        public const string VAR_SOULSHOT_COUNT = "soulshot_count";
        public const string VAR_SPIRITSHOT_COUNT = "spiritshot_count";
        public const string VAR_HIT_TIME_FACTOR = "hit_time_factor";
        public const string VAR_ITEM_MAKE_LIST = "item_make_list";
        public const string VAR_CORPSE_MAKE_LIST = "corpse_make_list";
        public const string VAR_ADDITIONAL_MAKE_LIST = "additional_make_list";
        public const string VAR_ADDITIONAL_MAKE_MULTI_LIST = "additional_make_multi_list";
        public const string VAR_HP_INCREASE = "hp_increase";
        public const string VAR_MP_INCREASE = "mp_increase";
        public const string VAR_SAFE_HEIGHT = "safe_height";

        //=============================

        public override void ReadData(int index, string value) {
            if (index == 1) {Type = value; return;}
            if (index == 2) {ID = value; return;}
            if (index == 3) {Name = value; return;}
            AddValue(new DocumentValue(value));
        }

        public override void WriteData(StreamWriter writer) {
            writer.Write(TAG_START);
            writer.Write('\t');
            writer.Write(Type);
            writer.Write('\t');
            writer.Write(ID);
            writer.Write('\t');
            writer.Write(Name);
            WriteValues(writer);
            writer.Write('\t');
            writer.Write(TAG_END);
        }

        //=============================

        public List<NpcDropGroup> GetDropGroups() {
            string data = GetValue(VAR_ADDITIONAL_MAKE_MULTI_LIST, null);
            if (data != null) return NpcDropGroup.ParseList(data);
            return null;
        }

        public void SetDropGroups(List<NpcDropGroup> items) {
            string data = NpcDropGroup.WriteList(items);
            SetValue(VAR_ADDITIONAL_MAKE_MULTI_LIST, data);
        }

        public NpcSpoilGroup GetSpoilItems() {
            string data = GetValue(VAR_CORPSE_MAKE_LIST, null);
            if (data != null) return NpcSpoilGroup.ParseList(data);
            return new NpcSpoilGroup();
        }

        public void SetSpoilItems(NpcSpoilGroup items) {
            string data = NpcSpoilGroup.WriteList(items);
            SetValue(VAR_CORPSE_MAKE_LIST, data);
        }

        public bool AdjustRates(float exp, float sp, ref int exp_count, ref int sp_count) {
            bool has_changes = false;
            //
            if (exp != 1f) {
                try {
                    float val = float.Parse(GetValue(NpcData.VAR_ACQUIRE_EXP_RATE, "0"));
                    SetValue(NpcData.VAR_ACQUIRE_EXP_RATE, (val * exp).ToString());
                    has_changes = true;
                    exp_count++;
                }
                catch {}
            }
            //
            if (sp != 1f) {
                try {
                    float val = float.Parse(GetValue(NpcData.VAR_ACQUIRE_SP, "0"));
                    SetValue(NpcData.VAR_ACQUIRE_SP, (val * sp).ToString());
                    has_changes = true;
                    sp_count++;
                }
                catch {}
            }
            //
            return has_changes;
        }
    }
}