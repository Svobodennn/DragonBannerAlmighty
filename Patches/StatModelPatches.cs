using HarmonyLib;
using SandBox.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace DragonBannerAlmighty.Patches
{
    // ------------------------------------------------------------------------------------------------
    // Movement-speed + ranged-accuracy effects (mutate AgentDrivenProperties).
    //
    // SandBox.GameComponents.SandboxAgentStatCalculateModel (1.4.5):
    //   private void SetPerkAndBannerEffectsOnAgent(Agent agent, CharacterObject agentCharacter,
    //                                               AgentDrivenProperties adp, WeaponComponentData equippedWeaponComponent)
    //     -> IncreasedTroopMovementSpeed    -> adp.MaxSpeedMultiplier
    //     -> DecreasedRangedAccuracyPenalty -> adp.WeaponInaccuracy (only when a ranged weapon is equipped)
    //   private void UpdateHorseStats(Agent agent, AgentDrivenProperties adp)   // agent == the mount
    //     -> IncreasedMountMovementSpeed    -> adp.MountSpeed (gated on the rider)
    //
    // All AddFactor -> property *= (1 + signedFactor). WeaponInaccuracy factor is negative (less
    // inaccuracy = better). Detection: DragonBanner.ShouldApply(banner, agent) (for horses: the rider).
    // ------------------------------------------------------------------------------------------------

    [HarmonyPatch(typeof(SandboxAgentStatCalculateModel), "SetPerkAndBannerEffectsOnAgent")]
    internal static class SetPerkAndBannerEffectsOnAgentPatch
    {
        private static void Postfix(Agent agent, AgentDrivenProperties agentDrivenProperties, WeaponComponentData equippedWeaponComponent)
        {
            try
            {
                if (agent == null || agentDrivenProperties == null)
                    return;
                if (!DragonBanner.ShouldApply(DragonBanner.GetActiveBanner(agent.Formation), agent))
                    return;

                if (DragonBanner.TryGetFactor(DefaultBannerEffects.IncreasedTroopMovementSpeed, out var fSpeed))
                    agentDrivenProperties.MaxSpeedMultiplier *= 1f + fSpeed;

                if ((equippedWeaponComponent?.IsRangedWeapon ?? false) &&
                    DragonBanner.TryGetFactor(DefaultBannerEffects.DecreasedRangedAccuracyPenalty, out var fAcc))
                    agentDrivenProperties.WeaponInaccuracy *= 1f + fAcc;
            }
            catch
            {
                // never break combat
            }
        }
    }

    [HarmonyPatch(typeof(SandboxAgentStatCalculateModel), "UpdateHorseStats")]
    internal static class UpdateHorseStatsPatch
    {
        private static void Postfix(Agent agent, AgentDrivenProperties agentDrivenProperties)
        {
            try
            {
                if (agent == null || agentDrivenProperties == null)
                    return;
                var rider = agent.RiderAgent;
                if (rider == null)
                    return; // vanilla only touches MountSpeed when there is a rider
                if (DragonBanner.ShouldApply(DragonBanner.GetActiveBanner(rider.Formation), rider) &&
                    DragonBanner.TryGetFactor(DefaultBannerEffects.IncreasedMountMovementSpeed, out var f))
                    agentDrivenProperties.MountSpeed *= 1f + f;
            }
            catch
            {
                // never break combat
            }
        }
    }
}
