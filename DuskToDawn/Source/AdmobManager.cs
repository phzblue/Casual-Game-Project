using System;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdmobManager : MonoBehaviour
{
	private InterstitialAd interstitial;
	private RewardedAd rewardedAd;
	public BannerView bannerView;
	bool completeAds = false;

	public void Start()
	{

#if UNITY_ANDROID
		string appId = "ca-app-pub-7937728858076640~3248784252";
#elif UNITY_IPHONE
        string appId = "ca-app-pub-7937728858076640~1716210738";
#else
        string appId = "unexpected_platform";
#endif

		MobileAds.SetiOSAppPauseOnBackground(true);

		// Initialize the Google Mobile Ads SDK.
		MobileAds.Initialize(appId);

		this.RequestInterstitial();
		this.CreateAndLoadRewardedAd();
		//this.RequestBanner();
	}

	private AdRequest CreateAdRequest()
	{
		return new AdRequest.Builder()
			.Build();
	}

	private void RequestInterstitial()
	{
		// These ad units are configured to always serve test ads.
#if UNITY_EDITOR
		string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-7937728858076640/8324060072";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-7937728858076640/3941961931";
#else
        string adUnitId = "unexpected_platform";
#endif

		// Clean up interstitial ad before creating a new one.
		if (this.interstitial != null)
		{
			this.interstitial.Destroy();
		}

		// Create an interstitial.
		this.interstitial = new InterstitialAd(adUnitId);

		// Register for ad events.
		this.interstitial.OnAdLoaded += this.HandleInterstitialLoaded;
		this.interstitial.OnAdFailedToLoad += this.HandleInterstitialFailedToLoad;
		this.interstitial.OnAdOpening += this.HandleInterstitialOpened;
		this.interstitial.OnAdClosed += this.HandleInterstitialClosed;
		this.interstitial.OnAdLeavingApplication += this.HandleInterstitialLeftApplication;

		// Load an interstitial ad.
		this.interstitial.LoadAd(this.CreateAdRequest());
	}

	public void CreateAndLoadRewardedAd()
	{
#if UNITY_EDITOR
		string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-7937728858076640/4312839519";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-7937728858076640/1660370411";
#else
        string adUnitId = "unexpected_platform";
#endif
		// Create new rewarded ad instance.
		this.rewardedAd = new RewardedAd(adUnitId);

		// Called when an ad request has successfully loaded.
		this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
		// Called when an ad request failed to load.
		this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
		// Called when an ad is shown.
		this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
		// Called when an ad request failed to show.
		this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
		// Called when the user should be rewarded for interacting with the ad.
		this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
		// Called when the ad is closed.
		this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

		this.rewardedAd.LoadAd(this.CreateAdRequest());
	}

	public void RequestBanner()
	{

		Debug.Log("Request Banner ads now ==============================");
#if UNITY_ANDROID
		string adUnitId = "ca-app-pub-7937728858076640/7401806740"; // ca-app-pub-3940256099942544/6300978111
#elif UNITY_IPHONE
      string adUnitId = "ca-app-pub-7937728858076640/4291168491";
#else
      string adUnitId = "unexpected_platform";
#endif

		// Create a 320x50 banner at the top of the screen.
		bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);
		bannerView.Hide();

		// Called when an ad request has successfully loaded.
		bannerView.OnAdLoaded += HandleOnAdLoaded;
		// Called when an ad request failed to load.
		bannerView.OnAdFailedToLoad += HandleOnAdFailedToLoad;
		// Called when an ad is clicked.
		bannerView.OnAdOpening += HandleOnAdOpened;
		// Called when the user returned from the app after an ad click.
		bannerView.OnAdClosed += HandleOnAdClosed;
		// Called when the ad click caused the user to leave the application.
		bannerView.OnAdLeavingApplication += HandleOnAdLeavingApplication;

		// Load the banner with the request.
		bannerView.LoadAd(this.CreateAdRequest());
		
	}

	public void ShowInterstitial()
	{
		if (IsInterstitialAdReady())
		{
			GameManager.instance.AFTrackRichEvent("InterstitialAds", null, 0, this.interstitial.MediationAdapterClassName());
			this.interstitial.Show();
		}
	}

	public void ShowRewardedAd()
	{
		if (IsRewardedAdReady())
		{
			GameManager.instance.AFTrackRichEvent("RewardAds", null, 0, this.rewardedAd.MediationAdapterClassName());
			this.rewardedAd.Show();
		}
		else
		{
			MonoBehaviour.print("Rewarded ad is not ready yet");
		}
	}

	public bool IsInterstitialAdReady()
	{
		return this.interstitial != null && this.interstitial.IsLoaded();
	}

	public bool IsRewardedAdReady()
	{
		return this.rewardedAd != null && this.rewardedAd.IsLoaded();
	}

	#region Interstitial callback handlers

	public void HandleInterstitialLoaded(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleInterstitialLoaded event received");
	}

	public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		Invoke("RequestInterstitial", 10);

		//MonoBehaviour.print(
		//"HandleInterstitialFailedToLoad event received with message: " + args.Message);
	}

	public void HandleInterstitialOpened(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleInterstitialOpened event received");
	}

	public void HandleInterstitialClosed(object sender, EventArgs args)
	{
		RequestInterstitial();

		MonoBehaviour.print("HandleInterstitialClosed event received");
	}

	public void HandleInterstitialLeftApplication(object sender, EventArgs args)
	{
		GameManager.instance.AFTrackRichEvent("InterstitialAdsClick", null, 0, this.interstitial.MediationAdapterClassName());
		MonoBehaviour.print("HandleInterstitialLeftApplication event received");
	}

	#endregion

	#region RewardedAd callback handlers

	public void HandleRewardedAdLoaded(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleRewardedAdLoaded event received");
	}

	public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
	{
		Invoke("CreateAndLoadRewardedAd", 10);

		//MonoBehaviour.print(
		//	"HandleRewardedAdFailedToLoad event received with message: " + args.Message);
	}

	public void HandleRewardedAdOpening(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleRewardedAdOpening event received");
	}

	public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
	{
		//MonoBehaviour.print(
		//	"HandleRewardedAdFailedToShow event received with message: " + args.Message);
	}

	public void HandleRewardedAdClosed(object sender, EventArgs args)
	{
		CreateAndLoadRewardedAd();

		MonoBehaviour.print("HandleRewardedAdClosed event received");

		if (completeAds)
		{
			completeAds = false;
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

	public void HandleUserEarnedReward(object sender, Reward args)
	{
		completeAds = true;

		string type = args.Type;
		double amount = args.Amount;
		MonoBehaviour.print(
			"HandleRewardedAdRewarded event received for "
						+ amount.ToString() + " " + type);
	}

	#endregion

	#region BannerAds callback handlers
	public void HandleOnAdLoaded(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleAdLoaded banner event received");
	}

	public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		//Invoke("RequestBanner", 10);
		MonoBehaviour.print("HandleFailedToReceiveAd banner event received with message: "
							+ args.Message);
	}

	public void HandleOnAdOpened(object sender, EventArgs args)
	{
		GameManager.instance.AFTrackRichEvent("BannerClick", null, 0, this.bannerView.MediationAdapterClassName());
		MonoBehaviour.print("HandleAdOpened banner event received");
	}

	public void HandleOnAdClosed(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleAdClosed banner event received");
	}

	public void HandleOnAdLeavingApplication(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleAdLeavingApplication banner event received");
	}
	#endregion
}