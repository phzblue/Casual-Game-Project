using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_IOS || UNITY_ANDROID
using Helpshift;
using HSMiniJSON;
#endif

public class HelpShift : MonoBehaviour
{
	[SerializeField] GameObject badges1_adr;
	[SerializeField] GameObject badges2_adr;
	[SerializeField] GameObject badges1_ios;
	[SerializeField] GameObject badges2_ios;

#if UNITY_IOS || UNITY_ANDROID
	private HelpshiftSdk _support;
	
	public void didReceiveNotificationCount(string count)
	{
		if (Int32.TryParse(count, out int x))
		{
			if (x > 0)
			{
#if UNITY_IOS
				badges2_ios.SetActive(true);
				badges1_ios.SetActive(true);
				badges2_ios.GetComponentInChildren<Text>().text = count;
				badges1_ios.GetComponentInChildren<Text>().text = count;
#elif UNITY_ANDROID
				badges1_adr.SetActive(true);
				badges1_adr.GetComponentInChildren<Text>().text = count;
				badges2_adr.SetActive(true);
				badges2_adr.GetComponentInChildren<Text>().text = count;
#endif
			}
			else
			{
#if UNITY_IOS
				badges1_ios.SetActive(false);
#elif UNITY_ANDROID
				badges1_adr.SetActive(false);
#endif
			}
		}
	}

	public void didReceiveInAppNotificationCount(string count)
	{
		if (Int32.TryParse(count, out int x))
		{
			if (x > 0)
			{
#if UNITY_IOS
				badges2_ios.SetActive(true);
				badges1_ios.SetActive(true);
				badges2_ios.GetComponentInChildren<Text>().text = count;
				badges1_ios.GetComponentInChildren<Text>().text = count;
#elif UNITY_ANDROID
				badges1_adr.SetActive(true);
				badges1_adr.GetComponentInChildren<Text>().text = count;
				badges2_adr.SetActive(true);
				badges2_adr.GetComponentInChildren<Text>().text = count;
#endif
			}
			else
			{
#if UNITY_IOS
				badges1_ios.SetActive(false);
#elif UNITY_ANDROID
				badges1_adr.SetActive(false);
#endif
			}
		}
	}

	public void didReceiveUnreadMessagesCount(string count)
	{
		if (Int32.TryParse(count, out int x))
		{
			if (x > 0)
			{
#if UNITY_IOS
				badges2_ios.SetActive(true);
				badges1_ios.SetActive(true);
				badges2_ios.GetComponentInChildren<Text>().text = count;
				badges1_ios.GetComponentInChildren<Text>().text = count;
#elif UNITY_ANDROID
				badges1_adr.SetActive(true);
				badges1_adr.GetComponentInChildren<Text>().text = count;
				badges2_adr.SetActive(true);
				badges2_adr.GetComponentInChildren<Text>().text = count;
#endif
			}
			else
			{
#if UNITY_IOS
				badges1_ios.SetActive(false);
#elif UNITY_ANDROID
				badges1_adr.SetActive(false);
#endif
			}
		}
	}
	
	private void Awake()
	{
		_support = HelpshiftSdk.getInstance();

		string apiKey = "104b4c0624a8d4af55e1450f32fc3141";
		string domainName = "gogame.helpshift.com";
		string appId;
#if UNITY_ANDROID
		appId = "gogame_platform_20190815050725429-c8b0474e199e3c5";
#elif UNITY_IOS
		appId = "gogame_platform_20190815050725419-6114cbea21b1f1d";
#endif
		_support.install(apiKey, domainName, appId, getInstallConfig());
	}

	private void Start()
	{
		//badge1 = GameObject.FindGameObjectWithTag("badge1");
		//badge2 = GameObject.FindGameObjectWithTag("badge2");

#if UNITY_ANDROID
		_support.registerDelegates();
#endif
		_support.requestUnreadMessagesCount(true);
	}

	public void ShowConversation()
	{
		_support.showConversation(getApiConfig());
	}

	private Dictionary<string, object> getInstallConfig()
	{
		Dictionary<string, object> installDictionary = new Dictionary<string, object>();
		installDictionary.Add("unityGameObject", "Helpshift");
		installDictionary.Add("enableInAppNotification", "yes"); // Possible options:  "yes", "no"
		installDictionary.Add("enableDefaultFallbackLanguage", "yes"); // Possible options:  "yes", "no"
		installDictionary.Add("disableEntryExitAnimations", "no"); // Possible options:  "yes", "no"
		installDictionary.Add("enableInboxPolling", "yes"); // Possible options:  "yes", "no"
		installDictionary.Add("enableLogging", "yes"); // Possible options:  "yes", "no"
		installDictionary.Add("screenOrientation", -1); // Possible options:  SCREEN_ORIENTATION_LANDSCAPE=0, SCREEN_ORIENTATION_PORTRAIT=1, SCREEN_ORIENTATION_UNSPECIFIED = -1
		return installDictionary;
	}

	private Dictionary<string, object> getApiConfig()
	{
		Dictionary<string, object> configDictionary = new Dictionary<string, object>();
		/*
        Possible values:
        CONTACT_US_ALWAYS, CONTACT_US_NEVER, CONTACT_US_AFTER_VIEWING_FAQS, CONTACT_US_AFTER_MARKING_ANSWER_UNHELPFUL
         */
		configDictionary.Add("enableContactUs", HelpshiftSdk.CONTACT_US_AFTER_VIEWING_FAQS);
		configDictionary.Add("gotoConversationAfterContactUs", "no"); // Possible options:  "yes", "no"
		configDictionary.Add("requireEmail", "no"); // Possible options:  "yes", "no"
		configDictionary.Add("hideNameAndEmail", "no"); // Possible options:  "yes", "no"
		configDictionary.Add("enableFullPrivacy", "no"); // Possible options:  "yes", "no"
		configDictionary.Add("showSearchOnNewConversation", "no"); // Possible options:  "yes", "no"
		configDictionary.Add("showConversationResolutionQuestion", "yes"); // Possible options:  "yes", "no"
		configDictionary.Add("enableTypingIndicator", "yes"); // Possible options:  "yes", "no"
		configDictionary.Add("showConversationInfoScreen", "yes"); // Possible options:  "yes", "no"
		return configDictionary;
	}

	public void HideBadge()
	{
#if UNITY_IOS
		badges1_ios.SetActive(false);
#elif UNITY_ANDROID
		badges1_adr.SetActive(false);
#endif
	}

	public void HideBadge2()
	{
#if UNITY_IOS
		badges2_ios.SetActive(false);
#elif UNITY_ANDROID
		badges2_adr.SetActive(false);
#endif
	}

#endif
}
