using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Purchasing;
using I2.Loc;
//using Facebook.Unity;
#if UNITY_ANDROID
using Unity.Notifications.Android;
using UnityEngine.Android;
#endif

public class GameManager : MonoBehaviour
{
	[SerializeField] UnityAds unityAds = null;
	[SerializeField] AdMobAds admobAds = null;
	[SerializeField] GameObject banner = null;

	Firebase.FirebaseApp app;

	public Dictionary<string, ShipConfig> shipConfigMap = new Dictionary<string, ShipConfig>();

	public static GameManager instance;
	public string rewardType;
	public string playerName = "";
	public PlayerData playerData;

	public bool levelStart = true;
	public bool isFever = false;
	public bool noInternet = false;
	public bool isPurchasing = false;

	public int finalScore = 0;
	public int lastLevel = 1;
	public float adsTimer = 0;

	string filePath;

	private void Awake()
	{
		if (instance == null) instance = this;
		else if (instance != this) Destroy(gameObject);

		LoadPlayerData();

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
		Time.timeScale = 1f;
		DontDestroyOnLoad(this);
		SoundManager.instance.Refresh();

		Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
			var dependencyStatus = task.Result;
			if (dependencyStatus == Firebase.DependencyStatus.Available)
			{
				// Create and hold a reference to your FirebaseApp,
				// where app is a Firebase.FirebaseApp property of your application class.
				app = Firebase.FirebaseApp.DefaultInstance;

				// Set a flag here to indicate whether Firebase is ready to use by your app.
			}
			else
			{
				UnityEngine.Debug.LogError(System.String.Format(
				  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
				// Firebase Unity SDK is not safe to use here.
			}
		});

		InitAppsFlyer();
		//FBSDKInit();

		banner.SetActive(!playerData.purchasedNoAds);
	}

	private void SendLocalNotification()
	{
#if UNITY_IOS
		UnityEngine.iOS.LocalNotification __notification = new UnityEngine.iOS.LocalNotification();
		__notification.alertAction = LocalizedString.GetString("notificationTitle");
		__notification.alertBody = LocalizedString.GetString("notificationDesc");
#elif UNITY_ANDROID
		AndroidNotification __notification = new AndroidNotification();
		__notification.Title = LocalizedString.GetString("notificationTitle");
		__notification.Text = LocalizedString.GetString("notificationDesc");
#endif
		for (int i = 0; i < 5; i++)
		{
			switch (i)
			{
#if UNITY_IOS
				case 0:
					__notification.fireDate = System.DateTime.Now.AddDays(1);
					break;
				case 1:
					__notification.fireDate = System.DateTime.Now.AddDays(5);
					break;
				case 2:
					__notification.fireDate = System.DateTime.Now.AddDays(10);
					break;
				case 3:
					__notification.fireDate = System.DateTime.Now.AddDays(14);
					break;
				case 4:
					__notification.fireDate = System.DateTime.Now.AddDays(20);
					break;
#elif UNITY_ANDROID
				case 0:
					__notification.FireTime = System.DateTime.Now.AddDays(1);
					break;
				case 1:
					__notification.FireTime = System.DateTime.Now.AddDays(5);
					break;
				case 2:
					__notification.FireTime = System.DateTime.Now.AddDays(10);
					break;
				case 3:
					__notification.FireTime = System.DateTime.Now.AddDays(14);
					break;
				case 4:
					__notification.FireTime = System.DateTime.Now.AddDays(20);
					break;
#endif
			}

#if UNITY_IOS
			UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(__notification);
#elif UNITY_ANDROID
			AndroidNotificationCenter.SendNotification(__notification, "channel_id");
#endif
		}
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
	}

	private void FBSDKInit()
	{
		//FB.Init();
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
		AppsFlyer.setAppID ("1486122716 ");
		AppsFlyer.getConversionData();
		AppsFlyer.trackAppLaunch ();
#elif UNITY_ANDROID
		/* Mandatory - set your Android package name */
		AppsFlyer.setAppID("net.gogame.pewpewplanet");
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

		List<string> scoreList = new List<string>();

		switch (eventName)
		{
			case "app_open":
				afEvent.Add("login", 1.ToString());
				break;
			case "af_tutorial_completion":
				break;
			case "af_share":
				break;
			case "match_played":
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
				afEvent.Add("adsLocation", rewardType);
				break;
			case "InterstitialAds":
				eventName = "af_ad_view_admob";
				afEvent.Add("adProvider", adType);
				afEvent.Add("adType", "Interstitial");
				afEvent.Add("adsLocation", "MenuTitle");
				break;
			case "charUnlock":
				switch (playerData.skinPurchased.Count)
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
					scoreList.Add("reach_score_5000");
					scoreList.Add("reach_score_2000");
					scoreList.Add("reach_score_800");
					scoreList.Add("reach_score_200");
					scoreList.Add("reach_score_50");
					scoreList.Add("reach_score_less_50");
				}
				else if (score >= 2000)
				{
					scoreList.Add("reach_score_2000");
					scoreList.Add("reach_score_800");
					scoreList.Add("reach_score_200");
					scoreList.Add("reach_score_50");
					scoreList.Add("reach_score_less_50");
				}
				else if (score >= 800)
				{
					scoreList.Add("reach_score_800");
					scoreList.Add("reach_score_200");
					scoreList.Add("reach_score_50");
					scoreList.Add("reach_score_less_50");
				}
				else if (score >= 200)
				{
					scoreList.Add("reach_score_200");
					scoreList.Add("reach_score_50");
					scoreList.Add("reach_score_less_50");
				}
				else if (score >= 50)
				{
					scoreList.Add("reach_score_50");
					scoreList.Add("reach_score_less_50");
				}
				else
				{
					scoreList.Add("reach_score_less_50");
				}
				break;
		}
		if(scoreList.Count > 0)
		{
			foreach(string child in scoreList)
			{
				AppsFlyer.trackRichEvent(child, afEvent);
			}
		}
		else
		{
			AppsFlyer.trackRichEvent(eventName, afEvent);
		}
	}

	public void CheckBadgeActivity(bool active)
	{
		GameObject.FindGameObjectWithTag("badge").SetActive(active);
	}

	private void FixedUpdate()
	{
		adsTimer += Time.deltaTime;
		CheckInternet();
	}
	
	public bool IsAdsReady()
	{
		return unityAds.IsUnityRewardAdsReady() || admobAds.IsRewardedAdReady();
	}

	public void ShowInterstitialAds()
	{
		if (!playerData.purchasedNoAds && adsTimer >= 200) //
		{
			adsTimer = 0;

			if (unityAds.IsUnityInterstitialAdsReady())
			{
				unityAds.DisplayInterstitialAds();
			}
			else if (admobAds.IsInterstitialAdReady())
			{
				admobAds.ShowInterstitial();
			}
		}
	}

	public void ShowRewardAds()
	{
		if (unityAds.IsUnityRewardAdsReady())
		{
			unityAds.DisplayRewardedAds();
		}
		else if(admobAds.IsRewardedAdReady())
		{
			admobAds.ShowRewardedAd();
		}
	}

	public void RewardAdsCompleted()
	{
		playerData.adsWatched++;

		GameConfig.CheckAdsSkin();
		playerData.SaveData();

		switch (rewardType)
		{
			case "revive":
				FindObjectOfType<ReviveSceneController>().OnAdsCompleted();
				break;
			case "coin":
				FindObjectOfType<GameOverController>().OnAdsCompleted();
				break;
		}
		rewardType = "";
	}

	public void CheckInternet()
	{
		noInternet = Application.internetReachability == NetworkReachability.NotReachable;
	}

	public void OnPurchaseComplete(Product p)
	{
		if (p.definition.id.Contains("noads"))
		{
			playerData.purchasedNoAds = true;
			playerData.SaveData();

			FindObjectOfType<AdMobBanner>().HideBanner();
			banner.SetActive(false);

			if (FindObjectOfType<TitleSceneController>() != null)
			{
				FindObjectOfType<TitleSceneController>().HideAdsButton(false);
			}

		}
		else
		{
			foreach (ShipConfig ship in shipConfigMap.Values)
			{
				if (ship.shipType == ShipConfig.Type.Gacha &&
					ship.iapID.Equals(p.definition.id) && !playerData.skinPurchased.Contains(ship.nameTerm))
				{
					Debug.Log("add skin to player now : " + ship.nameTerm);
					playerData.skinPurchased.Add(ship.nameTerm);
					playerData.chosenSkin = ship.nameTerm;
					playerData.SaveData();

					if (FindObjectOfType<SkinSceneController>() != null)
					{
						FindObjectOfType<SkinSceneController>().PurchaseFinish(ship.nameTerm);
					}
				}
			}
		}
		
	}

	public void TakeScreenShotandShare()
	{
		StartCoroutine(ScreenShot());
	}	

	private IEnumerator ScreenShot()
	{
		yield return new WaitForEndOfFrame();

		Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		ss.Apply();

		filePath = Path.Combine(Application.temporaryCachePath, "ScreenShot.png");
		File.WriteAllBytes(filePath, ss.EncodeToPNG());

		// To avoid memory leaks
		Destroy(ss);

		new NativeShare().SetText("Join me now at Pew Pew Planet \nhttps://go.onelink.me/3uuF/ab1dc7b7").AddFile(filePath).Share();
	}

	private void LoadPlayerData()
	{
		string data = @"
{'Starzinger':{'nameTerm':'Starzinger'},'Nightowl':{'nameTerm':'Nightowl','shipType':'Ads','objective':10},'Orcaz':{'nameTerm':'Orcaz','shipType':'Gacha','iapID':'net.gogame.pewpewplanet.03orcaz'},'Mothra':{'nameTerm':'Mothra','shipType':'Gacha','iapID':'net.gogame.pewpewplanet.04mothra'},'Whalez':{'nameTerm':'Whalez','shipType':'Gacha','iapID':'net.gogame.pewpewplanet.05whalez'},'Slack':{'nameTerm':'Slack','shipType':'Score','objective':7},'Destiny':{'nameTerm':'Destiny','shipType':'Gacha','iapID':'net.gogame.pewpewplanet.07destiny'},'LightYear':{'nameTerm':'LightYear','shipType':'Gacha','iapID':'net.gogame.pewpewplanet.08lightyear'},'CP8':{'nameTerm':'CP8','shipType':'Gacha','iapID':'net.gogame.pewpewplanet.09cp8'},'Lucky13':{'nameTerm':'Lucky13','shipType':'Gacha','iapID':'net.gogame.pewpewplanet.10lucky13'},'M3P':{'nameTerm':'M3P','shipType':'Gacha','iapID':'net.gogame.pewpewplanet.11m3p'},'Tera':{'nameTerm':'Tera','shipType':'Score','objective':10},'Giga':{'nameTerm':'Giga','shipType':'Gacha','iapID':'net.gogame.pewpewplanet.13giga'},'Mega':{'nameTerm':'Mega','shipType':'Gacha','iapID':'net.gogame.pewpewplanet.14mega'},'Nano':{'nameTerm':'Nano','shipType':'MatchPlayed','objective':50},'SwallowRed':{'nameTerm':'SwallowRed','shipType':'MatchPlayed','objective':100},'SwallowYellow':{'nameTerm':'SwallowYellow','shipType':'Gacha','iapID':'net.gogame.pewpewplanet.17yellowswallow'},'SwallowWhite':{'nameTerm':'SwallowWhite','shipType':'Gacha','iapID':'net.gogame.pewpewplanet.18whiteswallow'},'SwallowPink':{'nameTerm':'SwallowPink','shipType':'Gacha','iapID':'net.gogame.pewpewplanet.19pinkswallow'},'SwallowGreen':{'nameTerm':'SwallowGreen','shipType':'Gacha','iapID':'net.gogame.pewpewplanet.20greenswallow'},'SwallowBlue':{'nameTerm':'SwallowBlue','shipType':'Gacha','iapID':'net.gogame.pewpewplanet.21blueswallow'},'SwallowBlack':{'nameTerm':'SwallowBlack','shipType':'Gacha','iapID':'net.gogame.pewpewplanet.22blackswallow'},'Passion':{'nameTerm':'Passion','shipType':'MatchPlayed','objective':10},'Agg':{'nameTerm':'Agg','shipType':'Ads','objective':5},'Bee2stinger':{'nameTerm':'Bee2stinger','shipType':'Ads','objective':50},'Lime':{'nameTerm':'Lime','shipType':'Score','objective':5},'Michaelin':{'nameTerm':'Michaelin','shipType':'Gacha','iapID':'net.gogame.pewpewplanet.27michaelin'},'Flamango':{'nameTerm':'Flamango','shipType':'Gacha','iapID':'net.gogame.pewpewplanet.28flamango'},'MaxEnder':{'nameTerm':'MaxEnder','shipType':'Gacha','iapID':'net.gogame.pewpewplanet.29maxender'},'Firefly':{'nameTerm':'Firefly','shipType':'Gacha','iapID':'net.gogame.pewpewplanet.30firefly'},'Cool':{'nameTerm':'Cool','shipType':'Gacha','iapID':'net.gogame.pewpewplanet.31cool'},'Camomile':{'nameTerm':'Camomile','shipType':'Gacha','iapID':'net.gogame.pewpewplanet.32camomile'},'Blackhole':{'nameTerm':'Blackhole','shipType':'Gacha','iapID':'net.gogame.pewpewplanet.33blackhole'},'Aquajet2000':{'nameTerm':'Aquajet2000','shipType':'Gacha','iapID':'net.gogame.pewpewplanet.34aquajet2000'},'SuperX2':{'nameTerm':'SuperX2','shipType':'Gacha','iapID':'net.gogame.pewpewplanet.35superx2'},'Super':{'nameTerm':'Super','shipType':'Gacha','iapID':'net.gogame.pewpewplanet.36super'}}
			";

		shipConfigMap = Encoder.jsonDecode<Dictionary<string, ShipConfig>>(data);

		if (PlayerPrefs.HasKey(SystemInfo.deviceUniqueIdentifier + "_playerData_galaxy"))
		{
			playerData = Encoder.jsonDecode<PlayerData>(PlayerPrefs.GetString(SystemInfo.deviceUniqueIdentifier + "_playerData_galaxy"));
		}
		else
		{
			playerData = new PlayerData();
			playerData.SaveData();
		}
	}
}
