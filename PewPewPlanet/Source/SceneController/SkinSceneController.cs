using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using I2.Loc;
using DG.Tweening;

public class SkinSceneController : MonoBehaviour
{
	Dictionary<string, ShipConfig> shipConfigMap = new Dictionary<string, ShipConfig>();
	List<ShipConfig> gachaShipList = new List<ShipConfig>();

	[SerializeField] Image bigShipImage = null;
	[SerializeField] Text bigShipName = null;
	[SerializeField] Text bigShipDesc = null;
	[SerializeField] Text money = null;
	[SerializeField] Button utiliseButton = null;
	[SerializeField] GameObject scrollContent = null;
	[SerializeField] GameObject contentPrefab = null;
	[SerializeField] GameObject container = null;

	[SerializeField] GameObject backButton = null;
	[SerializeField] GameObject shareButton = null;
	[SerializeField] GameObject summonButton = null;
	[SerializeField] GameObject gachaButton = null;

	[SerializeField] GameObject radar = null;
	[SerializeField] GameObject scrollContainer = null;
	[SerializeField] GameObject textContainer = null;
	[SerializeField] Text missionText = null;

	GameObject currentSelected = null;
	GameObject currentShipButton = null;
	bool isRewardSummon = false;
	bool isGachaClicked = false;

	PlayerData playerData;

	// Start is called before the first frame update
	void Start()
	{
		playerData = GameManager.instance.playerData;

		money.text = playerData.playerCoin.ToString();

		shipConfigMap = GameManager.instance.shipConfigMap;
		PopulateShipContainer();

		SetShip(playerData.chosenSkin);

		utiliseButton.gameObject.SetActive(false);
		summonButton.SetActive(gachaShipList.Count>0);

		if (playerData.unlockedSkin.Count > 0)
		{
			bigShipImage.sprite = Resources.Load<Sprite>("Ships/Sattelite");
			bigShipImage.rectTransform.rotation = Quaternion.identity;
			bigShipImage.rectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

			SummonRewardSkin();
		}
		else
		{
			SetShipOnstage(playerData.chosenSkin);
		}

	}

	public void ModifyUtilityButton(bool completeSummon = false, bool isSet = false, bool isIAP = false, bool isSummon = false, string shipNames = "")
	{
		isGachaClicked = false;

		utiliseButton.onClick.RemoveAllListeners();
		utiliseButton.gameObject.SetActive(true);
		utiliseButton.interactable = true;

		if (completeSummon)
		{
			utiliseButton.transform.GetChild(1).gameObject.SetActive(false);
			utiliseButton.transform.GetChild(0).gameObject.SetActive(true);
			utiliseButton.transform.GetChild(0).GetComponentInChildren<Text>().text = LocalizedString.GetString("roger").ToUpper();
			if (playerData.unlockedSkin.Count > 0)
			{
				utiliseButton.onClick.AddListener(SummonRewardSkin);
			}
			else
			{
				utiliseButton.onClick.AddListener(FinishSummon);
			}
		}
		else if (isSummon) //display how much AllStar Cost on button
		{
			isGachaClicked = true;

			if (playerData.unlockedSkin.Count > 0)
			{
				utiliseButton.transform.GetChild(1).gameObject.SetActive(false);
				utiliseButton.transform.GetChild(0).gameObject.SetActive(true);
				utiliseButton.transform.GetChild(0).GetComponentInChildren<Text>().text = LocalizedString.GetString("unlockGuardian").ToUpper();

				utiliseButton.onClick.AddListener(SummonRewardSkin);
			}
			else
			{
				int gachaCost = GameConfig.GetGachaCost();

				utiliseButton.interactable = playerData.playerCoin >= gachaCost;

				utiliseButton.transform.GetChild(0).gameObject.SetActive(false);
				utiliseButton.transform.GetChild(1).gameObject.SetActive(true);
				utiliseButton.transform.GetChild(1).GetComponentInChildren<Text>().text = gachaCost.ToString();

				utiliseButton.onClick.AddListener(Summon);
			}
			
		}
		else if (isSet) //button can either be 'Set' SetShip
		{
			utiliseButton.transform.GetChild(1).gameObject.SetActive(false);
			utiliseButton.transform.GetChild(0).gameObject.SetActive(true);
			utiliseButton.transform.GetChild(0).GetComponentInChildren<Text>().text = LocalizedString.GetString("use").ToUpper();

			utiliseButton.onClick.AddListener(delegate { SetShip(shipNames); SetUpUtilityButton(shipNames); });
		}
		else if (isIAP) // or '0.99usd'
		{
			utiliseButton.transform.GetChild(1).gameObject.SetActive(false);
			utiliseButton.transform.GetChild(0).gameObject.SetActive(true);
			utiliseButton.transform.GetChild(0).GetComponentInChildren<Text>().text = "$0.99";

			if (IAPManager.instance.GetProductDetail(shipConfigMap[shipNames].iapID) != null)
			{
				utiliseButton.transform.GetChild(0).GetComponentInChildren<Text>().text =
					IAPManager.instance.GetProductDetail(shipConfigMap[shipNames].iapID).metadata.localizedPriceString;

				utiliseButton.onClick.AddListener(delegate {
					if (GameManager.instance.noInternet)
					{
						SceneManager.LoadSceneAsync(9, LoadSceneMode.Additive);
					}
					else
						PurchaseSkin(shipNames);
				});
			}
			else
			{
				utiliseButton.interactable = false;
			}
		}

		utiliseButton.onClick.AddListener(CommonButtonSound);
	}

	public void DisplayGachaInfo(Button button)
	{
		StopAllCoroutines();

		button.Select();
		bigShipDesc.gameObject.SetActive(true);

		if (currentShipButton != null)
			currentShipButton.transform.GetChild(3).gameObject.SetActive(false);

		gachaButton.transform.GetChild(0).gameObject.SetActive(true);

		StartCoroutine(FadeOutAndSlideInAnim("Sattelite"));
		bigShipName.text = LocalizedString.GetString("discover").ToUpper();
		bigShipDesc.text = LocalizedString.GetString("recruit").ToUpper();
		bigShipImage.rectTransform.rotation = Quaternion.identity;	

		ModifyUtilityButton(isSummon: true);
	}

	private void SetUpUtilityButton(string shipName)
	{
		utiliseButton.onClick.RemoveAllListeners();

		if (shipConfigMap[shipName].shipType == ShipConfig.Type.Gacha)
		{
			if (playerData.skinPurchased.Contains(shipName))
			{
				ModifyUtilityButton(isSet: true, shipNames: shipName);
			}
			else
			{
				ModifyUtilityButton(isIAP: true, shipNames: shipName);
			}
		}
		else
		{
			if (!playerData.skinPurchased.Contains(shipName))
			{
				switch (shipConfigMap[shipName].shipType)
				{
					case ShipConfig.Type.Ads:
						bigShipDesc.text = string.Format(LocalizedString.GetString("watchAdToUnlock").ToUpper(), shipConfigMap[shipName].objective - playerData.adsWatched);
						break;
					case ShipConfig.Type.MatchPlayed:
						bigShipDesc.text = string.Format(LocalizedString.GetString("playMatchToUnlock").ToUpper(), shipConfigMap[shipName].objective - playerData.matchPlayed);
						break;
					case ShipConfig.Type.Score:
						bigShipDesc.text = string.Format(LocalizedString.GetString("reachScoreToUnlock").ToUpper(), shipConfigMap[shipName].objective);
						break;
				}
				utiliseButton.gameObject.SetActive(false);
				bigShipDesc.gameObject.SetActive(true);
			}
			else
			{
				ModifyUtilityButton(isSet: true, shipNames: shipName);
			}
		}
	}

	private void SetShipOnstage(string shipName)
	{
		bigShipDesc.gameObject.SetActive(false);
		StopAllCoroutines();

		StartCoroutine(FadeOutAndSlideInAnim(shipName));

		bigShipName.text = LocalizedString.GetString(shipName).ToUpper();
	}


	public IEnumerator FadeOutAndSlideInAnim(string shipName)
	{
		//bigShipImage.DOFade(0, 0.5f);
		//yield return new WaitForSeconds(1);
		//bigShipImage.transform.DOLocalMoveX(-1500, .3f);
		bigShipImage.rectTransform.DOLocalMove(new Vector3(-1600, -1500, 0), .3f);
		yield return new WaitForSeconds(.3f);

		bigShipImage.sprite = Resources.Load<Sprite>("Ships/" + shipName);
		if (shipName.Equals("Sattelite"))
		{
			bigShipImage.rectTransform.rotation = Quaternion.identity;
			bigShipImage.rectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

		}
		else
		{
			bigShipImage.rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, -45));
			bigShipImage.rectTransform.localScale = new Vector3(.8f, .8f, .8f);

		}
		bigShipImage.SetNativeSize();
		bigShipImage.rectTransform.DOLocalMove(new Vector3(0,-272,0),.3f,true);
	}

	private void SetShip(string shipName)
	{
		FindContainer(shipName);
		playerData.chosenSkin = shipName;
		playerData.SaveData();
	}

	private void FindContainer(string shipName)
	{
		foreach(Transform child in container.transform)
		{
			if (child.name.Equals(shipName))
			{
				if (currentSelected != null)
					currentSelected.transform.GetChild(0).gameObject.SetActive(false);
				child.GetChild(0).gameObject.SetActive(true);

				child.GetChild(2).gameObject.SetActive(false);

				currentSelected = child.gameObject;
			}
		}
	}

	public void Share()
	{
		CommonButtonSound();

		if (!GameManager.instance.playerData.hasShare)
		{
			GameManager.instance.playerData.hasShare = true;
			GameManager.instance.playerData.playerCoin += 20;
			GameManager.instance.playerData.SaveData();
			money.text = playerData.playerCoin.ToString();

			if (isGachaClicked)
			{
				int gachaCost = GameConfig.GetGachaCost();
				utiliseButton.interactable = playerData.playerCoin >= gachaCost;
			}

		}

		GameManager.instance.AFTrackRichEvent("af_share");

		GameManager.instance.TakeScreenShotandShare();
	}

	private void SummonRewardSkin()
	{
		textContainer.SetActive(false);

		isRewardSummon = true;
		Summon();
	}

	private void Summon()
	{
		shareButton.SetActive(false);
		scrollContainer.SetActive(false);
		utiliseButton.gameObject.SetActive(false);
		radar.SetActive(true);

		bigShipName.text = LocalizedString.GetString("discover").ToUpper();
		bigShipDesc.text = LocalizedString.GetString("search").ToUpper();
		backButton.SetActive(false);

		if (!isRewardSummon)
		{
			playerData.playerCoin -= GameConfig.GetGachaCost();
			money.text = playerData.playerCoin.ToString();
			playerData.gachaNum++;
			playerData.SaveData();
		}
	}

	private string SummonCalculator()
	{
		if (isRewardSummon)
		{
			string shipName = playerData.unlockedSkin[0];
			playerData.unlockedSkin.RemoveAt(0);
			return shipName;
		}
		else
		{
			int rand = Random.Range(0, gachaShipList.Count);
			string selectedShip = gachaShipList[rand].nameTerm;
			gachaShipList.RemoveAt(rand);

			return selectedShip;
		}
		
	}

	public void SummonAnimFinish()
	{
		shareButton.SetActive(true);

		//summon new ship
		string shipName = SummonCalculator();
		SetShipOnstage(shipName);
		SetShip(shipName);

		playerData.skinPurchased.Add(shipName);
		playerData.chosenSkin = shipName;
		playerData.SaveData();

		GameManager.instance.AFTrackRichEvent("charUnlock");

		if (isRewardSummon)
		{
			missionText.gameObject.SetActive(true);

			switch (shipConfigMap[shipName].shipType)
			{
				case ShipConfig.Type.Ads:
					missionText.text = string.Format(LocalizedString.GetString("watchedAd").ToUpper(), shipConfigMap[shipName].objective);
					break;
				case ShipConfig.Type.Score:
					missionText.text = string.Format(LocalizedString.GetString("reachedScore").ToUpper(), shipConfigMap[shipName].objective);
					break;
				case ShipConfig.Type.MatchPlayed:
					missionText.text = string.Format(LocalizedString.GetString("playedMatch").ToUpper(), shipConfigMap[shipName].objective);
					break;
			}
		}
		else
		{
			missionText.gameObject.SetActive(false);
		}

		ModifyUtilityButton(completeSummon: true);
		summonButton.SetActive(gachaShipList.Count > 0);

		radar.SetActive(false);
		textContainer.SetActive(true);
	}

	public void FinishSummon()
	{
		scrollContainer.SetActive(true);
		textContainer.SetActive(false);

		utiliseButton.gameObject.SetActive(false);
		backButton.SetActive(true);

		isRewardSummon = playerData.unlockedSkin.Count > 0;
	}

	public void PurchaseSkin(string shipName)
	{
		Debug.Log("iap id : " + shipConfigMap[shipName].iapID);
		GameManager.instance.isPurchasing = true;
		IAPManager.instance.PurchaseButtonClick(shipConfigMap[shipName].iapID);
	}

	public void PurchaseFinish(string shipName)
	{
		GameManager.instance.AFTrackRichEvent("charUnlock");

		SetShipOnstage(shipName);
		SetShip(shipName);
		gachaShipList.Remove(shipConfigMap[shipName]);
		ModifyUtilityButton(isSet: true, shipNames: shipName);

		summonButton.SetActive(gachaShipList.Count > 0);
	}

	private void PopulateShipContainer()
	{
		foreach (ShipConfig ship in shipConfigMap.Values)
		{
			if (ship.shipType == ShipConfig.Type.Gacha 
				&& !playerData.skinPurchased.Contains(ship.nameTerm))
			{
				gachaShipList.Add(ship);
			}

			GameObject shipContent = Instantiate(contentPrefab, scrollContent.transform);
			shipContent.name = ship.nameTerm;

			Image image = shipContent.transform.GetChild(1).GetChild(0).GetComponent<Image>();
			image.sprite = Resources.Load<Sprite>("Ships/" + ship.nameTerm);
			image.SetNativeSize();

			shipContent.transform.GetChild(2).gameObject.SetActive(!playerData.skinPurchased.Contains(ship.nameTerm));
			
			shipContent.GetComponent<Button>().onClick.AddListener(delegate {
				gachaButton.transform.GetChild(0).gameObject.SetActive(false);

				if (currentShipButton != null)
					currentShipButton.transform.GetChild(3).gameObject.SetActive(false);

				currentShipButton = shipContent;
				CommonButtonSound();
				shipContent.transform.GetChild(3).gameObject.SetActive(true);
				SetShipOnstage(ship.nameTerm);

				if (playerData.chosenSkin.Equals(ship.nameTerm))
				{
					utiliseButton.gameObject.SetActive(false);

				}
				else
				{
					SetUpUtilityButton(ship.nameTerm);
				}

			});

			if (ship.nameTerm.Equals(playerData.chosenSkin))
			{
				SetShip(ship.nameTerm);
				shipContent.GetComponent<Button>().Select();
			}
		}
	}

	public void CommonButtonSound()
	{
		SoundManager.instance.PlayButtonSound();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (SceneManager.sceneCount > 1)
			{
				SceneManager.UnloadSceneAsync(9);
			}
			else
			{
				TransitionManager.instance.SwitchScene(0);
			}
		}
	}

	public void GoBackMainMenu()
	{
		TransitionManager.instance.SwitchScene(0);
	}
}
