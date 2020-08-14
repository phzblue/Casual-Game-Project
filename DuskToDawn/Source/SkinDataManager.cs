using I2.Loc;

public class SkinDataManager
{
	public int skinID = 0;
	public int folderID = 100;
	public string skinName = null;
	public string skinDesc = null;
	public string storeID = "";

	public SkinCategory skinCategory = SkinCategory.DEFAULT;
	public int categoryDetail = 0;

	public SkinDataManager()
	{
	}

	public string GetConditionString()
	{
		switch (skinCategory)
		{
			case SkinCategory.PURCHASE:
			case SkinCategory.DEFAULT:
			default:
				return "";
			case SkinCategory.SCORE:
                return string.Format(LocalizedString.GetString("reachTo"), GameConfig.scoredSkin[categoryDetail].Key);
			case SkinCategory.ADS:
                return string.Format(LocalizedString.GetString("watchAdTo"), GameConfig.adsUnlockedSkin[categoryDetail].Key -
                    GameManager.instance.playerData.adsWatched);
			case SkinCategory.MATCH:
                return string.Format(LocalizedString.GetString("playMatchTo"), GameConfig.playedSkin[categoryDetail].Key -
                    GameManager.instance.playerData.matchPlayed);
		}
	}

	public string GetUnlockedString()
	{
		switch (skinCategory)
		{
			case SkinCategory.PURCHASE:
			case SkinCategory.DEFAULT:
			default:
				return ""; 
			case SkinCategory.SCORE:
                return string.Format(LocalizedString.GetString("reachedScore"),GameConfig.scoredSkin[categoryDetail].Key);
			case SkinCategory.ADS:
                return string.Format(LocalizedString.GetString("watchedAd"), GameConfig.adsUnlockedSkin[categoryDetail].Key);
			case SkinCategory.MATCH:
                return string.Format(LocalizedString.GetString("playedMatch"), GameConfig.playedSkin[categoryDetail].Key);
		}
	}

	public enum SkinCategory
	{
		DEFAULT,
		SCORE,
		ADS,
		MATCH,
		PURCHASE
	}
}
