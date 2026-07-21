using HarmonyLib;
using SandBox.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace DragonBannerAlmighty.Patches
{
    // ------------------------------------------------------------------------------------------------
    // Morale-shock effects (return a ValueTuple).
    //
    // SandBox.GameComponents.SandboxBattleMoraleModel (1.4.5):
    //   (float affectedSideMaxMoraleLoss, float affectorSideMaxMoraleGain)
    //       CalculateMaxMoraleChangeDueToAgentIncapacitated(Agent affectedAgent, AgentState, Agent affectorAgent, in KillingBlow)
    //     affected -> DecreasedMoraleShock              reduces Item1 (the morale LOSS)
    //     affector -> IncreasedMoraleShockByMeleeTroops increases Item2 (the morale GAIN), infantry only
    //   (float, float) CalculateMaxMoraleChangeDueToAgentPanicked(Agent agent)
    //     agent    -> DecreasedMoraleShock              reduces Item1
    //
    // Vanilla clamps both tuple members to >= 0, so we do too. All AddFactor. Detection:
    // DragonBanner.ShouldApply(bannerForThatAgent, thatAgent) per side.
    // ------------------------------------------------------------------------------------------------

    [HarmonyPatch(typeof(SandboxBattleMoraleModel), nameof(SandboxBattleMoraleModel.CalculateMaxMoraleChangeDueToAgentIncapacitated))]
    internal static class MoraleIncapacitatedPatch
    {
        private static void Postfix(ref (float affectedSideMaxMoraleLoss, float affectorSideMaxMoraleGain) __result,
                                    Agent affectedAgent, Agent affectorAgent)
        {
            try
            {
                if (DragonBanner.ShouldApply(DragonBanner.GetActiveBanner(affectedAgent?.Formation), affectedAgent) &&
                    DragonBanner.TryGetFactor(DefaultBannerEffects.DecreasedMoraleShock, out var fLoss))
                {
                    float v = __result.affectedSideMaxMoraleLoss * (1f + fLoss);
                    __result.affectedSideMaxMoraleLoss = v < 0f ? 0f : v;
                }

                if (DragonBanner.ShouldApply(DragonBanner.GetActiveBanner(affectorAgent?.Formation), affectorAgent) &&
                    affectorAgent?.Character != null &&
                    affectorAgent.Character.DefaultFormationClass == FormationClass.Infantry &&
                    DragonBanner.TryGetFactor(DefaultBannerEffects.IncreasedMoraleShockByMeleeTroops, out var fGain))
                {
                    float v = __result.affectorSideMaxMoraleGain * (1f + fGain);
                    __result.affectorSideMaxMoraleGain = v < 0f ? 0f : v;
                }
            }
            catch
            {
                // never break combat
            }
        }
    }

    [HarmonyPatch(typeof(SandboxBattleMoraleModel), nameof(SandboxBattleMoraleModel.CalculateMaxMoraleChangeDueToAgentPanicked))]
    internal static class MoralePanickedPatch
    {
        private static void Postfix(ref (float affectedSideMaxMoraleLoss, float affectorSideMaxMoraleGain) __result, Agent agent)
        {
            try
            {
                if (DragonBanner.ShouldApply(DragonBanner.GetActiveBanner(agent?.Formation), agent) &&
                    DragonBanner.TryGetFactor(DefaultBannerEffects.DecreasedMoraleShock, out var fLoss))
                {
                    float v = __result.affectedSideMaxMoraleLoss * (1f + fLoss);
                    __result.affectedSideMaxMoraleLoss = v < 0f ? 0f : v;
                }
            }
            catch
            {
                // never break combat
            }
        }
    }
}
