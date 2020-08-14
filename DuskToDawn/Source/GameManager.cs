using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using I2.Loc;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
#if UNITY_WEBGL
using System.Runtime.InteropServices;
#endif

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	[SerializeField] AudioSource bgm = null;
	[SerializeField] string googlePublicKey;

	public AdmobManager admobManager = null;
	public UnityAds unityAdsManager = null;
	public GamePreference gamePreference;
	public PlayerData playerData;

	public string rewardAdsType = "";
	public int gameNumPerSession = 0;
	public float sessionStartTime = 0;
	public bool isPlayable = false;
	public bool powerupReset = false;

	public int tempRanRunnerIDL = 1;
	public int tempRanRunnerIDR = 1;

	public int bgIndex = 0;

	void Awake()
	{
		if (instance == null) instance = this;
		else if (instance != this) Destroy(gameObject);

#if UNITY_IOS
		UnityEngine.iOS.NotificationServices.RegisterForNotifications(
			UnityEngine.iOS.NotificationType.Alert | 
			UnityEngine.iOS.NotificationType.Badge | 
			UnityEngine.iOS.NotificationType.Sound);
#elif UNITY_ANDROID
		AndroidNotificationChannel c = new AndroidNotificationChannel()
		{
			Id = "channel_id",
			Name = "Default Channel",
			Importance = Importance.High,
			Description = "Generic notifications",
		};
		AndroidNotificationCenter.RegisterNotificationChannel(c);
#endif
	}

	private void Start()
	{
		DontDestroyOnLoad(this);
		LoadPlayerData();
#if !UNITY_WEBGL
		Application.targetFrameRate = 300;
#endif
		InitAppsFlyer();

		PlayBGM();
	}

	public void PlayBGM()
	{
		bgm.Play();
	}

	private void Update()
	{
		if (sessionStartTime <= 300f)
		{
			sessionStartTime += Time.deltaTime;
		}
	}

	private void LoadPlayerData()
	{
		if (PlayerPrefs.HasKey(SystemInfo.deviceUniqueIdentifier + "_playerData"))
		{
			playerData = Encoder.jsonDecode<PlayerData>(PlayerPrefs.GetString(SystemInfo.deviceUniqueIdentifier + "_playerData"));
		}
		else
		{
			playerData = new PlayerData();
			playerData.AddDefaultSkinNum();
			playerData.SaveData();
		}
	}

	private void InitAppsFlyer()
	{
		/* Mandatory - set your AppsFlyer’s Developer key. */
		AppsFlyer.setAppsFlyerKey("BdP724hHJFraJaxKXvNex7");
		/* For detailed logging */
		//AppsFlyer.setIsDebug (true);

#if UNITY_IOS
		/* Mandatory - set your apple app ID
		NOTE: You should enter the number only and not the "ID" prefix */
		AppsFlyer.setAppID ("1472690984");
		AppsFlyer.getConversionData();
		AppsFlyer.trackAppLaunch ();
#elif UNITY_ANDROID
		/* Mandatory - set your Android package name */
		AppsFlyer.setAppID("net.gogame.dusktodawn");
		/* For getting the conversion data in Android, you need to add the "AppsFlyerTrackerCallbacks" listener.*/
		AppsFlyer.init("BdP724hHJFraJaxKXvNex7", "AppsFlyerTrackerCallbacks");

		AppsFlyer.createValidateInAppListener("AppsFlyerTrackerCallbacks", "onInAppBillingSuccess", "onInAppBillingFailure");

#endif
		AFTrackRichEvent("app_open");
	}

	public void AFTrackRichEvent(string eventName, Product product = null, long score = 0, string adType = "")
	{
		Debug.Log("af_event: " + eventName + "===========================================");
		Dictionary<string, string> afEvent = new Dictionary<string, string>();

		switch (eventName)
		{
			case "app_open":
				afEvent.Add("login", 1.ToString());
				break;
			//case "af_purchase":


			//	afEvent.Add("af_cotent_id", product.definition.id);
			//	afEvent.Add("af_currency", product.metadata.isoCurrencyCode);
			//	afEvent.Add("af_revenue", product.metadata.localizedPrice.ToString());
			//	afEvent.Add("af_order_id", product.transactionID);
			//	afEvent.Add("af_store", product.definition.storeSpecificId);
			//	break;
			case "af_tutorial_completion":
				break;
			case "BannerClick":
				eventName = "af_ad_click_admob";

				afEvent.Add("adProvider", adType);
				afEvent.Add("adType", "RewardedVideo");
				break;
			case "InterstitialAdsClick":
				eventName = "af_ad_click_admob";

				afEvent.Add("adProvider", adType);
				afEvent.Add("adType", "Interstitial");
				break;
			case "RewardAds":
				eventName = "af_ad_view_admob";

				afEvent.Add("adProvider", adType);
				afEvent.Add("adType", "RewardedVideo");
				afEvent.Add("adsLocation", rewardAdsType);
				break;
			case "InterstitialAds":
				eventName = "af_ad_view_admob";
				afEvent.Add("adProvider", adType);
				afEvent.Add("adType", "Interstitial");
				afEvent.Add("adsLocation", "MenuTitle");
				break;
			case "session_5mins":
				afEvent.Add("game_played", gameNumPerSession.ToString());
				break;
			case "charUnlock":
				switch (playerData.skinIDContain.Count)
				{
					case 5:
						eventName = "chracter_unlocked_5";
						break;
					case 10:
						eventName = "chracter_unlocked_10";
						break;
					case 15:
						eventName = "chracter_unlocked_15";
						break;
					case 20:
						eventName = "chracter_unlocked_20";
						break;
				}
				break;
			case "sendScore":
				if (score >= 5000)
				{
					eventName = "reach_score_5000";
				}
				else if (score >= 2000)
				{
					eventName = "reach_score_2000";
				}
				else if (score >= 1000)
				{
					eventName = "reach_score_1000";
				}
				else if (score >= 500)
				{
					eventName = "reach_score_500";
				}
				else if (score >= 300)
				{
					eventName = "reach_score_300";
				}
				else
				{
					eventName = "reach_score_less_300";
				}
				break;
		}
		AppsFlyer.trackRichEvent(eventName, afEvent);
	}

	public PurchaseProcessingResult ProcessPurchase(Product e, int skinID = -1)
	{
		bool validPurchase = true; // Presume valid for platforms with no R.V.

		// Unity IAP's validation logic is only included on these platforms.
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_TVOS
		// Prepare the validator with the secrets we prepared in the Editor
		// obfuscation window.
		var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
			AppleTangle.Data(), Application.identifier);

		try
		{
			// On Google Play, result will have a single product Id.
			// On Apple stores receipts contain multiple products.
			var result = validator.Validate(e.receipt);
			Debug.Log("Receipt is valid. Contents:");
			foreach (IPurchaseReceipt productReceipt in result)
			{
				Debug.Log(productReceipt.productID);
				Debug.Log(productReceipt.purchaseDate);
				Debug.Log(productReceipt.transactionID);
			}
		}
		catch (IAPSecurityException)
		{
			Debug.Log("Invalid receipt, not unlocking content");
			validPurchase = false;
		}


#endif
		if (validPurchase)
		{
			// Unlock the appropriate content here.
#if UNITY_ANDROID
			//(string publicKey, string purchaseData, string signature, string price, string currency

			Dictionary<string, object> wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(e.receipt);

			if(wrapper == null)
				throw new InvalidReceiptDataException();

			string payload = (string)wrapper["Payload"];
			var gpDetails = (Dictionary<string, object>)MiniJson.JsonDecode(payload);
			string gpJson = (string)gpDetails["json"];
			string gpSig = (string)gpDetails["signature"];

			AppsFlyer.validateReceipt(googlePublicKey, gpJson, gpSig, e.metadata.localizedPriceString, e.metadata.isoCurrencyCode, new Dictionary<string, string>());
			
#elif UNITY_IOS
			if (Debug.isDebugBuild) {
				AppsFlyer.setIsSandbox(true);
			}
			AppsFlyer.validateReceipt(e.definition.id, e.metadata.localizedPriceString, e.metadata.isoCurrencyCode, e.transactionID, new Dictionary<string, string>());
#endif
		
			if (skinID != -1)
			{
				GameObject.FindObjectOfType<SkinSceneManager>().AddSkinIDToPlayer(skinID);
				skinID = 0;
			}else{
				FindObjectOfType<TitleSceneManager>().AddNoAdsPurchase();
			}
		}

		return PurchaseProcessingResult.Complete;
	}


	public bool CheckGachaAvailability()
	{
		bool canGacha;
		bool enoughGold;
		if (GameConfig.gachaCost.ContainsKey(playerData.playerGachaNum))
		{
			enoughGold = playerData.playerCoin >= GameConfig.gachaCost[playerData.playerGachaNum];
		}
		else
		{
			enoughGold = playerData.playerCoin >= GameConfig.gachaCost.Keys[GameConfig.gachaCost.Count - 1];
		}

		canGacha = (enoughGold && playerData.skinIDContain.Count - 1 < GameConfig.skinList.Count - 1) ||
			playerData.newCharacterIDList.Count > 0;

		return (canGacha && playerData.NumGachaSkinOwn() < GameConfig.gachaSkinNum);
	}

	public void ShowRewardedAds(string type)
	{
		this.rewardAdsType = type;
#if UNITY_WEBGL
		PlayAdsenseAds();
#else
		if (unityAdsManager.IsUnityRewardAdsReady())
		{
			Debug.Log("show unity reward ads =================================== ");

			unityAdsManager.DisplayRewardedAds();
		}
		else
		{
			Debug.Log("show admob reward ads =================================== ");

			admobManager.ShowRewardedAd();
		}
#endif
	}

	public bool IsAdsReady()
	{
		Debug.Log("admobManager.IsRewardedAdReady() =================================== " + admobManager.IsRewardedAdReady());
		Debug.Log("unityAdsManager.IsUnityRewardAdsReady() =================================== " + unityAdsManager.IsUnityRewardAdsReady());
		return admobManager.IsRewardedAdReady() || unityAdsManager.IsUnityRewardAdsReady();
	}

	public void ToggleBGM(bool isOn)
	{
		if (isOn)
		{
			bgm.Play();
		}
		else
		{
			bgm.Stop();
		}
	}

	public void ShowInterstitialAds()
	{
#if UNITY_WEBGL
		PlayAdsenseAds();
#else
		if (!playerData.purchasedNoAds)
		{
			if (unityAdsManager.IsUnityInterstitialAdsReady())
			{
				Debug.Log("show unity interstitial ads =================================== ");
				unityAdsManager.DisplayInterstitialAds();
			}
			else
			{
				Debug.Log("show admob interstitial ads =================================== ");

				admobManager.Invoke("ShowInterstitial", 0.5f);
			}
		}
#endif
	}

	private void SendLocalNotification()
	{

#if UNITY_IOS
		UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();

		List<UnityEngine.iOS.LocalNotification> notifications = new List<UnityEngine.iOS.LocalNotification>();

		for(int i=0; i<5; i++)
		{

			UnityEngine.iOS.LocalNotification __notification = new UnityEngine.iOS.LocalNotification();
			__notification.alertAction = LocalizedString.GetString("notificationTitle" + (i + 1));
			__notification.alertBody = LocalizedString.GetString("notificationDesc" + (i + 1));
			notifications.Add(__notification);
		}

		int seed;
		for (int i=0; i<5; i++)
		{
			seed = Random.Range(0, notifications.Count);

			switch (i)
			{
				case 0:
					notifications[seed].fireDate = System.DateTime.Now.AddDays(1);
					break;
				case 1:
					notifications[seed].fireDate = System.DateTime.Now.AddDays(5);
					break;
				case 2:
					notifications[seed].fireDate = System.DateTime.Now.AddDays(10);
					break;
				case 3:
					notifications[seed].fireDate = System.DateTime.Now.AddDays(14);
					break;
				case 4:
					notifications[seed].fireDate = System.DateTime.Now.AddDays(20);
					break;
				
			}

			UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(notifications[seed]);
			notifications.RemoveAt(seed); //D1, D5, D10, D14
		}
		
#elif UNITY_ANDROID

		int seed;
		List<int> mailNum = new List<int>
		{
			1,2,3,4,5
		};

		for (int i = 0; i < 5; i++)
		{
			seed = Random.Range(0, mailNum.Count);
			int mailId = mailNum[seed];

			string title = LocalizedString.GetString("notificationTitle" + mailId);
			string body = LocalizedString.GetString("notificationDesc" + mailId);

			AndroidNotification notification = new AndroidNotification();
			notification.Title = title;
			notification.Text = body;
			switch (i)
			{
				case 0:
					notification.FireTime = System.DateTime.Now.AddDays(1);
					break;
				case 1:
					notification.FireTime = System.DateTime.Now.AddDays(5);
					break;
				case 2:
					notification.FireTime = System.DateTime.Now.AddDays(10);
					break;
				case 3:
					notification.FireTime = System.DateTime.Now.AddDays(14);
					break;
				case 4:
					notification.FireTime = System.DateTime.Now.AddDays(20);
					break;
				
			}

			AndroidNotificationCenter.SendNotification(notification, "channel_id");

			mailNum.RemoveAt(seed); //D1, D5, D10, D14

		}
#endif
	}

	private void OnApplicationPause(bool pause)
	{

		if (!pause)
		{
#if UNITY_IOS
			UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
#elif UNITY_ANDROID
			AndroidNotificationCenter.CancelAllNotifications();
#endif
		}
		else
		{
			SendLocalNotification();
		}

		if (GameObject.FindObjectOfType<GameSceneManager>() != null && pause &&
            FindObjectOfType<GameSceneManager>().isPlayable)
        {
            GameObject.FindObjectOfType<GameSceneManager>().ShowPauseMenu();
		}

		if(sessionStartTime > 10)
		{
			AFTrackRichEvent("session_5mins");
			sessionStartTime = 0;
			gameNumPerSession = 0;
		}

	}

	private void OnApplicationQuit()
    {
        AFTrackRichEvent("session_5mins");
        sessionStartTime = 0;
        gameNumPerSession = 0;
	}

#if UNITY_WEBGL
	[DllImport("__Internal")]
	private static extern void _playAds();

	public void PlayAdsenseAds()
	{
		_playAds();
	}

	public void _onWebGLAdsFinish()
	{
		playerData.adsWatched++;
		switch (rewardAdsType)
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

		rewardAdsType = "";
	}
#endif
}

