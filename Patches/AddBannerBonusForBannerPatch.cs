using HarmonyLib;
using SandBox.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace DragonBannerAlmighty.Patches
{
    // ------------------------------------------------------------------------------------------------
    // Combat damage effects.
    //
    // The vanilla Helpers.BannerHelper.AddBannerBonusForBanner helper is inlined into these models, so
    // we Postfix the concrete model methods that call it instead.
    //
    // SandBox.GameComponents.SandboxAgentApplyDamageModel (1.4.5):
    //   float ApplyDamageAmplifications(in AttackInformation, in AttackCollisionData, float baseDamage)
    //     attacker -> IncreasedMeleeDamage / IncreasedMeleeDamageAgainstMountedTroops (melee),
    //                 IncreasedRangedDamage (ranged/consumable), IncreasedChargeDamage (horse charge)
    //     victim   -> DecreasedChargeDamage (horse charge)
    //   float ApplyDamageReductions(in AttackInformation, in AttackCollisionData, float baseDamage)
    //     victim   -> DecreasedRangedAttackDamage (attacker ranged), DecreasedMeleeAttackDamage (attacker melee)
    //   float CalculateShieldDamage(in AttackInformation, float baseDamage)
    //     victim   -> DecreasedShieldDamage
    //
    // Every effect is AddFactor, so we scale the returned damage by (1 + signedFactor). Struct in-params
    // are received by ref (1:1 match with the original ByRef arg; read only, never written). Detection
    // uses DragonBanner.ShouldApply(activeBannerForThatSide, agentForThatSide).
    // ------------------------------------------------------------------------------------------------

    [HarmonyPatch(typeof(SandboxAgentApplyDamageModel), nameof(SandboxAgentApplyDamageModel.ApplyDamageAmplifications))]
    internal static class ApplyDamageAmplificationsPatch
    {
        private static void Postfix(ref float __result, ref AttackInformation attackInformation, ref AttackCollisionData collisionData)
        {
            try
            {
                float atkFactor = 0f;
                if (DragonBanner.ShouldApply(DragonBanner.GetActiveBanner(attackInformation.AttackerFormation), attackInformation.AttackerAgent))
                {
                    var wcd = attackInformation.AttackerWeapon.CurrentUsageItem;
                    if (wcd != null)
                    {
                        if (wcd.IsMeleeWeapon)
                        {
                            if (DragonBanner.TryGetFactor(DefaultBannerEffects.IncreasedMeleeDamage, out var f))
                                atkFactor += f;
                            if (attackInformation.DoesVictimHaveMountAgent &&
                                DragonBanner.TryGetFactor(DefaultBannerEffects.IncreasedMeleeDamageAgainstMountedTroops, out var fm))
                                atkFactor += fm;
                        }
                        else if (wcd.IsConsumable)
                        {
                            if (DragonBanner.TryGetFactor(DefaultBannerEffects.IncreasedRangedDamage, out var fr))
                                atkFactor += fr;
                        }
                    }
                    if (collisionData.IsHorseCharge &&
                        DragonBanner.TryGetFactor(DefaultBannerEffects.IncreasedChargeDamage, out var fc))
                        atkFactor += fc;
                }

                float vicFactor = 0f;
                if (collisionData.IsHorseCharge &&
                    DragonBanner.ShouldApply(DragonBanner.GetActiveBanner(attackInformation.VictimFormation), attackInformation.VictimAgent) &&
                    DragonBanner.TryGetFactor(DefaultBannerEffects.DecreasedChargeDamage, out var fdc))
                    vicFactor += fdc;

                if (atkFactor != 0f)
                    __result *= 1f + atkFactor;
                if (vicFactor != 0f)
                    __result *= 1f + vicFactor;
            }
            catch
            {
                // never break combat
            }
        }
    }

    [HarmonyPatch(typeof(SandboxAgentApplyDamageModel), nameof(SandboxAgentApplyDamageModel.ApplyDamageReductions))]
    internal static class ApplyDamageReductionsPatch
    {
        private static void Postfix(ref float __result, ref AttackInformation attackInformation)
        {
            try
            {
                if (!DragonBanner.ShouldApply(DragonBanner.GetActiveBanner(attackInformation.VictimFormation), attackInformation.VictimAgent))
                    return;

                // Vanilla keys the taken-damage reduction off the ATTACKER's weapon type.
                var attackerWcd = attackInformation.AttackerWeapon.CurrentUsageItem;
                float factor = 0f;
                if (attackerWcd != null)
                {
                    if (attackerWcd.IsConsumable)
                    {
                        if (DragonBanner.TryGetFactor(DefaultBannerEffects.DecreasedRangedAttackDamage, out var fr))
                            factor += fr;
                    }
                    else if (attackerWcd.IsMeleeWeapon)
                    {
                        if (DragonBanner.TryGetFactor(DefaultBannerEffects.DecreasedMeleeAttackDamage, out var fmv))
                            factor += fmv;
                    }
                }

                if (factor != 0f)
                    __result *= 1f + factor; // factor negative -> less damage taken
            }
            catch
            {
                // never break combat
            }
        }
    }

    [HarmonyPatch(typeof(SandboxAgentApplyDamageModel), nameof(SandboxAgentApplyDamageModel.CalculateShieldDamage))]
    internal static class CalculateShieldDamagePatch
    {
        private static void Postfix(ref float __result, ref AttackInformation attackInformation)
        {
            try
            {
                if (DragonBanner.ShouldApply(DragonBanner.GetActiveBanner(attackInformation.VictimFormation), attackInformation.VictimAgent) &&
                    DragonBanner.TryGetFactor(DefaultBannerEffects.DecreasedShieldDamage, out var f))
                    __result *= 1f + f; // f negative -> shield takes less damage
            }
            catch
            {
                // never break combat
            }
        }
    }
}
