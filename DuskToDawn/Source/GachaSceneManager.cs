using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;

public class GachaSceneManager : MonoBehaviour
{
	[SerializeField]
	List<Sprite> characterSprite = null;
	[SerializeField]
	Text summonName = null;
	[SerializeField]
	Text summonDesc = null;
	[SerializeField]
	GameObject newImage = null;
	[SerializeField]
	GameObject skinSprite = null;
	
	public Sprite[] confirmSprites;
	public Sprite[] backSprites;
	public GameObject backButton;
	public GameObject rerollButton;
	public Text coinDisplay;
	SpriteState spriteState;

	public GameObject summonButton;
	public GameObject summonText;
	public GameObject shareButton;
	public GameObject light2;
	public GameObject idleGachaPartcle;
	public GameObject tab;
	public GameObject rewardNoticeUI;
	public GameObject rewardNoticeTab;
	public Animator summonedAnimator;
	public Animator summonObject = null;
	public Text summonCost;

	public int rollResult = 0;
	public int prevSummonIndex = -1;
	public bool hasReroll = false;
	public bool rewardSummon = false;
	public bool adsRoll = false;

	private int gachaCost = 0;
	private bool firstView = true;

	private void Start()
	{
		BackButtonManager.instance.SetCurrentScreen("gacha_main");

		RefreshCoinDisplay();

		shareButton.SetActive(false);
		idleGachaPartcle.SetActive(false);
		rewardNoticeTab.SetActive(false);

		summonButton.SetActive(true);
		summonText.SetActive(true);
		summonText.transform.GetChild(0).gameObject.SetActive(false);
		
		if (GameManager.instance.playerData.newCharacterIDList.Count > 0)
		{
			BackButtonManager.instance.SetCurrentScreen("");

			if (firstView)
			{
				rewardNoticeUI.SetActive(true);
				firstView = false;
			}
			rewardNoticeTab.SetActive(true);

			backButton.SetActive(false);
			summonText.GetComponent<Text>().text = LocalizedString.GetString("complete").ToUpper();
			summonText.transform.GetChild(0).gameObject.SetActive(true);
			summonText.transform.GetChild(0).GetComponent<Text>().text = GameConfig.skinList[GameManager.instance.playerData.newCharacterIDList[0]].GetUnlockedString();
			//summonText.GetComponentInChildren<Text>().text = GameConfig.skinList[2].GetUnlockedString();

			summonCost.gameObject.SetActive(false);
			summonButton.GetComponent<Button>().interactable = true;
			hasReroll = false;
			rewardSummon = true;
		}
		else if (GameManager.instance.playerData.NumGachaSkinOwn() < GameConfig.gachaSkinNum)
		{
			backButton.SetActive(true);
			summonCost.gameObject.SetActive(true);
			
			summonButton.GetComponent<Button>().interactable = GameManager.instance.CheckGachaAvailability();
			idleGachaPartcle.SetActive(GameManager.instance.CheckGachaAvailability());

			if (GameConfig.gachaCost.ContainsKey(GameManager.instance.playerData.playerGachaNum))
			{
				summonCost.text = GameConfig.gachaCost[GameManager.instance.playerData.playerGachaNum].ToString();
				gachaCost = GameConfig.gachaCost[GameManager.instance.playerData.playerGachaNum];
			}
			else
			{
				summonCost.text = GameConfig.gachaCost[GameConfig.gachaCost.Count].ToString();
				gachaCost = GameConfig.gachaCost[GameConfig.gachaCost.Count];
			}

			summonText.GetComponent<Text>().text = LocalizedString.GetString("getNewRunner").ToUpper();

			spriteState = backButton.GetComponent<Button>().spriteState;

			hasReroll = true;
			rewardSummon = false;

		}
		else
		{
			summonCost.text = GameConfig.gachaCost[GameConfig.gachaCost.Count].ToString();
			gachaCost = GameConfig.gachaCost[GameConfig.gachaCost.Count];
			
			summonText.GetComponent<Text>().text = LocalizedString.GetString("summonedAll").ToUpper();
			summonButton.GetComponent<Button>().interactable = false;
		}

	}

	public void AdWatched()
	{
		rerollButton.SetActive(false);
		backButton.SetActive(false);
		coinDisplay.transform.parent.gameObject.SetActive(false);
		ResetAnim();
	}

	public void UpdateSummonNameDesc(int summonCharacterIndex)
	{
        summonName.text = LocalizedString.GetString("skinNameID" + summonCharacterIndex);
		summonDesc.text = LocalizedString.GetString("skinDescID" + summonCharacterIndex);

		backButton.GetComponent<Image>().sprite = confirmSprites[0];
		spriteState.pressedSprite = confirmSprites[1];
		backButton.GetComponent<Button>().spriteState = spriteState;
	}

	public void GachaResultCalculator()
	{ 
		int ranIndex;
		if (GameManager.instance.playerData.newCharacterIDList.Count > 0)
		{
			ranIndex = GameManager.instance.playerData.newCharacterIDList[0];
			GameManager.instance.playerData.newCharacterIDList.RemoveAt(0);

			rewardSummon = true;
		}
		else
		{
			if (adsRoll)
			{
				adsRoll = false;
				Debug.Log(GameManager.instance.playerData.skinIDContain.Remove(prevSummonIndex) + " =========================== remove?");
				prevSummonIndex = -1;
			}

			rewardSummon = false;

			ranIndex = Random.Range(1, GameConfig.skinList.Count);
			while (true)
			{
				if (GameConfig.skinList[ranIndex].skinCategory == SkinDataManager.SkinCategory.PURCHASE &&
					!GameManager.instance.playerData.skinIDContain.Contains(ranIndex))
				{
					break;
				}
				else
				{
					ranIndex = Random.Range(1, GameConfig.skinList.Count);
				}
			}

			prevSummonIndex = ranIndex;
		}
		
		rollResult = ranIndex;
		newImage.SetActive(true);

		skinSprite.GetComponent<Image>().sprite = characterSprite[rollResult-1];
		UpdateSummonNameDesc(ranIndex);

		GameManager.instance.playerData.skinIDContain.Add(rollResult);
		GameManager.instance.playerData.SaveData();

	}

	public void ShowAds()
	{
		GameManager.instance.ShowRewardedAds("FreeRoll");
	}

	public void SummonGacha()
	{
		summonObject.SetTrigger("Summon");
		GameManager.instance.ToggleBGM(false);
		coinDisplay.transform.parent.gameObject.SetActive(false);
		summonButton.SetActive(false);
		backButton.SetActive(false);
		summonText.gameObject.SetActive(false);
		idleGachaPartcle.SetActive(false);
		tab.SetActive(false);

		BackButtonManager.instance.SetCurrentScreen("");
	}

	public void OnAdsFinish()
	{
		adsRoll = true;
		summonedAnimator.SetBool("Summon", false);

		this.Invoke("SummonGacha", 0.5f);

		hasReroll = false;
		GameConfig.CheckFreeSkin();
		GameManager.instance.playerData.SaveData();
	}

	public void DeductCoin()
	{
		if (!rewardSummon)
		{
			GameManager.instance.playerData.playerCoin -= gachaCost;
			GameManager.instance.playerData.SaveData();
			RefreshCoinDisplay();
		}
	}

	public void RefreshCoinDisplay()
	{
		coinDisplay.text = GameManager.instance.playerData.playerCoin.ToString();
	}

	public void ConfirmButton()
	{
		if (rollResult == 0)
		{
			BackButtonManager.instance.SetCurrentScreen("title");
			TransitionManager.instance.SwitchScene(0);
		}
		else
		{
			BackButtonManager.instance.SetCurrentScreen("gacha_2");

			rerollButton.SetActive(false);

			if (!GameManager.instance.playerData.skinIDContain.Contains(rollResult))
			{
				GameManager.instance.playerData.skinIDContain.Add(rollResult);
				GameManager.instance.AFTrackRichEvent("charUnlock");
				GameManager.instance.playerData.SaveData();
			}

			GameManager.instance.playerData.leftRunnerID = rollResult;

			if (!rewardSummon)
				GameManager.instance.playerData.playerGachaNum++;

			rollResult = 0;

			GameManager.instance.playerData.SaveData();

			Start();
		}

		summonButton.GetComponent<Button>().interactable = GameManager.instance.CheckGachaAvailability();

		if (GameConfig.gachaCost.ContainsKey(GameManager.instance.playerData.playerGachaNum))
		{
			summonCost.text = GameConfig.gachaCost[GameManager.instance.playerData.playerGachaNum].ToString();
		}
		else
		{
			summonCost.text = GameConfig.gachaCost[GameConfig.gachaCost.Count].ToString();
		}

		ResetAnim();
	}

	public void ResetAnim()
	{
		prevSummonIndex = -1;
		summonedAnimator.SetBool("Summon", false);
		light2.SetActive(false);

		backButton.GetComponent<Image>().sprite = backSprites[0];
		spriteState.pressedSprite = backSprites[1];
		backButton.GetComponent<Button>().spriteState = spriteState;
	}

	private void OnDisable()
	{
		firstView = true;
	}
}
