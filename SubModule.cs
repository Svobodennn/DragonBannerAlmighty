using System;
using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace DragonBannerAlmighty
{
    public sealed class SubModule : MBSubModuleBase
    {
        internal const string HarmonyDomain = "com.melih.dragonbanneralmighty";
        private Harmony? _harmony;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            try
            {
                _harmony = new Harmony(HarmonyDomain);
                _harmony.PatchAll(typeof(SubModule).Assembly);
                _ = Settings.Instance;
            }
            catch (Exception ex)
            {
                Debug.Print("[Dragon Banner Almighty] init failed: " + ex);
            }
        }
    }
}
