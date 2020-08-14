using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using I2.Loc;

public class TitleSceneController: MonoBehaviour
{
	[SerializeField] Text bestScore = null;
	[SerializeField] Text todayScore = null;
	[SerializeField] Text coin = null;
	[SerializeField] GameObject noAdsPurchaseButton = null;
	[SerializeField] GameObject summonParticle = null;
	[SerializeField] GameObject summonNewText = null;
	[SerializeField] GameObject exitPrompt = null;
	[SerializeField] Image summonButton = null;

	DateTime utcDate = DateTime.UtcNow.AddHours(8);

	// Start is called before the first frame update
	void Start()
    {
		bestScore.text = LocalizedString.GetString("best").ToUpper() + " " + Server.instance.playerBestScore;
		todayScore.text = LocalizedString.GetString("today").ToUpper() + " " + Server.instance.playerDailyScore;
		coin.text = GameManager.instance.playerData.playerCoin.ToString();

		noAdsPurchaseButton.SetActive(!GameManager.instance.playerData.purchasedNoAds);
		UpdateShipImages();

		summonParticle.SetActive(GameManager.instance.playerData.unlockedSkin.Count > 0);
		summonNewText.SetActive(GameManager.instance.playerData.unlockedSkin.Count > 0);
		
	}

	private void UpdateShipImages()
	{

		summonButton.sprite = Resources.Load<Sprite>("Ships/" + GameManager.instance.playerData.chosenSkin);
		summonButton.SetNativeSize();

		float x = summonButton.GetComponent<RectTransform>().sizeDelta.x;
		float y = summonButton.GetComponent<RectTransform>().sizeDelta.y;

		summonButton.GetComponent<RectTransform>().sizeDelta = new Vector2(x * 0.3f, y*0.3f);

	}

	public void PurchaseNoAds()
	{
		//net.gogame.pewpewplanet.noads
		if (GameManager.instance.noInternet)
		{
			SceneManager.LoadSceneAsync(9, LoadSceneMode.Additive);
		}
		else
		{
			IAPManager.instance.PurchaseButtonClick("net.gogame.pewpewplanet.noads");
		}
	}

	public void HideAdsButton(bool isActive)
	{
		noAdsPurchaseButton.SetActive(isActive);
	}

	public void BackButtonSound()
	{
		SoundManager.instance.PlayButtonSound();
	}

	public void ChangeToGameScene()
	{
		GameManager.instance.AFTrackRichEvent("match_played");
		if(GameManager.instance.playerData.matchPlayed == 0)
		{
			TransitionManager.instance.SwitchScene(10);
		}
		else
		{
			TransitionManager.instance.SwitchScene(1);
		}
	}

	public void LoadLeaderBoardScene()
	{
		if (GameManager.instance.noInternet)
		{
			SceneManager.LoadSceneAsync(9, LoadSceneMode.Additive);
		}
		else
		{
			if (GameManager.instance.playerName == "")
			{
				TransitionManager.instance.SwitchScene(4);
			}
			else
			{
				TransitionManager.instance.SwitchScene(3);
			}
		}
	}

	public void ChangeSettingScene()
	{
		TransitionManager.instance.SwitchScene(7);
	}

	public void ChangeSkinScene()
	{
		TransitionManager.instance.SwitchScene(2);
	}

	public void ExitGame()
	{
		SoundManager.instance.PlayButtonSound();
		Application.Quit();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if(SceneManager.sceneCount > 1)
			{
				SceneManager.UnloadSceneAsync(9);
			}
			else
			{
				exitPrompt.SetActive(!exitPrompt.activeInHierarchy);
			}
		}
	}
}
