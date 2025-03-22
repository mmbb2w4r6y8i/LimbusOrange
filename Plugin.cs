using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using BepInEx.Configuration;
using LimbusOrange.Manager;
using System.IO;
using System.Reflection;

namespace LimbusOrange;

[BepInPlugin(Guid, Name, Version)]
public class Plugin : BasePlugin
{
	public const string Guid = $"Com.{Author}.{Name}";
	public const string Name = "LimbusOrange";
	public const string Version = "0.0.1";
	public const string Author = "Orange";
	public static string ModPath;

	public static Harmony Harmony = new(Name);
	public static ConfigFile config;
	public static ConfigEntry<int> timeSpeedConfig;


	public override void Load()
	{
		ModPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		config = Config;
		timeSpeedConfig = config.Bind("Settings", "TimeSpeed", 1, "战斗的时间速度");
		// Plugin startup logic
		Log.LogInfo($"Plugin {Guid} is loaded!");
		Harmony.PatchAll(typeof(GlobalManager));
		Harmony.PatchAll(typeof(BattleManager));
	}

	public override bool Unload()
	{
		Harmony.UnpatchSelf();
		return true;
	}

	

}
