using BattleUI.EvilStock;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LimbusOrange.Manager;

public static class BattleManager
{
	public static Dictionary<string, Sprite> sprites;
	public static UIButtonWithOverlayImg timeSpeedBtn;

	static BattleManager()
	{
		InitResources();
	}

	static void UIInitialize()
	{
		timeSpeedBtn._onClick.AddListener((Action)OnClickTimeSpeedBtn);
		UpdateTimeSpeedBtnImage();
	}

	static void OnClickTimeSpeedBtn()
	{
		int timeSpeed = Plugin.timeSpeedConfig.Value;
		timeSpeed += 1;
		timeSpeed = timeSpeed > 3 ? 1 : timeSpeed;
		Plugin.timeSpeedConfig.Value = timeSpeed;
		Time.timeScale = timeSpeed;
		UpdateTimeSpeedBtnImage();
	}

	static void UpdateTimeSpeedBtnImage()
	{
		string fileName = $"{Plugin.timeSpeedConfig.Value}x";
		sprites.TryGetValue(fileName, out Sprite sprite);
		if (sprite)
		{
			timeSpeedBtn.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
		}
	}

	static void InitResources()
	{
		sprites = new Dictionary<string, Sprite>();
		foreach (var fileInfo in new DirectoryInfo(Plugin.ModPath + "/Resources/Images").GetFiles().Where(f => f.Extension is ".jpg" or ".png"))
		{
			Texture2D texture2D = new(2, 2);
			ImageConversion.LoadImage(texture2D, File.ReadAllBytes(fileInfo.FullName));
			var sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height),
				new Vector2(0.5f, 0.5f));
			var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.FullName);
			texture2D.name = fileNameWithoutExtension;
			sprite.name = fileNameWithoutExtension;
			Object.DontDestroyOnLoad(sprite);
			sprite.hideFlags |= HideFlags.HideAndDontSave;
			sprites[fileNameWithoutExtension] = sprite;
		}
	}

	[HarmonyPatch(typeof(EvilStockController), nameof(EvilStockController.Init))]
	[HarmonyPostfix]
	private static void EvilStockController_Init(EvilStockController __instance)
	{
		
		if (timeSpeedBtn) return;
		Transform pivotRight = __instance.transform.GetChild(0).GetChild(2);
		GameObject inBattleButtons = null;
		for (int i = 0; i < pivotRight.childCount; i++)
		{
			var obj = pivotRight.GetChild(i).gameObject;
			if(obj.name.Contains("InBattleButtons"))
			{
				inBattleButtons = obj;
				break;
			}
		}
		if(inBattleButtons == null) return;
		timeSpeedBtn = Object.Instantiate(inBattleButtons, inBattleButtons.transform.parent).GetComponentInChildren<UIButtonWithOverlayImg>();

		//Change layout to insert timeSpeedBtn
		float width = timeSpeedBtn.GetComponent<RectTransform>().rect.width;

		Vector3 localPos = inBattleButtons.transform.localPosition;
		localPos.x -= width;
		inBattleButtons.transform.localPosition = localPos;

		var upperSkillInfoBtn = __instance._upperSkillInfoStateSettingUI;
		localPos = upperSkillInfoBtn.transform.localPosition;
		localPos.x -= width;
		upperSkillInfoBtn.transform.localPosition = localPos;
		UIInitialize();
	}
}
