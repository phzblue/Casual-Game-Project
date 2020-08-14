using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAds : MonoBehaviour, IUnityAdsListener
{
#if UNITY_IOS
    private string gameId = "3198622";
#elif UNITY_ANDROID
	private string gameId = "3198623";
#endif
	string myPlacementId = "rewardedVideo";
	bool testMode = false;

	// Start is called before the first frame update
	void Start()
    {
		Advertisement.AddListener(this);
		Advertisement.Initialize(gameId, testMode);
	}

	public bool IsUnityInterstitialAdsReady()
	{
		return Advertisement.IsReady("video");
	}

	public bool IsUnityRewardAdsReady()
	{
		return Advertisement.IsReady(myPlacementId);
	}

	public void DisplayInterstitialAds()
	{
		if (Advertisement.IsReady("video"))
		{
			GameManager.instance.AFTrackRichEvent("InterstitialAds", null, 0, "UnityAds");

			Advertisement.Show("video");
		}
	}

	public void DisplayRewardedAds()
	{
		if (Advertisement.IsReady(myPlacementId))
		{
			GameManager.instance.AFTrackRichEvent("RewardAds", null, 0, "UnityAds");
			Advertisement.Show(myPlacementId);
		}
	}

	// Implement IUnityAdsListener interface methods:
	public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
	{
		Advertisement.Load(placementId);
		// Define conditional logic for each ad completion status:
		if (showResult == ShowResult.Finished)
		{
			if (placementId == myPlacementId)
			{

				GameManager.instance.playerData.adsWatched++;
				switch (GameManager.instance.rewardAdsType)
				{
					case "FreeRoll":
						GameObject.FindObjectOfType<GachaSceneManager>().OnAdsFinish();
						break;
					case "FreeGold":
						GameObject.FindObjectOfType<GameSceneManager>().OnAdsFinishCoin();
						break;
					case "FreeHeart":
						GameObject.FindObjectOfType<GameSceneManager>().OnAdsFinishHeart();
						break;
				}

				GameManager.instance.rewardAdsType = "";
			}
		}
		else if (showResult == ShowResult.Skipped)
		{
			// Do not reward the user for skipping the ad.
		}
		else if (showResult == ShowResult.Failed)
		{
		}
	}

	public void OnUnityAdsReady(string placementId)
	{
	}

	public void OnUnityAdsDidError(string message)
	{
		// Log the error.
	}

	public void OnUnityAdsDidStart(string placementId)
	{
		//send appsflyer here tracking
	}
}
