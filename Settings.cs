using UnityEngine;
using ModSettings;
using MelonLoader;

namespace RuinedMending
{
    internal class RuinedMendingSettings : JsonModSettings
    {        
        [Section("General")]
     
        [Name("Enabled")]
        [Description("Enable/disable complete mod functionality")]
        public bool modEnabled = true;       


        protected override void OnConfirm()
        {
            base.OnConfirm();
        }
    }

    internal static class Settings
    {
        public static RuinedMendingSettings options;

        public static void OnLoad()
        {
            options = new RuinedMendingSettings();
            options.AddToModSettings("Ruined Mending");
        }
    }
}
