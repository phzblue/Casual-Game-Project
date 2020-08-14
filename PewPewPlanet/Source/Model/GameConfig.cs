using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;

public static class GameConfig
{
	public static SortedList<int, int> gachaCost = new SortedList<int, int>
	{
		{ 1, 10 }, { 2, 30 }, { 3, 30 }, { 4, 30 },
		{ 5, 30 }, { 6, 50 }, { 7, 50 }, { 8, 50 },
		{ 9, 50 }, { 10, 50 }, { 11, 100 },
		{ 12, 100 }, { 13, 100 }, { 14, 100 },
		{ 15, 100 }, { 16, 300 }, { 17, 300 },
		{ 18, 300 }, { 19, 300 }, { 20, 300 },
		{ 21, 500 }

		//{ 1, 1 }
	};

	public static int GetGachaCost()
	{
		int gachaNum = GameManager.instance.playerData.gachaNum;

		if(gachaCost.ContainsKey(gachaNum))
		{
			return gachaCost[gachaNum];
		}
		else
		{
			return gachaCost[gachaCost.Count];
		}
	}

	static List<string> adsSkin = new List<string>() { "Agg", "Bee2stinger", "Nightowl" };
	static List<string> scoreSkin = new List<string>() { "Lime", "Slack", "Tera" };
	static List<string> matchSkin = new List<string>() { "Passion", "Nano", "SwallowRed" };

	public static void CheckAdsSkin()
	{
		Dictionary<string, ShipConfig> shipConfigMap = GameManager.instance.shipConfigMap;
		PlayerData playerData = GameManager.instance.playerData;

		foreach(string skin in adsSkin)
		{
			if (!playerData.skinPurchased.Contains(skin) &&
				playerData.adsWatched >= shipConfigMap[skin].objective &&
				!playerData.unlockedSkin.Contains(skin))
			{
				playerData.unlockedSkin.Add(skin);
			}
		}
	}

	public static void CheckMatchSkin()
	{
		Dictionary<string, ShipConfig> shipConfigMap = GameManager.instance.shipConfigMap;
		PlayerData playerData = GameManager.instance.playerData;

		foreach (string skin in matchSkin)
		{
			if (!playerData.skinPurchased.Contains(skin) &&
				playerData.matchPlayed >= shipConfigMap[skin].objective &&
				!playerData.unlockedSkin.Contains(skin))
			{
				playerData.unlockedSkin.Add(skin);
			}
		}
	}

	public static void CheckScoreSkin(int score)
	{
		Dictionary<string, ShipConfig> shipConfigMap = GameManager.instance.shipConfigMap;
		PlayerData playerData = GameManager.instance.playerData;

		foreach (string skin in scoreSkin)
		{
			if (!playerData.skinPurchased.Contains(skin) &&
				score >= shipConfigMap[skin].objective &&
				!playerData.unlockedSkin.Contains(skin))
			{
				playerData.unlockedSkin.Add(skin);
			}
		}
	}
}
