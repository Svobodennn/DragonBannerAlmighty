using System;
using System.Collections.Generic;
using HarmonyLib;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace DragonBannerAlmighty.Patches
{
    /// <summary>
    /// PRIMARY inventory item-hover tooltip. The inventory detail panel is built by
    /// <c>ItemMenuVM.RefreshItemTooltips</c>, which fills <c>TargetItemProperties</c> directly and does
    /// NOT flow through <c>TooltipRefresherCollection.RefreshItemTooltip</c>. We Postfix it and append
    /// one line per active Dragon Banner effect.
    /// </summary>
    [HarmonyPatch(typeof(ItemMenuVM), "RefreshItemTooltips")]
    internal static class ItemMenuTooltipPatch
    {
        private static void Postfix(ItemMenuVM __instance, ItemVM item)
        {
            try
            {
                if (__instance == null || item == null)
                    return;

                var itemObj = item.ItemRosterElement.EquipmentElement.Item;
                if (itemObj == null || itemObj.StringId != DragonBanner.ItemId)
                    return;

                var settings = Settings.Instance;
                if (settings == null || !settings.MasterEnabled)
                    return;

                var list = __instance.TargetItemProperties;
                if (list == null)
                    return;

                list.Add(NewProperty(" ", " "));
                list.Add(NewProperty("Dragon Banner Almighty", " "));
                foreach (var line in DragonBannerTooltip.BuildLines(settings))
                    list.Add(NewProperty(line.Key, line.Value));
            }
            catch
            {
                // A cosmetic tooltip must never break gameplay.
            }
        }

        private static ItemMenuTooltipPropertyVM NewProperty(string definition, string value)
            => new ItemMenuTooltipPropertyVM(definition, value, 0, false, (HintViewModel?)null, (string?)null, false);
    }

    /// <summary>
    /// SECONDARY: the registered ItemObject tooltip refresher (encyclopedia and any other hover
    /// context that raises an ItemObject tooltip). args[0] is an <c>EquipmentElement?</c> in 1.4.5.
    /// </summary>
    [HarmonyPatch(typeof(TooltipRefresherCollection), nameof(TooltipRefresherCollection.RefreshItemTooltip))]
    internal static class RefreshItemTooltipPatch
    {
        private static void Postfix(PropertyBasedTooltipVM propertyBasedTooltipVM, object[] args)
        {
            try
            {
                if (propertyBasedTooltipVM == null || args == null || args.Length == 0)
                    return;

                var item = (args[0] as EquipmentElement?)?.Item;
                if (item == null || item.StringId != DragonBanner.ItemId)
                    return;

                var settings = Settings.Instance;
                if (settings == null || !settings.MasterEnabled)
                    return;

                var flags = default(TooltipProperty.TooltipPropertyFlags);
                propertyBasedTooltipVM.AddProperty(" ", " ", 0, flags);
                propertyBasedTooltipVM.AddProperty("Dragon Banner Almighty", " ", 0, flags);
                foreach (var line in DragonBannerTooltip.BuildLines(settings))
                    propertyBasedTooltipVM.AddProperty(line.Key, line.Value, 0, flags);
            }
            catch
            {
                // A cosmetic tooltip must never break gameplay.
            }
        }
    }

    /// <summary>Shared builder for the per-effect tooltip lines (all 13 effects are AddFactor -> %).</summary>
    internal static class DragonBannerTooltip
    {
        public static IEnumerable<KeyValuePair<string, string>> BuildLines(Settings settings)
        {
            foreach (var effect in Settings.GetManagedEffects())
            {
                if (effect == null)
                    continue;
                if (!settings.TryGetEffectConfig(effect, out var tier, out var multiplier))
                    continue;
                if (tier < 1) tier = 1; else if (tier > 3) tier = 3;
                var bonus = effect.GetBonusAtLevel(tier) * multiplier;
                if (bonus == 0f)
                    continue;
                var percent = (int)Math.Round(bonus * 100f);
                var value = (bonus > 0f ? "+" : "") + percent + "%";
                yield return new KeyValuePair<string, string>(effect.Name.ToString(), value);
            }
        }
    }
}
