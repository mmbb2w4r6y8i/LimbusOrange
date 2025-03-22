using UnityEngine;
using HarmonyLib;
using BattleUI.Operation;
using UnityEngine.SceneManagement;
using Security;

namespace LimbusOrange.Manager;

public static class GlobalManager
{
	public static Scene currentScene;
	public static NewOperationAutoSelectManager newOperationAutoSelectManager;
	public static NewOperationController newOperationController;

	[HarmonyPatch(typeof(GlobalGameManager), nameof(GlobalGameManager.OnSceneLoaded))]
	[HarmonyPrefix]
	private static bool GlobalGameManager_OnSceneLoaded(Scene scene)
	{
		currentScene = scene;
		if (scene.name == "BattleScene")
		{
			newOperationAutoSelectManager = Object.FindAnyObjectByType<NewOperationAutoSelectManager>();
			newOperationController = Object.FindAnyObjectByType<NewOperationController>();
			Time.timeScale = Plugin.timeSpeedConfig.Value;
			Debug.Log($"BattleScene loaded timescale = {Time.timeScale}");
		}
		else
		{
			Time.timeScale = 1;
		}
		return true;
	}

	[HarmonyPatch(typeof(Time), nameof(Time.timeScale), MethodType.Setter)]
	[HarmonyPrefix]
	private static bool TimeScale_Set(ref float value)
	{
		if(currentScene.name == "BattleScene")
		{
			value = Plugin.timeSpeedConfig.Value;
		}
		else
		{
			value = 1;
		}
		return true;
	}

	[HarmonyPatch(typeof(SpeedHackHandler), nameof(SpeedHackHandler.IsCheatDetected), MethodType.Getter)]
	[HarmonyPrefix]
	private static bool SpeedHackHandler_IsCheatDetected(ref bool __result)
	{
		__result = false;
		return false;
	}
}
