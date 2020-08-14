using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using I2.Loc;

public class GameOverController : MonoBehaviour
{
	//[SerializeField] GameObject rankButton = null;
	[SerializeField] CanvasGroup adsObject = null;
	[SerializeField] Button adsButton = null;
	[SerializeField] Text adsText = null;
	[SerializeField] Text finalScore = null;
	[SerializeField] Text bestScore = null;
	[SerializeField] Text todayBest = null;
	[SerializeField] Text playerCoin = null;
	[SerializeField] Text level = null;
	[SerializeField] AudioClip bgm = null;
	[SerializeField] GameObject particle = null;
	[SerializeField] Text coinText = null;
	[SerializeField] AudioClip coinSound = null;
	int adsCoin = 0;

	// Start is called before the first frame update
	void Start()
    {
		GameManager.instance.playerData.matchPlayed++;
		GameConfig.CheckScoreSkin(GameManager.instance.lastLevel);
		Server.instance.UpdateHighScore(GameManager.instance.finalScore);

		if(GameManager.instance.playerData.matchPlayed == 1)
		{
			GameManager.instance.AFTrackRichEvent("af_tutorial_completion");
		}

		GameManager.instance.AFTrackRichEvent("sendScore", score: GameManager.instance.finalScore);

		GameConfig.CheckMatchSkin();
		GameManager.instance.playerData.SaveData();

		adsCoin = Random.Range(20, 50);
		finalScore.text = GameManager.instance.finalScore.ToString();
		bestScore.text = LocalizedString.GetString("best").ToUpper() + " " + Server.instance.playerBestScore;
		todayBest.text = LocalizedString.GetString("today").ToUpper() + " " + Server.instance.playerDailyScore;
		playerCoin.text = GameManager.instance.playerData.playerCoin.ToString();
		level.text = GameManager.instance.lastLevel.ToString();

		bool hasAds = GameManager.instance.IsAdsReady();

		if (hasAds)
		{
			//got ads
			adsButton.interactable = true;
			adsText.text = LocalizedString.GetString("watchAdsCoin").ToUpper();
		}
		else
		{
			// no ads
			adsButton.interactable = false;
			adsText.text = LocalizedString.GetString("noAdsToWatch").ToUpper();
		}

		StartCoroutine(FadeController.FadeIn(adsObject));
	}

	public void CommonButtonSound()
	{
		SoundManager.instance.PlayButtonSound();
	}

	public void NextButton()
	{
		TransitionManager.instance.SwitchScene(0);
		SoundManager.instance.PlayBGM(bgm);
		GameManager.instance.Invoke("ShowInterstitialAds", 1f);
	}

	public void LoadLeaderBoardScene()
	{
		CommonButtonSound();

		if (GameManager.instance.noInternet)
		{
			SceneManager.LoadSceneAsync(9, LoadSceneMode.Additive);
		}
		else
		{
			if (GameManager.instance.playerName == "")
			{
				SceneManager.LoadScene(4, LoadSceneMode.Single);
			}
			else
			{
				SceneManager.LoadScene(3, LoadSceneMode.Single);
			}
		}

	}

	public void ShareImage()
	{
		CommonButtonSound();

		if (!GameManager.instance.playerData.hasShare)
		{
			GameManager.instance.playerData.hasShare = true;
			GameManager.instance.playerData.playerCoin += 20;
			GameManager.instance.playerData.SaveData();
			UpdateCoinText();
		}

		GameManager.instance.AFTrackRichEvent("af_share");

		GameManager.instance.TakeScreenShotandShare();
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
				NextButton();
			}
		}
	}

	public void UpdateCoinText()
	{
		playerCoin.text = GameManager.instance.playerData.playerCoin.ToString();
	}

	public void ShowAds(string rewardType)
	{
		GameManager.instance.rewardType = rewardType;
		GameManager.instance.ShowRewardAds();
	}

	public void OnAdsCompleted()
	{
		StartCoroutine(FadeController.FadeOut(adsObject));
		particle.SetActive(true);

		coinText.gameObject.SetActive(true);
		coinText.text = string.Format(LocalizedString.GetString("earned"), adsCoin);
		StartCoroutine(PlayCoinSound());

		GameManager.instance.playerData.playerCoin += adsCoin;
		playerCoin.text = GameManager.instance.playerData.playerCoin.ToString();
	}

	private IEnumerator PlayCoinSound()
	{
		for (int i = 0; i < 5; i++)
		{
			SoundManager.instance.PlaySFX(coinSound);
			yield return new WaitForSeconds(0.5f);
		}
	}
}
