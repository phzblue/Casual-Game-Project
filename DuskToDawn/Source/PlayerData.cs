using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
	public int playerGachaNum = 1;
	public int playerCoin = 0;
	public int adsWatched = 0;
	public int matchPlayed = 0;
	public int highScore = 0;
	public int lastMileStone = 0;
	public int leftRunnerID = 1;
	public int rightRunnerID = 1;

	public bool purchasedNoAds = false;
	public bool hasShare = false;

	public List<int> skinIDContain = new List<int>() {};
	public List<int> newCharacterIDList = new List<int>();

	public PlayerData(){}

	public void SaveData()
	{
		string playerDataJson = Encoder.jsonEncode(this);

		PlayerPrefs.SetString(SystemInfo.deviceUniqueIdentifier + "_playerData", playerDataJson);
	}

	public void AddDefaultSkinNum()
	{
		skinIDContain.Add(0);
		skinIDContain.Add(1);
	}

	public int NumGachaSkinOwn()
	{
		int num = 0;
		foreach(int i in skinIDContain)
		{
			if (GameConfig.skinList[i].skinCategory == SkinDataManager.SkinCategory.PURCHASE)
			{
				num++;
			}
		}

		return num;
	}
}
