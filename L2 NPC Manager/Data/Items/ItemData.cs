using L2NPCManager.IO.Documents;
using System.IO;

namespace L2NPCManager.Data.Items
{
    public class ItemData : ItemBase
    {
        public const string TAG_START = "item_begin";
        public const string TAG_END = "item_end";

        public const string VAR_ITEM_TYPE = "item_type";
        public const string VAR_SLOT_BIT_TYPE = "slot_bit_type";
        public const string VAR_ARMOR_TYPE = "armor_type";
        public const string VAR_ETCITEM_TYPE = "etcitem_type";
        public const string VAR_RECIPE_ID = "recipe_id";
        public const string VAR_BLESSED = "blessed";
        public const string VAR_WEIGHT = "weight";
        public const string VAR_DEFAULT_ACTION = "default_action";
        public const string VAR_CONSUME_TYPE = "consume_type";
        public const string VAR_INITIAL_COUNT = "initial_count";
        public const string VAR_MAXIMUM_COUNT = "maximum_count";
        public const string VAR_SOULSHOT_COUNT = "soulshot_count";
        public const string VAR_SPIRITSHOT_COUNT = "spiritshot_count";
        public const string VAR_REDUCED_SOULSHOT = "reduced_soulshot";
        public const string VAR_REDUCED_SPIRITSHOT = "reduced_spiritshot";
        public const string VAR_REDUCED_MP_CONSUME = "reduced_mp_consume";
        public const string VAR_IMMEDIATE_EFFECT = "immediate_effect";
        public const string VAR_PRICE = "price";
        public const string VAR_DEFAULT_PRICE = "default_price";
        public const string VAR_ITEM_SKILL = "item_skill";
        public const string VAR_CRITICAL_ATTACK_SKILL = "critical_attack_skill";
        public const string VAR_ATTACK_SKILL = "attack_skill";
        public const string VAR_MAGIC_SKILL = "magic_skill";
        public const string VAR_ITEM_SKILL_ENCHANTED_FOUR = "item_skill_enchanted_four";
        public const string VAR_MATERIAL_TYPE = "material_type";
        public const string VAR_CRYSTAL_TYPE = "crystal_type";
        public const string VAR_CRYSTAL_COUNT = "crystal_count";
        public const string VAR_IS_TRADE = "is_trade";
        public const string VAR_IS_DROP = "is_drop";
        public const string VAR_IS_DESTRUCT = "is_destruct";
        public const string VAR_PHYSICAL_DAMAGE = "physical_damage";
        public const string VAR_RANDOM_DAMAGE = "random_damage";
        public const string VAR_WEAPON_TYPE = "weapon_type";
        public const string VAR_CAN_PENETRATE = "can_penetrate";
        public const string VAR_CRITICAL = "critical";
        public const string VAR_HIT_MODIFY = "hit_modify";
        public const string VAR_AVOID_MODIFY = "avoid_modify";
        public const string VAR_DUAL_FHIT_RATE = "dual_fhit_rate";
        public const string VAR_SHIELD_DEFENSE = "shield_defense";
        public const string VAR_SHIELD_DEFENSE_RATE = "shield_defense_rate";
        public const string VAR_ATTACK_RANGE = "attack_range";
        public const string VAR_DAMAGE_RANGE = "damage_range";
        public const string VAR_ATTACK_SPEED = "attack_speed";
        public const string VAR_REUSE_DELAY = "reuse_delay";
        public const string VAR_MP_CONSUME = "mp_consume";
        public const string VAR_MAGICAL_DAMAGE = "magical_damage";
        public const string VAR_DURABILITY = "durability";
        public const string VAR_DAMAGED = "damaged";
        public const string VAR_PHYSICAL_DEFENSE = "physical_defense";
        public const string VAR_MAGICAL_DEFENSE = "magical_defense";
        public const string VAR_MP_BONUS = "mp_bonus";
        public const string VAR_CATEGORY = "category";
        public const string VAR_ENCHANTED = "enchanted";
        public const string VAR_HTML = "html";
        public const string VAR_EQUIP_PET = "equip_pet";
        public const string VAR_MAGIC_WEAPON = "magic_weapon";
        public const string VAR_ENCHANT_ENABLE = "enchant_enable";
        public const string VAR_CAN_EQUIP_SEX = "can_equip_sex";
        public const string VAR_CAN_EQUIP_RACE = "can_equip_race";
        public const string VAR_CAN_EQUIP_CHANGE_CLASS = "can_equip_change_class";
        public const string VAR_CAN_EQUIP_CLASS = "can_equip_class";
        public const string VAR_CAN_EQUIP_AGIT = "can_equip_agit";
        public const string VAR_CAN_EQUIP_CASTLE = "can_equip_castle";
        public const string VAR_CAN_EQUIP_CASTLE_NUM = "can_equip_castle_num";
        public const string VAR_CAN_EQUIP_CLAN_LEADER = "can_equip_clan_leader";
        public const string VAR_CAN_EQUIP_CLAN_LEVEL = "can_equip_clan_level";
        public const string VAR_CAN_EQUIP_HERO = "can_equip_hero";
        public const string VAR_CAN_EQUIP_NOBLESS = "can_equip_nobless";
        public const string VAR_CAN_EQUIP_CHAOTIC = "can_equip_chaotic";

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
    }
}