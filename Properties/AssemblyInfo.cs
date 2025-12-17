using MelonLoader;
using RuinedMending;
using System.Reflection;
using System.Runtime.InteropServices;

//This is a C# comment. Comments have no impact on compilation.

//ModName, ModVersion, ModAuthor, and ModNamespace.ModClassInheritingFromMelonMod all need changed.

[assembly: AssemblyTitle("RuinedMending")]
[assembly: AssemblyCopyright("Zanda268")]

//Version numbers in C# are a set of 1 to 4 positive integers separated by periods.
//Mods typically use 3 numbers. For example: 1.2.1
//The mod version need specified in three places.
[assembly: AssemblyVersion("0.9.0")]
[assembly: AssemblyFileVersion("0.9.0")]
[assembly: MelonInfo(typeof(RuinedMending.RuinedMending), "RuinedMending", "0.9.0", "Zanda268", null)]

//This tells MelonLoader that the mod is only for The Long Dark.
[assembly: MelonGame("Hinterland", "TheLongDark")]