using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using I2.Loc;
using GoogleMobileAdsMediationTestSuite.Api;
using GoogleMobileAds.Api;

public class TitleSceneManager : MonoBehaviour
{
	PlayerData playerData;
	[SerializeField] GameObject gachaButton = null;
	[SerializeField] GameObject languageUI = null;
	[SerializeField] GameObject languageLoadingUI = null;
	[SerializeField] GameObject noAds_iosButton = null;
	[SerializeField] GameObject noAds_adrButton = null;
	[SerializeField] Text coinAmount = null;
	[SerializeField] Image leftRunnerImage = null;
	[SerializeField] Image rightRunnerImage = null;

	[SerializeField] Sprite[] enableGacha = null;
	[SerializeField] Sprite[] disableGacha = null;

	[SerializeField] Text highScoreText = null;
	[SerializeField] Text todayBestText = null;

	[SerializeField] List<Sprite> spriteRunnerList = new List<Sprite>();

	int leftRandID = 1;
	int rightRandID = 1;

	string newLanguageSelected = "";

	bool purchaseProcess = false;
	public bool noInternet = false;

	// Start is called before the first frame update
	void Start()
    {
		BackButtonManager.instance.SetCurrentScreen("title");
		BackButtonManager.instance.RefreshObject();

		playerData = GameManager.instance.playerData;
		coinAmount.text = playerData.playerCoin.ToString();
		GachaButtonSprite(GameManager.instance.CheckGachaAvailability());
		CheckRewardSkin();

		UpdateScore();
		UpdateCharacterSprite();

#if UNITY_IOS
		noAds_iosButton.SetActive(!playerData.purchasedNoAds);
		noAds_adrButton.SetActive(false);
#elif UNITY_ANDROID
		noAds_iosButton.SetActive(false);
		noAds_adrButton.SetActive(!playerData.purchasedNoAds);
#endif
		Invoke("UpdateScore", 0.5f);

		GameManager.instance.PlayBGM();
	}

	public void OpenURL(string url)
	{
		Application.OpenURL(url);
	}

	public void ShowAds()
	{
		GameManager.instance.ShowInterstitialAds();
	}

	public void UpdateScore()
	{

        highScoreText.text = LeaderboardController.instance.playerHighScore.ToString();
		todayBestText.text = LocalizedString.GetString("today") +
            " " + LeaderboardController.instance.playerHighScoreDaily.ToString();
	}

	public void UpdateCharacterSprite()
	{
		if(playerData.leftRunnerID == 0)
		{
			leftRandID = playerData.skinIDContain[Random.Range(1, playerData.skinIDContain.Count)];
		}
		else
		{
			leftRandID = playerData.leftRunnerID;
		}
		GameManager.instance.tempRanRunnerIDL = leftRandID;
		if (playerData.rightRunnerID == 0)
		{
			rightRandID = playerData.skinIDContain[Random.Range(1, playerData.skinIDContain.Count)];
		}
		else
		{
			rightRandID = playerData.rightRunnerID;
		}
		GameManager.instance.tempRanRunnerIDR = rightRandID;


		leftRunnerImage.sprite = spriteRunnerList[leftRandID-1];
		rightRunnerImage.sprite = spriteRunnerList[rightRandID-1];
	}

	public void GachaButtonSprite(bool canGacha)
	{
		SpriteState tempState = gachaButton.GetComponent<Button>().spriteState;

		if (canGacha)
		{
			gachaButton.GetComponent<Image>().sprite = enableGacha[0];
			tempState.pressedSprite = enableGacha[1];
			gachaButton.GetComponent<Button>().spriteState = tempState;
		}
		else
		{
			gachaButton.GetComponent<Image>().sprite = disableGacha[0];
			tempState.pressedSprite = disableGacha[1];
			gachaButton.GetComponent<Button>().spriteState = tempState;
		}
	}

	public void CheckRewardSkin()
	{
		if (playerData.newCharacterIDList.Count > 0)
		{
			BackButtonManager.instance.SetCurrentScreen("gacha_1");

			TransitionManager.instance.SwitchScene(2);
		}
	}

	public void UpdateOpenLanguageUI()
	{
		BackButtonManager.instance.SetCurrentScreen("language");
	}

	public void ToggleLanguage(string lan)
	{
		newLanguageSelected = lan;
	}

	public void EnterPurchaseProcess()
	{
		if (noInternet)
		{
			BackButtonManager.instance.ShowNoInternetPrompt();
		}
		else
		{
			purchaseProcess = true;
		}
	}

	public void OpenLeaderboard()
	{
		if (noInternet)
		{
			BackButtonManager.instance.ShowNoInternetPrompt();
		}
		else
		{
			TransitionManager.instance.SwitchScene(4);
		}
	}

	public void OnPurchaseComplete(Product product)
	{
		if (purchaseProcess)
		{
			purchaseProcess = false;

			GameManager.instance.ProcessPurchase(product);
		}
	}

	public void AddNoAdsPurchase()
	{
		playerData.purchasedNoAds = true;
		playerData.SaveData();

		noAds_iosButton.SetActive(false);
		noAds_adrButton.SetActive(false);
	}

	public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
	{
		if (purchaseProcess)
		{
			purchaseProcess = false;
			switch (reason)
			{
				case PurchaseFailureReason.PurchasingUnavailable:
					Debug.Log("Purchase Fail Reason : ======================================= PurchasingUnavailable");
					break;
				case PurchaseFailureReason.ExistingPurchasePending:
					Debug.Log("Purchase Fail Reason : ======================================= ExistingPurchasePending");
					break;
				case PurchaseFailureReason.ProductUnavailable:
					Debug.Log("Purchase Fail Reason : ======================================= ProductUnavailable");
					break;
				case PurchaseFailureReason.SignatureInvalid:
					Debug.Log("Purchase Fail Reason : ======================================= SignatureInvalid");
					break;
				case PurchaseFailureReason.UserCancelled:
					Debug.Log("Purchase Fail Reason : ======================================= UserCancelled");
					break;
				case PurchaseFailureReason.PaymentDeclined:
					Debug.Log("Purchase Fail Reason : ======================================= PaymentDeclined");
					break;
				case PurchaseFailureReason.DuplicateTransaction:
					Debug.Log("Purchase Fail Reason : ======================================= DuplicateTransaction");
					break;
				case PurchaseFailureReason.Unknown:
					Debug.Log("Purchase Fail Reason : ======================================= Unknown");
					break;
			}
		}
	}

    public void AddSkinID(int id)
    {
        if (!playerData.skinIDContain.Contains(id))
        {
            playerData.skinIDContain.Add(id);	
		}
    }

	public void ShowMediationTestSuite()
	{
		MediationTestSuite.AdRequest = new AdRequest.Builder()
			.AddTestDevice("2E29D88642047EBDE0F7B03181D284C0").Build();
		//addTestDevice("2E29D88642047EBDE0F7B03181D284C0"
		MediationTestSuite.Show();
	}

	private void FixedUpdate()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			noInternet = true;
		}
		else
		{
			noInternet = false;
		}
	}


}
