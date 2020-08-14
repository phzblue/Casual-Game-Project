using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
	public int playerCoin = 0;
	public int adsWatched = 0;
	public int matchPlayed = 0;
	public int gachaNum = 1;

	public string chosenSkin = "Starzinger";

	public bool purchasedNoAds = false;
	public bool hasShare = false;
	public bool isBGMOn = true;
	public bool isSFXOn = true;
	public bool isVibrationOn = true;

	public List<string> unlockedSkin = new List<string>();
	public List<string> skinPurchased = new List<string>() {
		"Starzinger"
	};

	public PlayerData(){}

	public void SaveData()
	{
		string playerDataJson = Encoder.jsonEncode(this);

		PlayerPrefs.SetString(SystemInfo.deviceUniqueIdentifier + "_playerData_galaxy", playerDataJson);
	}
}
