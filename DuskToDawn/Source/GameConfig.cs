using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;

public static class GameConfig
{
	public static SortedList<int, int> gachaCost = new SortedList<int, int>
	{
		{ 1, 50 }, { 2, 200 }, { 3, 200 }, { 4, 200 },
		{ 5, 200 }, { 6, 500 }, { 7, 500 }, { 8, 500 },
		{ 9, 500 }, { 10, 500 }, { 11, 1000 },
		{ 12, 1000 }, { 13, 1000 }, { 14, 1000 },
		{ 15, 1000 }, { 16, 2000 }, { 17, 2000 },
		{ 18, 2000 }, { 19, 2000 }, { 20, 2000 },
		{ 21, 3000 }

		//{ 1, 1 }
	};

	public static Dictionary<int, KeyValuePair<int, int>> adsUnlockedSkin = new Dictionary<int, KeyValuePair<int, int>>
	{
		{ 1, new KeyValuePair<int, int>(1, 19) },
		{ 2, new KeyValuePair<int, int>(5, 33) },
		{ 3, new KeyValuePair<int, int>(10, 23) },
		{ 4, new KeyValuePair<int, int>(50, 13) }
		//{ 5, new KeyValuePair<int, int>(100, 29) }
	};
	public static Dictionary<int, KeyValuePair<int, int>> scoredSkin = new Dictionary<int, KeyValuePair<int, int>>
	{
		{ 1, new KeyValuePair<int, int>(200, 2) },
		{ 2, new KeyValuePair<int, int>(500, 3) },
		{ 3, new KeyValuePair<int, int>(1000, 10) },
		{ 4, new KeyValuePair<int, int>(1500, 29) }
		//{ 5, new KeyValuePair<int, int>(2000, 0) }

	};
	public static Dictionary<int, KeyValuePair<int, int>> playedSkin = new Dictionary<int, KeyValuePair<int, int>>
	{
		{ 1, new KeyValuePair<int, int>(10, 35) },
		{ 2, new KeyValuePair<int, int>(20, 8) },
		{ 3, new KeyValuePair<int, int>(50, 25) },
		{ 4, new KeyValuePair<int, int>(100, 6) }
		//{ 5, new KeyValuePair<int, int>(150, 0) }
	};

	public static int gachaSkinNum = 24;
	public static List<SkinDataManager> skinList = Encoder.jsonDecode<List<SkinDataManager>>("[{\"skinID\":0,\"skinName\":\"random\",\"skinDesc\":\"randomPick\",\"skinCategory\":\"DEFAULT\"},{\"skinID\":1,\"skinCategory\":\"DEFAULT\"},{\"skinID\":2,\"skinCategory\":\"SCORE\",\"categoryDetail\":1},{\"skinID\":3,\"skinCategory\":\"SCORE\",\"categoryDetail\":2},{\"skinID\":4,\"skinCategory\":\"PURCHASE\",\"storeID\":\"net.gogame.duskanddawn.04sagecrow\"},{\"skinID\":5,\"skinCategory\":\"PURCHASE\",\"storeID\":\"net.gogame.duskanddawn.05crowwitch\"},{\"skinID\":6,\"skinCategory\":\"MATCH\",\"categoryDetail\":4},{\"skinID\":7,\"skinCategory\":\"PURCHASE\",\"storeID\":\"net.gogame.duskanddawn.05crowwitch\"},{\"skinID\":8,\"skinCategory\":\"MATCH\",\"categoryDetail\":2},{\"skinID\":9,\"skinCategory\":\"PURCHASE\",\"storeID\":\"net.gogame.duskanddawn.09angel\"},{\"skinID\":10,\"skinCategory\":\"SCORE\",\"categoryDetail\":3},{\"skinID\":11,\"skinCategory\":\"PURCHASE\",\"storeID\":\"net.gogame.duskanddawn.11furypoca\"},{\"skinID\":12,\"skinCategory\":\"PURCHASE\",\"storeID\":\"net.gogame.duskanddawn.12tribalhare\"},{\"skinID\":13,\"skinCategory\":\"ADS\",\"categoryDetail\":4},{\"skinID\":14,\"skinCategory\":\"PURCHASE\",\"storeID\":\"net.gogame.duskanddawn.14unicorn\"},{\"skinID\":15,\"skinCategory\":\"PURCHASE\",\"storeID\":\"net.gogame.duskanddawn.15redjetpack\"},{\"skinID\":16,\"skinCategory\":\"PURCHASE\",\"storeID\":\"net.gogame.duskanddawn.16greenjetpack\"},{\"skinID\":17,\"skinCategory\":\"PURCHASE\",\"storeID\":\"net.gogame.duskanddawn.17bluejetpack\"},{\"skinID\":18,\"skinCategory\":\"PURCHASE\",\"storeID\":\"net.gogame.duskanddawn.18yellowjetpack\"},{\"skinID\":19,\"skinCategory\":\"ADS\",\"categoryDetail\":1},{\"skinID\":20,\"skinCategory\":\"PURCHASE\",\"storeID\":\"net.gogame.duskanddawn.20blackjetpack\"},{\"skinID\":21,\"skinCategory\":\"PURCHASE\",\"storeID\":\"net.gogame.duskanddawn.21whitejetpack\"},{\"skinID\":22,\"skinCategory\":\"PURCHASE\",\"storeID\":\"net.gogame.duskanddawn.22sneakythief\"},{\"skinID\":23,\"skinCategory\":\"ADS\",\"categoryDetail\":3},{\"skinID\":24,\"skinCategory\":\"PURCHASE\",\"storeID\":\"net.gogame.duskanddawn.24superhuman\"},{\"skinID\":25,\"skinCategory\":\"MATCH\",\"categoryDetail\":3},{\"skinID\":26,\"skinCategory\":\"PURCHASE\",\"storeID\":\"net.gogame.duskanddawn.26blackinsect\"},{\"skinID\":27,\"skinCategory\":\"PURCHASE\",\"storeID\":\"net.gogame.duskanddawn.27whiteinsect\"},{\"skinID\":28,\"skinCategory\":\"PURCHASE\",\"storeID\":\"net.gogame.duskanddawn.28neonhero\"},{\"skinID\":29,\"skinCategory\":\"SCORE\",\"categoryDetail\":4},{\"skinID\":30,\"skinCategory\":\"PURCHASE\",\"storeID\":\"net.gogame.duskanddawn.30worthyrunner\"},{\"skinID\":31,\"skinCategory\":\"PURCHASE\",\"storeID\":\"net.gogame.duskanddawn.31youngrunner\"},{\"skinID\":32,\"skinCategory\":\"PURCHASE\",\"storeID\":\"net.gogame.duskanddawn.32steelman\"},{\"skinID\":33,\"skinCategory\":\"ADS\",\"categoryDetail\":2},{\"skinID\":34,\"skinCategory\":\"PURCHASE\",\"storeID\":\"net.gogame.duskanddawn.34bluemaskrunner\"},{\"skinID\":35,\"skinCategory\":\"MATCH\",\"categoryDetail\":1},{\"skinID\":36,\"skinCategory\":\"PURCHASE\",\"storeID\":\"net.gogame.duskanddawn.36orangemaskrunner\"},{\"skinID\":37,\"skinCategory\":\"PURCHASE\",\"storeID\":\"net.gogame.duskanddawn.37purplemaskrunner\"}]");

	public static void CheckFreeSkin(int score = 0)
	{
		CheckFreeSkinFromAdsWatched();

		CheckFreeSkinFromMatchPlayed();

		CheckFreeSkinFromScore(score);

		GameManager.instance.playerData.SaveData();
	}

	public static string CheckFreeSkinFromAdsWatched()
	{
		PlayerData playerData = GameManager.instance.playerData;

		//check for free skin from watched ads <level, <criteria, skin id>>
		foreach (KeyValuePair<int, int> detail in adsUnlockedSkin.Values)
		{
			if (playerData.adsWatched == detail.Key && !playerData.skinIDContain.Contains(detail.Value)
				&& !playerData.newCharacterIDList.Contains(detail.Value))
			{
				playerData.newCharacterIDList.Add(detail.Value);
                
				return string.Format(LocalizedString.GetString("watchedAd"), detail.Key.ToString());

            }
		}

		return "";
	}

	public static string CheckFreeSkinFromMatchPlayed()
	{
		PlayerData playerData = GameManager.instance.playerData;

		//check for free skin from match played <level, <criteria, skin id>>
		foreach (KeyValuePair<int, int> detail in playedSkin.Values)
		{
			if (playerData.matchPlayed == detail.Key && !playerData.skinIDContain.Contains(detail.Value)
				&& !playerData.newCharacterIDList.Contains(detail.Value))
			{
				playerData.newCharacterIDList.Add(detail.Value);
                return string.Format(LocalizedString.GetString("playedMatch"), detail.Key.ToString());
			}
		}

		return "";
	}

	public static string CheckFreeSkinFromScore(int score)
	{
		PlayerData playerData = GameManager.instance.playerData;

		//check for free skin from highScore <level, <criteria, skin id>>
		foreach (KeyValuePair<int, int> detail in scoredSkin.Values)
		{
			if (score >= detail.Key && !playerData.skinIDContain.Contains(detail.Value)
				&& !playerData.newCharacterIDList.Contains(detail.Value))
			{
				playerData.newCharacterIDList.Add(detail.Value);

                return string.Format(LocalizedString.GetString("reachedScore"), score.ToString());
			}
		}

		return "";
	}


}
