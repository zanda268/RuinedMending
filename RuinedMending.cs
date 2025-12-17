using ComplexLogger;
using Il2CppInterop;
using Il2CppInterop.Runtime.Injection; 
using MelonLoader;
using System.Collections;
using UnityEngine;

namespace RuinedMending
{
	public class RuinedMending : MelonMod
	{
		internal static ComplexLogger<RuinedMending> Logger = new();

        public override void OnInitializeMelon()
		{
            Settings.OnLoad();
        }


		internal static void Log(String str, ComplexLogger.FlaggedLoggingLevel level = FlaggedLoggingLevel.Always)
		{
			Logger.Log(str, level);
		}
	}
}