using TaleWorlds.Core;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;

namespace DragonBannerAlmighty
{
    public sealed class Settings : AttributeGlobalSettings<Settings>
    {
        public override string Id => "DragonBannerAlmighty_v1";
        public override string DisplayName => "Dragon Banner Almighty";
        public override string FolderName => "DragonBannerAlmighty";
        public override string FormatType => "json2";

        // ---- General ----------------------------------------------------------
        private const string GGeneral = "General";

        [SettingPropertyBool("Enable Mod (Master Switch)", Order = 0, RequireRestart = false,
            HintText = "Master on/off for all Dragon Banner effects. Default: on.")]
        [SettingPropertyGroup(GGeneral, GroupOrder = 0)]
        public bool MasterEnabled { get; set; } = true;

        // Per-effect blocks. Each: Enabled + Tier (1-3) + Multiplier (0.1-10).
        // Tier picks which vanilla banner-tier value the effect uses; Multiplier scales it.

        // 1) Increased Melee Damage
        private const string G1 = "Melee Damage";
        [SettingPropertyBool("Enabled", Order = 0, RequireRestart = false, HintText = "Your troops deal more melee damage.")]
        [SettingPropertyGroup(G1, GroupOrder = 1)]
        public bool MeleeDamageEnabled { get; set; } = true;
        [SettingPropertyInteger("Tier (1-3)", 1, 3, Order = 1, RequireRestart = false, HintText = "3 = strongest vanilla value.")]
        [SettingPropertyGroup(G1, GroupOrder = 1)]
        public int MeleeDamageTier { get; set; } = 3;
        [SettingPropertyFloatingInteger("Multiplier", 0.1f, 10f, "0.00", Order = 2, RequireRestart = false, HintText = "Scales this effect. 1.0 = standard.")]
        [SettingPropertyGroup(G1, GroupOrder = 1)]
        public float MeleeDamageMultiplier { get; set; } = 1.0f;

        // 2) Increased Melee Damage vs Mounted
        private const string G2 = "Melee Damage vs Mounted";
        [SettingPropertyBool("Enabled", Order = 0, RequireRestart = false, HintText = "Your troops deal more melee damage to mounted enemies.")]
        [SettingPropertyGroup(G2, GroupOrder = 2)]
        public bool MeleeVsMountedEnabled { get; set; } = true;
        [SettingPropertyInteger("Tier (1-3)", 1, 3, Order = 1, RequireRestart = false, HintText = "3 = strongest vanilla value.")]
        [SettingPropertyGroup(G2, GroupOrder = 2)]
        public int MeleeVsMountedTier { get; set; } = 3;
        [SettingPropertyFloatingInteger("Multiplier", 0.1f, 10f, "0.00", Order = 2, RequireRestart = false, HintText = "Scales this effect. 1.0 = standard.")]
        [SettingPropertyGroup(G2, GroupOrder = 2)]
        public float MeleeVsMountedMultiplier { get; set; } = 1.0f;

        // 3) Increased Ranged Damage
        private const string G3 = "Ranged Damage";
        [SettingPropertyBool("Enabled", Order = 0, RequireRestart = false, HintText = "Your troops deal more ranged damage.")]
        [SettingPropertyGroup(G3, GroupOrder = 3)]
        public bool RangedDamageEnabled { get; set; } = true;
        [SettingPropertyInteger("Tier (1-3)", 1, 3, Order = 1, RequireRestart = false, HintText = "3 = strongest vanilla value.")]
        [SettingPropertyGroup(G3, GroupOrder = 3)]
        public int RangedDamageTier { get; set; } = 3;
        [SettingPropertyFloatingInteger("Multiplier", 0.1f, 10f, "0.00", Order = 2, RequireRestart = false, HintText = "Scales this effect. 1.0 = standard.")]
        [SettingPropertyGroup(G3, GroupOrder = 3)]
        public float RangedDamageMultiplier { get; set; } = 1.0f;

        // 4) Increased Charge Damage
        private const string G4 = "Charge Damage";
        [SettingPropertyBool("Enabled", Order = 0, RequireRestart = false, HintText = "Your troops deal more charge (couched/trample) damage.")]
        [SettingPropertyGroup(G4, GroupOrder = 4)]
        public bool ChargeDamageEnabled { get; set; } = true;
        [SettingPropertyInteger("Tier (1-3)", 1, 3, Order = 1, RequireRestart = false, HintText = "3 = strongest vanilla value.")]
        [SettingPropertyGroup(G4, GroupOrder = 4)]
        public int ChargeDamageTier { get; set; } = 3;
        [SettingPropertyFloatingInteger("Multiplier", 0.1f, 10f, "0.00", Order = 2, RequireRestart = false, HintText = "Scales this effect. 1.0 = standard.")]
        [SettingPropertyGroup(G4, GroupOrder = 4)]
        public float ChargeDamageMultiplier { get; set; } = 1.0f;

        // 5) Decreased incoming Charge Damage
        private const string G5 = "Incoming Charge Damage Reduction";
        [SettingPropertyBool("Enabled", Order = 0, RequireRestart = false, HintText = "Your troops take less charge damage.")]
        [SettingPropertyGroup(G5, GroupOrder = 5)]
        public bool IncomingChargeReductionEnabled { get; set; } = true;
        [SettingPropertyInteger("Tier (1-3)", 1, 3, Order = 1, RequireRestart = false, HintText = "3 = strongest vanilla value.")]
        [SettingPropertyGroup(G5, GroupOrder = 5)]
        public int IncomingChargeReductionTier { get; set; } = 3;
        [SettingPropertyFloatingInteger("Multiplier", 0.1f, 10f, "0.00", Order = 2, RequireRestart = false, HintText = "Scales this effect. 1.0 = standard.")]
        [SettingPropertyGroup(G5, GroupOrder = 5)]
        public float IncomingChargeReductionMultiplier { get; set; } = 1.0f;

        // 6) Decreased Ranged Accuracy Penalty
        private const string G6 = "Ranged Accuracy";
        [SettingPropertyBool("Enabled", Order = 0, RequireRestart = false, HintText = "Your ranged troops are more accurate (less penalty).")]
        [SettingPropertyGroup(G6, GroupOrder = 6)]
        public bool RangedAccuracyEnabled { get; set; } = true;
        [SettingPropertyInteger("Tier (1-3)", 1, 3, Order = 1, RequireRestart = false, HintText = "3 = strongest vanilla value.")]
        [SettingPropertyGroup(G6, GroupOrder = 6)]
        public int RangedAccuracyTier { get; set; } = 3;
        [SettingPropertyFloatingInteger("Multiplier", 0.1f, 10f, "0.00", Order = 2, RequireRestart = false, HintText = "Scales this effect. 1.0 = standard.")]
        [SettingPropertyGroup(G6, GroupOrder = 6)]
        public float RangedAccuracyMultiplier { get; set; } = 1.0f;

        // 7) Decreased Morale Shock (resistance)
        private const string G7 = "Morale Shock Resistance";
        [SettingPropertyBool("Enabled", Order = 0, RequireRestart = false, HintText = "Your troops lose less morale from shocks.")]
        [SettingPropertyGroup(G7, GroupOrder = 7)]
        public bool MoraleShockResistEnabled { get; set; } = true;
        [SettingPropertyInteger("Tier (1-3)", 1, 3, Order = 1, RequireRestart = false, HintText = "3 = strongest vanilla value.")]
        [SettingPropertyGroup(G7, GroupOrder = 7)]
        public int MoraleShockResistTier { get; set; } = 3;
        [SettingPropertyFloatingInteger("Multiplier", 0.1f, 10f, "0.00", Order = 2, RequireRestart = false, HintText = "Scales this effect. 1.0 = standard.")]
        [SettingPropertyGroup(G7, GroupOrder = 7)]
        public float MoraleShockResistMultiplier { get; set; } = 1.0f;

        // 8) Decreased incoming Melee Attack Damage
        private const string G8 = "Incoming Melee Damage Reduction";
        [SettingPropertyBool("Enabled", Order = 0, RequireRestart = false, HintText = "Your troops take less melee damage.")]
        [SettingPropertyGroup(G8, GroupOrder = 8)]
        public bool IncomingMeleeReductionEnabled { get; set; } = true;
        [SettingPropertyInteger("Tier (1-3)", 1, 3, Order = 1, RequireRestart = false, HintText = "3 = strongest vanilla value.")]
        [SettingPropertyGroup(G8, GroupOrder = 8)]
        public int IncomingMeleeReductionTier { get; set; } = 3;
        [SettingPropertyFloatingInteger("Multiplier", 0.1f, 10f, "0.00", Order = 2, RequireRestart = false, HintText = "Scales this effect. 1.0 = standard.")]
        [SettingPropertyGroup(G8, GroupOrder = 8)]
        public float IncomingMeleeReductionMultiplier { get; set; } = 1.0f;

        // 9) Decreased incoming Ranged Attack Damage
        private const string G9 = "Incoming Ranged Damage Reduction";
        [SettingPropertyBool("Enabled", Order = 0, RequireRestart = false, HintText = "Your troops take less ranged damage.")]
        [SettingPropertyGroup(G9, GroupOrder = 9)]
        public bool IncomingRangedReductionEnabled { get; set; } = true;
        [SettingPropertyInteger("Tier (1-3)", 1, 3, Order = 1, RequireRestart = false, HintText = "3 = strongest vanilla value.")]
        [SettingPropertyGroup(G9, GroupOrder = 9)]
        public int IncomingRangedReductionTier { get; set; } = 3;
        [SettingPropertyFloatingInteger("Multiplier", 0.1f, 10f, "0.00", Order = 2, RequireRestart = false, HintText = "Scales this effect. 1.0 = standard.")]
        [SettingPropertyGroup(G9, GroupOrder = 9)]
        public float IncomingRangedReductionMultiplier { get; set; } = 1.0f;

        // 10) Decreased Shield Damage
        private const string G10 = "Shield Damage Reduction";
        [SettingPropertyBool("Enabled", Order = 0, RequireRestart = false, HintText = "Your troops' shields take less damage.")]
        [SettingPropertyGroup(G10, GroupOrder = 10)]
        public bool ShieldDamageReductionEnabled { get; set; } = true;
        [SettingPropertyInteger("Tier (1-3)", 1, 3, Order = 1, RequireRestart = false, HintText = "3 = strongest vanilla value.")]
        [SettingPropertyGroup(G10, GroupOrder = 10)]
        public int ShieldDamageReductionTier { get; set; } = 3;
        [SettingPropertyFloatingInteger("Multiplier", 0.1f, 10f, "0.00", Order = 2, RequireRestart = false, HintText = "Scales this effect. 1.0 = standard.")]
        [SettingPropertyGroup(G10, GroupOrder = 10)]
        public float ShieldDamageReductionMultiplier { get; set; } = 1.0f;

        // 11) Increased Troop Movement Speed
        private const string G11 = "Troop Movement Speed";
        [SettingPropertyBool("Enabled", Order = 0, RequireRestart = false, HintText = "Your foot troops move faster in battle.")]
        [SettingPropertyGroup(G11, GroupOrder = 11)]
        public bool TroopSpeedEnabled { get; set; } = true;
        [SettingPropertyInteger("Tier (1-3)", 1, 3, Order = 1, RequireRestart = false, HintText = "3 = strongest vanilla value.")]
        [SettingPropertyGroup(G11, GroupOrder = 11)]
        public int TroopSpeedTier { get; set; } = 3;
        [SettingPropertyFloatingInteger("Multiplier", 0.1f, 10f, "0.00", Order = 2, RequireRestart = false, HintText = "Scales this effect. 1.0 = standard.")]
        [SettingPropertyGroup(G11, GroupOrder = 11)]
        public float TroopSpeedMultiplier { get; set; } = 1.0f;

        // 12) Increased Mount Movement Speed
        private const string G12 = "Mount Movement Speed";
        [SettingPropertyBool("Enabled", Order = 0, RequireRestart = false, HintText = "Your mounts move faster in battle.")]
        [SettingPropertyGroup(G12, GroupOrder = 12)]
        public bool MountSpeedEnabled { get; set; } = true;
        [SettingPropertyInteger("Tier (1-3)", 1, 3, Order = 1, RequireRestart = false, HintText = "3 = strongest vanilla value.")]
        [SettingPropertyGroup(G12, GroupOrder = 12)]
        public int MountSpeedTier { get; set; } = 3;
        [SettingPropertyFloatingInteger("Multiplier", 0.1f, 10f, "0.00", Order = 2, RequireRestart = false, HintText = "Scales this effect. 1.0 = standard.")]
        [SettingPropertyGroup(G12, GroupOrder = 12)]
        public float MountSpeedMultiplier { get; set; } = 1.0f;

        // 13) Increased Morale Shock dealt by melee troops
        private const string G13 = "Morale Shock Dealt";
        [SettingPropertyBool("Enabled", Order = 0, RequireRestart = false, HintText = "Your melee troops deal more morale shock to enemies.")]
        [SettingPropertyGroup(G13, GroupOrder = 13)]
        public bool MoraleShockDealtEnabled { get; set; } = true;
        [SettingPropertyInteger("Tier (1-3)", 1, 3, Order = 1, RequireRestart = false, HintText = "3 = strongest vanilla value.")]
        [SettingPropertyGroup(G13, GroupOrder = 13)]
        public int MoraleShockDealtTier { get; set; } = 3;
        [SettingPropertyFloatingInteger("Multiplier", 0.1f, 10f, "0.00", Order = 2, RequireRestart = false, HintText = "Scales this effect. 1.0 = standard.")]
        [SettingPropertyGroup(G13, GroupOrder = 13)]
        public float MoraleShockDealtMultiplier { get; set; } = 1.0f;

        /// <summary>The 13 banner effects this mod manages, in menu order.</summary>
        public static BannerEffect[] GetManagedEffects() => new[]
        {
            DefaultBannerEffects.IncreasedMeleeDamage,
            DefaultBannerEffects.IncreasedMeleeDamageAgainstMountedTroops,
            DefaultBannerEffects.IncreasedRangedDamage,
            DefaultBannerEffects.IncreasedChargeDamage,
            DefaultBannerEffects.DecreasedChargeDamage,
            DefaultBannerEffects.DecreasedRangedAccuracyPenalty,
            DefaultBannerEffects.DecreasedMoraleShock,
            DefaultBannerEffects.DecreasedMeleeAttackDamage,
            DefaultBannerEffects.DecreasedRangedAttackDamage,
            DefaultBannerEffects.DecreasedShieldDamage,
            DefaultBannerEffects.IncreasedTroopMovementSpeed,
            DefaultBannerEffects.IncreasedMountMovementSpeed,
            DefaultBannerEffects.IncreasedMoraleShockByMeleeTroops,
        };

        /// <summary>
        /// Maps the queried banner effect (compared by reference to the game's singletons)
        /// to its per-effect config. Returns false if the effect is disabled or unrecognized.
        /// </summary>
        public bool TryGetEffectConfig(BannerEffect effect, out int tier, out float multiplier)
        {
            tier = 3;
            multiplier = 1.0f;
            // The game only calls the patched helper with a DefaultBannerEffects.* singleton,
            // so by the time we run these are initialized; a plain null check is enough.
            if (effect is null)
                return false;

            bool enabled;
            if (effect == DefaultBannerEffects.IncreasedMeleeDamage)
                { enabled = MeleeDamageEnabled; tier = MeleeDamageTier; multiplier = MeleeDamageMultiplier; }
            else if (effect == DefaultBannerEffects.IncreasedMeleeDamageAgainstMountedTroops)
                { enabled = MeleeVsMountedEnabled; tier = MeleeVsMountedTier; multiplier = MeleeVsMountedMultiplier; }
            else if (effect == DefaultBannerEffects.IncreasedRangedDamage)
                { enabled = RangedDamageEnabled; tier = RangedDamageTier; multiplier = RangedDamageMultiplier; }
            else if (effect == DefaultBannerEffects.IncreasedChargeDamage)
                { enabled = ChargeDamageEnabled; tier = ChargeDamageTier; multiplier = ChargeDamageMultiplier; }
            else if (effect == DefaultBannerEffects.DecreasedChargeDamage)
                { enabled = IncomingChargeReductionEnabled; tier = IncomingChargeReductionTier; multiplier = IncomingChargeReductionMultiplier; }
            else if (effect == DefaultBannerEffects.DecreasedRangedAccuracyPenalty)
                { enabled = RangedAccuracyEnabled; tier = RangedAccuracyTier; multiplier = RangedAccuracyMultiplier; }
            else if (effect == DefaultBannerEffects.DecreasedMoraleShock)
                { enabled = MoraleShockResistEnabled; tier = MoraleShockResistTier; multiplier = MoraleShockResistMultiplier; }
            else if (effect == DefaultBannerEffects.DecreasedMeleeAttackDamage)
                { enabled = IncomingMeleeReductionEnabled; tier = IncomingMeleeReductionTier; multiplier = IncomingMeleeReductionMultiplier; }
            else if (effect == DefaultBannerEffects.DecreasedRangedAttackDamage)
                { enabled = IncomingRangedReductionEnabled; tier = IncomingRangedReductionTier; multiplier = IncomingRangedReductionMultiplier; }
            else if (effect == DefaultBannerEffects.DecreasedShieldDamage)
                { enabled = ShieldDamageReductionEnabled; tier = ShieldDamageReductionTier; multiplier = ShieldDamageReductionMultiplier; }
            else if (effect == DefaultBannerEffects.IncreasedTroopMovementSpeed)
                { enabled = TroopSpeedEnabled; tier = TroopSpeedTier; multiplier = TroopSpeedMultiplier; }
            else if (effect == DefaultBannerEffects.IncreasedMountMovementSpeed)
                { enabled = MountSpeedEnabled; tier = MountSpeedTier; multiplier = MountSpeedMultiplier; }
            else if (effect == DefaultBannerEffects.IncreasedMoraleShockByMeleeTroops)
                { enabled = MoraleShockDealtEnabled; tier = MoraleShockDealtTier; multiplier = MoraleShockDealtMultiplier; }
            else
                return false;

            return enabled;
        }
    }
}
