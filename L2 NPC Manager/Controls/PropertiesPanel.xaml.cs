using L2NPCManager.AI.Scripts;
using L2NPCManager.Data;
using L2NPCManager.Data.Npc;
using L2NPCManager.IO;
using L2NPCManager.Windows;
using System.Windows;
using System.Windows.Controls;

namespace L2NPCManager.Controls
{
    public partial class PropertiesPanel : UserControl
    {
        public delegate void ChangedEvent();
        public event ChangedEvent OnChanged;

        public NpcManager NpcMgr {get; set;}
        public ScriptManager ScriptMgr {get; set;}

        private bool isLoading;
        private bool hasChanges;
        private ScriptProperty ai_property;
        private NpcData src_npc;


        public PropertiesPanel() {
            InitializeComponent();
        }

        //=============================

        public void Load() {
            isLoading = true;
            if (NpcMgr != null) {
                lstTypes.ItemsSource = NpcMgr.AvailableTypes;
                lstClan.ItemsSource = NpcMgr.AvailableClans;
                lstIgnoreClanList.ItemsSource = NpcMgr.AvailableIgnoreClans;
                lstRace.ItemsSource = NpcMgr.AvailableRace;
                lstBaseAttackType.ItemsSource = NpcMgr.AvailableBaseAttackTypes;
            } else {
                lstTypes.ItemsSource = null;
                lstClan.ItemsSource = null;
                lstIgnoreClanList.ItemsSource = null;
                lstRace.ItemsSource = null;
                lstBaseAttackType.ItemsSource = null;
            }
            isLoading = false;
        }

        public void LoadItem(NpcData item) {
            src_npc = item;
            //
            isLoading = true;
            lstTypes.Text = item.Type;
            txtLevel.Text = item.GetValue(NpcData.VAR_LEVEL, "0");
            txtAI.Text = getAI(item.GetValue(NpcData.VAR_NPC_AI, null));
            txtAcquireExpRate.Text = item.GetValue(NpcData.VAR_ACQUIRE_EXP_RATE, null);
            txtAcquireSp.Text = item.GetValue(NpcData.VAR_ACQUIRE_SP, null);
            chkUnsowing.IsChecked = item.GetValue(NpcData.VAR_UNSOWING, false);
            lstClan.Text = item.GetValue(NpcData.VAR_CLAN, null);
            lstIgnoreClanList.Text = item.GetValue(NpcData.VAR_IGNORE_CLAN_LIST, null);
            txtClanHelpRange.Text = item.GetValue(NpcData.VAR_CLAN_HELP_RANGE, null);
            txtSlotChest.Text = StringUtils.getString(item.GetValue(NpcData.VAR_SLOT_CHEST, null));
            txtSlotRHand.Text = StringUtils.getString(item.GetValue(NpcData.VAR_SLOT_RHAND, null));
            txtSlotLHand.Text = StringUtils.getString(item.GetValue(NpcData.VAR_SLOT_LHAND, null));
            txtShieldDefenseRate.Text = item.GetValue(NpcData.VAR_SHIELD_DEFENSE_RATE, null);
            txtShieldDefense.Text = item.GetValue(NpcData.VAR_SHIELD_DEFENSE, null);
            lstRace.Text = item.GetValue(NpcData.VAR_RACE, null);
            lstSex.SelectedIndex = getSex(item.GetValue(NpcData.VAR_SEX, null));
            chkUndying.IsChecked = item.GetValue(NpcData.VAR_UNDYING, false);
            chkCanBeAttacked.IsChecked = item.GetValue(NpcData.VAR_CAN_BE_ATTACKED, false);
            txtCorpseTime.Text = item.GetValue(NpcData.VAR_CORPSE_TIME, null);
            chkNoSleepMode.IsChecked = item.GetValue(NpcData.VAR_NO_SLEEP_MODE, false);
            txtAgroRange.Text = item.GetValue(NpcData.VAR_AGRO_RANGE, null);
            txtGroundHigh.Text = getFirstTri(item.GetValue(NpcData.VAR_GROUND_HIGH, null));
            txtGroundLow.Text = getFirstTri(item.GetValue(NpcData.VAR_GROUND_LOW, null));
            txtExp.Text = item.GetValue(NpcData.VAR_EXP, null);
            txtOrgHp.Text = item.GetValue(NpcData.VAR_ORG_HP, null);
            txtOrgHpRegen.Text = item.GetValue(NpcData.VAR_ORG_HP_REGEN, null);
            txtOrgMp.Text = item.GetValue(NpcData.VAR_ORG_MP, null);
            txtOrgMpRegen.Text = item.GetValue(NpcData.VAR_ORG_MP_REGEN, null);
            txtCollisionRadius.Text = getDual(item.GetValue(NpcData.VAR_COLLISION_RADIUS, null));
            txtCollisionHeight.Text = getDual(item.GetValue(NpcData.VAR_COLLISION_HEIGHT, null));
            txtStr.Text = item.GetValue(NpcData.VAR_STR, null);
            txtInt.Text = item.GetValue(NpcData.VAR_INT, null);
            txtDex.Text = item.GetValue(NpcData.VAR_DEX, null);
            txtWit.Text = item.GetValue(NpcData.VAR_WIT, null);
            txtCon.Text = item.GetValue(NpcData.VAR_CON, null);
            txtMen.Text = item.GetValue(NpcData.VAR_MEN, null);
            lstBaseAttackType.Text = item.GetValue(NpcData.VAR_BASE_ATTACK_TYPE, null);
            txtBaseAttackRange.Text = item.GetValue(NpcData.VAR_BASE_ATTACK_RANGE, null);
            //
            string[] baseDamageRange = getDamageRange(item.GetValue(NpcData.VAR_BASE_DAMAGE_RANGE, null));
            txtBaseDamageRange_angle.Text = (baseDamageRange.Length > 2 ? baseDamageRange[2] : null);
            txtBaseDamageRange_distance.Text = (baseDamageRange.Length > 3 ? baseDamageRange[3] : null);
            //
            txtBaseRandDam.Text = item.GetValue(NpcData.VAR_BASE_RAND_DAM, null);
            txtBasePhysicalAttack.Text = item.GetValue(NpcData.VAR_BASE_PHYSICAL_ATTACK, null);
            txtBaseCritical.Text = item.GetValue(NpcData.VAR_BASE_CRITICAL, null);
            txtPhysicalHitModify.Text = item.GetValue(NpcData.VAR_PHYSICAL_HIT_MODIFY, null);
            txtBaseAttackSpeed.Text = item.GetValue(NpcData.VAR_BASE_ATTACK_SPEED, null);
            txtBaseReuseDelay.Text = item.GetValue(NpcData.VAR_BASE_REUSE_DELAY, null);
            txtBaseMagicAttack.Text = item.GetValue(NpcData.VAR_BASE_MAGIC_ATTACK, null);
            txtBaseDefend.Text = item.GetValue(NpcData.VAR_BASE_DEFEND, null);
            txtBaseMagicDefend.Text = item.GetValue(NpcData.VAR_BASE_MAGIC_DEFEND, null);
            txtPhysicalAvoidModify.Text = item.GetValue(NpcData.VAR_PHYSICAL_AVOID_MODIFY, null);
            txtSoulshotCount.Text = item.GetValue(NpcData.VAR_SOULSHOT_COUNT, null);
            txtSpiritShotCount.Text = item.GetValue(NpcData.VAR_SPIRITSHOT_COUNT, null);
            txtHitTimeFactor.Text = item.GetValue(NpcData.VAR_HIT_TIME_FACTOR, null);
            txtSafeHeight.Text = item.GetValue(NpcData.VAR_SAFE_HEIGHT, null);
            isLoading = false;
            hasChanges = false;
        }

        public void SaveItem(NpcData item) {
            item.Type = (string)lstTypes.Text;
            item.SetValue(NpcData.VAR_LEVEL, txtLevel.Text);
            item.SetValue(NpcData.VAR_NPC_AI, setAI(txtAI.Text));
            item.SetValue(NpcData.VAR_ACQUIRE_EXP_RATE, txtAcquireExpRate.Text);
            item.SetValue(NpcData.VAR_ACQUIRE_SP, txtAcquireSp.Text);
            item.SetValue(NpcData.VAR_UNSOWING, chkUnsowing.IsChecked);
            item.SetValue(NpcData.VAR_CLAN, lstClan.Text);
            item.SetValue(NpcData.VAR_IGNORE_CLAN_LIST, lstIgnoreClanList.Text);
            item.SetValue(NpcData.VAR_CLAN_HELP_RANGE, txtClanHelpRange.Text);
            item.SetValue(NpcData.VAR_SLOT_CHEST, StringUtils.setString(txtSlotChest.Text));
            item.SetValue(NpcData.VAR_SLOT_RHAND, StringUtils.setString(txtSlotRHand.Text));
            item.SetValue(NpcData.VAR_SLOT_LHAND, StringUtils.setString(txtSlotLHand.Text));
            item.SetValue(NpcData.VAR_SHIELD_DEFENSE_RATE, txtShieldDefenseRate.Text);
            item.SetValue(NpcData.VAR_SHIELD_DEFENSE, txtShieldDefense.Text);
            item.SetValue(NpcData.VAR_RACE, lstRace.Text);
            item.SetValue(NpcData.VAR_SEX, setSex(lstSex.SelectedIndex));
            item.SetValue(NpcData.VAR_UNDYING, chkUndying.IsChecked);
            item.SetValue(NpcData.VAR_CAN_BE_ATTACKED, chkCanBeAttacked.IsChecked);
            item.SetValue(NpcData.VAR_CORPSE_TIME, txtCorpseTime.Text);
            item.SetValue(NpcData.VAR_NO_SLEEP_MODE, chkNoSleepMode.IsChecked);
            item.SetValue(NpcData.VAR_AGRO_RANGE, txtAgroRange.Text);
            item.SetValue(NpcData.VAR_GROUND_HIGH, setFirstTri(txtGroundHigh.Text));
            item.SetValue(NpcData.VAR_GROUND_LOW, setFirstTri(txtGroundLow.Text));
            item.SetValue(NpcData.VAR_EXP, txtExp.Text);
            item.SetValue(NpcData.VAR_ORG_HP, txtOrgHp.Text);
            item.SetValue(NpcData.VAR_ORG_HP_REGEN, txtOrgHpRegen.Text);
            item.SetValue(NpcData.VAR_ORG_MP, txtOrgMp.Text);
            item.SetValue(NpcData.VAR_ORG_MP_REGEN, txtOrgMpRegen.Text);
            item.SetValue(NpcData.VAR_COLLISION_RADIUS, setDual(txtCollisionRadius.Text));
            item.SetValue(NpcData.VAR_COLLISION_HEIGHT, setDual(txtCollisionHeight.Text));
            item.SetValue(NpcData.VAR_STR, txtStr.Text);
            item.SetValue(NpcData.VAR_INT, txtInt.Text);
            item.SetValue(NpcData.VAR_DEX, txtDex.Text);
            item.SetValue(NpcData.VAR_WIT, txtWit.Text);
            item.SetValue(NpcData.VAR_CON, txtCon.Text);
            item.SetValue(NpcData.VAR_MEN, txtMen.Text);
            item.SetValue(NpcData.VAR_BASE_ATTACK_TYPE, lstBaseAttackType.Text);
            item.SetValue(NpcData.VAR_BASE_ATTACK_RANGE, txtBaseAttackRange.Text);
            //
            string[] baseDamageRange = new string[] {"0", "0", txtBaseDamageRange_angle.Text, txtBaseDamageRange_distance.Text};
            item.SetValue(NpcData.VAR_BASE_DAMAGE_RANGE, setDamageRange(baseDamageRange));
            //
            item.SetValue(NpcData.VAR_BASE_RAND_DAM, txtBaseRandDam.Text);
            item.SetValue(NpcData.VAR_BASE_PHYSICAL_ATTACK, txtBasePhysicalAttack.Text);
            item.SetValue(NpcData.VAR_BASE_CRITICAL, txtBaseCritical.Text);
            item.SetValue(NpcData.VAR_PHYSICAL_HIT_MODIFY, txtPhysicalHitModify.Text);
            item.SetValue(NpcData.VAR_BASE_ATTACK_SPEED, txtBaseAttackSpeed.Text);
            item.SetValue(NpcData.VAR_BASE_REUSE_DELAY, txtBaseReuseDelay.Text);
            item.SetValue(NpcData.VAR_BASE_MAGIC_ATTACK, txtBaseMagicAttack.Text);
            item.SetValue(NpcData.VAR_BASE_DEFEND, txtBaseDefend.Text);
            item.SetValue(NpcData.VAR_BASE_MAGIC_DEFEND, txtBaseMagicDefend.Text);
            item.SetValue(NpcData.VAR_PHYSICAL_AVOID_MODIFY, txtPhysicalAvoidModify.Text);
            item.SetValue(NpcData.VAR_SOULSHOT_COUNT, txtSoulshotCount.Text);
            item.SetValue(NpcData.VAR_SPIRITSHOT_COUNT, txtSpiritShotCount.Text);
            item.SetValue(NpcData.VAR_HIT_TIME_FACTOR, txtHitTimeFactor.Text);
            item.SetValue(NpcData.VAR_SAFE_HEIGHT, txtSafeHeight.Text);
        }

        public void Clear() {
            isLoading = true;
            lstTypes.Text = null;
            txtLevel.Clear();
            txtAI.Text = null;
            txtAcquireExpRate.Clear();
            txtAcquireSp.Clear();
            chkUnsowing.IsChecked = false;
            lstClan.Text = null;
            lstIgnoreClanList.Text = null;
            txtClanHelpRange.Clear();
            txtSlotChest.Clear();
            txtSlotRHand.Clear();
            txtSlotLHand.Clear();
            txtShieldDefenseRate.Clear();
            txtShieldDefense.Clear();
            lstRace.Text = null;
            lstSex.SelectedValue = null;
            chkUndying.IsChecked = false;
            chkCanBeAttacked.IsChecked = false;
            txtCorpseTime.Clear();
            chkNoSleepMode.IsChecked = false;
            txtAgroRange.Clear();
            txtGroundHigh.Clear();
            txtGroundLow.Clear();
            txtExp.Clear();
            txtOrgHp.Clear();
            txtOrgHpRegen.Clear();
            txtOrgMp.Clear();
            txtOrgMpRegen.Clear();
            txtCollisionRadius.Clear();
            txtCollisionHeight.Clear();
            txtStr.Clear();
            txtInt.Clear();
            txtDex.Clear();
            txtWit.Clear();
            txtCon.Clear();
            txtMen.Clear();
            lstBaseAttackType.Text = null;
            txtBaseAttackRange.Clear();
            txtBaseDamageRange_angle.Clear();
            txtBaseDamageRange_distance.Clear();
            txtBaseRandDam.Clear();
            txtBasePhysicalAttack.Clear();
            txtBaseCritical.Clear();
            txtPhysicalHitModify.Clear();
            txtBaseAttackSpeed.Clear();
            txtBaseReuseDelay.Clear();
            txtBaseMagicAttack.Clear();
            txtBaseDefend.Clear();
            txtBaseMagicDefend.Clear();
            txtPhysicalAvoidModify.Clear();
            txtSoulshotCount.Clear();
            txtSpiritShotCount.Clear();
            txtHitTimeFactor.Clear();
            txtSafeHeight.Clear();
            isLoading = false;
        }

        public void setContentEnabled(bool enabled) {
            content.IsEnabled = enabled;
        }

        //-----------------------------

        private void txt_TextChanged(object sender, TextChangedEventArgs e) {
            if (!isLoading) setHasChanges(true);
        }

        private void chk_Checked(object sender, RoutedEventArgs e) {
            if (!isLoading) setHasChanges(true);
        }

        private void lst_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (!isLoading) setHasChanges(true);
        }

        private void btnNewScript_Click(object sender, RoutedEventArgs e) {
            Window parentWindow = Window.GetWindow(this);
            //
            NewScriptWindow window = new NewScriptWindow();
            window.Owner = parentWindow;
            window.ScriptMgr = ScriptMgr;
            bool? r = window.ShowDialog();
            if (r.HasValue && r.Value) {
                string new_script = window.NewScript.Name;
                txtAI.Text = new_script;
                //src_npc.SetValue(NpcData.VAR_NPC_AI, setAI(new_script));
                //invokeScriptChanged(src_npc);
            }
        }

        private void btnClans_Click(object sender, RoutedEventArgs e) {
            Window parentWindow = Window.GetWindow(this);
            //
            ClansWindow window = new ClansWindow();
            window.Owner = parentWindow;
            window.NpcMgr = NpcMgr;
            window.Data = lstClan.Text;
            window.ShowDialog();
            if (window.HasChanges) {
                lstClan.Text = window.Data;
            }
        }

        private void btnIgnoreClans_Click(object sender, RoutedEventArgs e) {
            Window parentWindow = Window.GetWindow(this);
            //
            IgnoreClansWindow window = new IgnoreClansWindow();
            window.Owner = parentWindow;
            window.NpcMgr = NpcMgr;
            window.Data = lstIgnoreClanList.Text;
            window.ShowDialog();
            if (window.HasChanges) {
                lstIgnoreClanList.Text = window.Data;
            }
        }

        //-----------------------------
        // Helpers

        private void setHasChanges(bool value) {
            if (value && !hasChanges) {
                if (OnChanged != null) OnChanged.Invoke();
                hasChanges = true;
            }
        }

        //private void invokeScriptChanged(NpcData data) {
        //    if (OnScriptChanged != null) OnScriptChanged.Invoke(data);
        //}

        private string getDual(string value) {
            if (value == null) return null;
            string val = StringUtils.Trim(value, "{", "}");
            int i = val.IndexOf(';');
            if (i < 0) return val;
            return val.Substring(0, i);
        }

        private string setDual(string value) {
            return StringUtils.Pad(value+';'+value, "{", "}");
        }

        private string getFirstTri(string value) {
            if (value == null) return null;
            string val = StringUtils.Trim(value, "{", "}");
            int i = val.IndexOf(';');
            if (i < 0) return val;
            return val.Substring(0, i);
        }

        private string setFirstTri(string value) {
            return StringUtils.Pad(value+";0;0", "{", "}");
        }

        private string[] getDamageRange(string value) {
            if (value == null) return null;
            return StringUtils.Trim(value, "{", "}").Split(';');
        }
        private string getDamageRange_distance(string value) {
            if (value == null) return null;
            string[] tags = StringUtils.Trim(value, "{", "}").Split(';');
            return tags[3];
        }

        private string setDamageRange(string[] tags) {
            string value = string.Join(";", tags);
            return StringUtils.Pad(value, "{", "}");
        }

        private int getSex(string value) {
            if (value == "male") return 0;
            if (value == "female") return 1;
            if (value == "etc") return 2;
            return -1;
        }

        private string setSex(int value) {
            if (value == 0) return "male";
            if (value == 1) return "female";
            if (value == 2) return "etc";
            return null;
        }

        private string getAI(string value) {
            ai_property = ScriptProperty.FromString(value);
            return ai_property.ScriptName;
        }

        private string setAI(string value) {
            ai_property.ScriptName = value;
            return ai_property.ToString();
        }
    }
}