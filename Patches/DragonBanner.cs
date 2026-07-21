using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace DragonBannerAlmighty.Patches
{
    /// <summary>
    /// Shared logic for the concrete Sandbox-model Harmony patches.
    ///
    /// The vanilla helper <c>Helpers.BannerHelper.AddBannerBonusForBanner</c> is tiny+static and gets
    /// JIT-inlined into the Sandbox combat models (net472, no ReadyToRun), so a Harmony patch on it
    /// never fires. Instead we Postfix the concrete model methods and re-apply each ENABLED
    /// Dragon-Banner effect from MCM.
    ///
    /// All 13 vanilla banner effects are <see cref="EffectIncrementType.AddFactor"/> in 1.4.5, and the
    /// "Decreased*" ones carry negative tier values, so a single rule covers every case:
    ///     result *= (1 + signedFactor)
    /// where signedFactor = effect.GetBonusAtLevel(tier) * multiplier.
    ///
    /// DETECTION is an OR of two conditions (evaluated per relevant agent), because the vanilla
    /// "carried banner" path (a formation banner-bearer) returns null when the player fights without a
    /// bearer formation:
    ///   1. the formation's active battle banner IS the dragon_banner, OR
    ///   2. the relevant agent is on the PLAYER's team AND the player's party possesses the banner.
    /// </summary>
    internal static class DragonBanner
    {
        public const string ItemId = "dragon_banner";

        private static readonly object Sync = new object();

        // Player-possession cache, refreshed once per Mission (possession does not change mid-battle
        // in practice; recomputing per battle is cheap and correct).
        private static Mission? _cachedMission;
        private static bool _cachedHasBanner;

        /// <summary>True when the formation's active battle banner is the Dragon Banner.</summary>
        public static bool IsDragon(BannerComponent? banner)
            => banner?.Item != null && banner.Item.StringId == ItemId;

        /// <summary>Active battle banner for a formation, detected exactly like the vanilla models do.</summary>
        public static BannerComponent? GetActiveBanner(Formation? formation)
        {
            if (formation == null)
                return null;
            try
            {
                return MissionGameModels.Current?.BattleBannerBearersModel?.GetActiveBanner(formation);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// MCM-configured signed AddFactor for an effect (tier value * multiplier), or false when the
        /// mod/effect is disabled or the factor is zero. Safe before MCM has initialised
        /// (Settings.Instance null -> false, vanilla behaviour preserved).
        /// </summary>
        public static bool TryGetFactor(BannerEffect? effect, out float factor)
        {
            factor = 0f;
            if (effect == null)
                return false;
            var s = Settings.Instance;
            if (s == null || !s.MasterEnabled)
                return false;
            if (!s.TryGetEffectConfig(effect, out var tier, out var multiplier))
                return false;
            if (tier < 1) tier = 1; else if (tier > 3) tier = 3;
            factor = effect.GetBonusAtLevel(tier) * multiplier;
            return factor != 0f;
        }

        /// <summary>Is this agent on the player's own team? (Allies are excluded by design.)</summary>
        public static bool IsPlayerSideAgent(Agent? agent)
        {
            try
            {
                var team = agent?.Team;
                return team != null && team.IsPlayerTeam;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// True when the player actually owns the Dragon Banner: it is in the main party's item roster,
        /// or equipped in the main hero's banner slot (EquipmentIndex.ExtraWeaponSlot in 1.4.5, per the
        /// game's own ItemVM mapping of ItemTypeEnum.Banner). Cached per Mission. Null-safe outside a
        /// campaign (Campaign.Current / MainParty / MainHero may be null) -> false.
        /// </summary>
        public static bool PlayerHasBanner()
        {
            var mission = Mission.Current;
            lock (Sync)
            {
                if (mission != null && ReferenceEquals(mission, _cachedMission))
                    return _cachedHasBanner;
                _cachedHasBanner = ComputePlayerHasBanner();
                _cachedMission = mission;
                return _cachedHasBanner;
            }
        }

        private static bool ComputePlayerHasBanner()
        {
            try
            {
                if (Campaign.Current == null)
                    return false;

                // 1) main party inventory
                var roster = MobileParty.MainParty?.ItemRoster;
                if (roster != null)
                {
                    for (int i = 0; i < roster.Count; i++)
                    {
                        var item = roster.GetElementCopyAtIndex(i).EquipmentElement.Item;
                        if (item != null && item.StringId == ItemId)
                            return true;
                    }
                }

                // 2) main hero's banner slot (ExtraWeaponSlot is the banner slot in 1.4.5)
                var equipment = Hero.MainHero?.BattleEquipment;
                if (equipment != null)
                {
                    var slotItem = equipment.GetEquipmentFromSlot(EquipmentIndex.ExtraWeaponSlot).Item;
                    if (slotItem != null && slotItem.StringId == ItemId)
                        return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// The per-side gate used by every combat postfix: effects apply when the side's active banner
        /// is the Dragon Banner, OR (robust fallback) the player owns the banner and the side's agent is
        /// on the player's team.
        /// </summary>
        public static bool ShouldApply(BannerComponent? activeBanner, Agent? sideAgent)
            => IsDragon(activeBanner) || (PlayerHasBanner() && IsPlayerSideAgent(sideAgent));
    }
}
