using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;

public class GameSceneManager : MonoBehaviour
{
	public static GameSceneManager instance;

	public ScoreManager scoreManager = null;
	public ObjectGenerator objectGenerator = null;
	public PlayerController pc;
	public PlayerData playerData = null;

	public Text coinAmount = null;
	public AudioSource feverBGM = null;
	public AudioClip coinSound = null;

	public GameObject heartAds = null;
	public GameObject coinAds = null;
	public GameObject rewardtab = null;
	public GameObject reviveMenu = null;
	public GameObject deathMenu = null;
	public GameObject pauseMenu = null;
	public GameObject earnCoinContainer = null;
	public GameObject innerAdsBanner = null;
	public GameObject coinRewardParticle = null;
	public Button coinAdsButton = null;

	public Text deathScoreText = null;
	public Text adsCoinText = null;
	public Text highscore = null;

	public bool isPlayable = false;
	public bool feverMode = false;
	public bool powerupReset = true;
	public bool canContinue = true;
	public int currentAdsCoin = 0;
	public bool noInternet = false;

	void Awake()
	{
		if (instance == null) instance = this;
		else if (instance != this) Destroy(gameObject);

		playerData = GameManager.instance.playerData;
	}

	private void Start()
	{
		GameManager.instance.gameNumPerSession++;
		BackButtonManager.instance.SetCurrentScreen("game");
		coinAmount.text = playerData.playerCoin.ToString();

        highscore.text = LocalizedString.GetString("highScore") + " " + LeaderboardController.instance.playerHighScore;

		Invoke("StartGame", 1f);
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

	public void ShowInterstitialAds()
	{
		GameManager.instance.ShowInterstitialAds();
	}

	public void FeverBgmToggle(bool isOn)
	{
		GameManager.instance.ToggleBGM(!isOn);

		feverBGM.loop = true;
		if (isOn)
		{
			feverBGM.Play();
		}
		else
		{
			feverBGM.Stop();
		}
	}

	public void CoinControl(int coin = 0)
	{
		if (coin == 0)
		{
			playerData.playerCoin++;
		}
		else
		{
			playerData.playerCoin += coin;
		}

		coinAmount.text = playerData.playerCoin.ToString();
	}

	public void RefreshCoinDisplay()
	{
		coinAmount.text = playerData.playerCoin.ToString();
	}

	public void StartGame()
	{
		isPlayable = true;

		GameObject.FindObjectOfType<Tutorial>().StartTutorial();
		scoreManager.scoreIncreasing = true;
		PlayerController.instance.InitPlayers(true);

		CheckReward();
	}

	public void CheckReward()
	{
		string rewardText = GameConfig.CheckFreeSkinFromMatchPlayed();

		if (!rewardText.Equals(""))
		{
			rewardtab.GetComponent<TabMovment>().SetTextInTab(rewardText);
			rewardtab.SetActive(true);
		}
	}

	public void GameEnd()
	{
		scoreManager.scoreIncreasing = false;
        isPlayable = false;

		pc.FeverIncrease(false);
		pc.ResetFocuser();
		pc.gameObject.SetActive(false);

		ToggleDeathMenuAds(canContinue, GameManager.instance.IsAdsReady());
	}

	public void ToggleDeathMenuAds(bool canContinue, bool isAdsReady)
	{
		pc.pause = canContinue;

		heartAds.SetActive(isAdsReady);
		coinAds.SetActive(isAdsReady);

		if (canContinue)
		{
            this.canContinue = false;
			BackButtonManager.instance.SetCurrentScreen("revive");
            pc.pause = true;
			reviveMenu.SetActive(true);
		}
		else
		{
            BackButtonManager.instance.SetCurrentScreen("death");

			pc.pause = false;
			Invoke("ShowDeathMenu", 3f);
		}

		GameConfig.CheckFreeSkin(Mathf.RoundToInt(scoreManager.scoreCount));
	}

    public void ShowDeathMenu()
    {
        deathMenu.SetActive(true);
    }

	public void ShowPauseMenu()
	{
		BackButtonManager.instance.SetCurrentScreen("pause_1");
		pauseMenu.SetActive(true);
		pauseMenu.GetComponentInChildren<PauseMenu>().PauseGame();
	}

	public void ShowAds(string type)
	{
		GameManager.instance.ShowRewardedAds(type);
	}

	public void OnAdsFinishCoin()
	{
		playerData.playerCoin += currentAdsCoin;
		RefreshCoinDisplay();
		UpdateCoinEarn();
	}

	public void OnAdsFinishHeart()
    {
        FindObjectOfType<LifeController>().Revive();
		pauseMenu.GetComponentInChildren<PauseMenu>().ResumeGame(true);

		Vector3 playerPosL = pc.transform.position - new Vector3(.5f, 0, 0);
		Vector3 playerPosR = pc.transform.position - new Vector3(.5f, 0, 0);

		pc.gameObject.SetActive(true);

		pc.ResetPlayerXPos(playerPosR, playerPosL);

		reviveMenu.SetActive(false);
		deathMenu.SetActive(false);
		
		string rewardText = GameConfig.CheckFreeSkinFromAdsWatched();

		if (!rewardText.Equals(""))
		{
			rewardtab.GetComponent<TabMovment>().SetTextInTab(rewardText);
			rewardtab.SetActive(true);
		}
	}

	public void UpdateCoinEarn()
	{
		innerAdsBanner.SetActive(false);
		coinAdsButton.gameObject.SetActive(false);
		earnCoinContainer.SetActive(true);

        adsCoinText.text = string.Format(LocalizedString.GetString("coinEarn"), currentAdsCoin);

		string rewardText = GameConfig.CheckFreeSkinFromAdsWatched();

		if (!rewardText.Equals(""))
		{
			rewardtab.GetComponent<TabMovment>().SetTextInTab(rewardText);
			rewardtab.SetActive(true);
		}

		//do some coin animation here
		coinRewardParticle.SetActive(true);
		StartCoroutine(PlayCoinSound());
	}

	private IEnumerator PlayCoinSound()
	{
		feverBGM.loop = false;
		for (int i = 0; i < 5; i++)
		{
			feverBGM.PlayOneShot(coinSound);
			yield return new WaitForSeconds(0.5f);
		}
	}
}