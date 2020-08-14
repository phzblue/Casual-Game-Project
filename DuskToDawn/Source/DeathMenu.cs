using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;

public class DeathMenu : MonoBehaviour
{
	public Text adsCoin;
	public Text hiScore;
	public Text currentScore;
	public Text todayScore;
	public GameObject earnCoinContainer;
	public GameObject coinRewardParticle;

	public GameObject topUpBanner;
	public GameObject gachaBanner;
	public GameObject innerAdsBanner;
	public GameObject adsButton;
	public GameObject rewardtab;
	public GameObject leaderboardButton;

	private AudioSource audioSource;
	public AudioClip coinSound;

	private void Awake()
	{
		if (audioSource == null)
			audioSource = GetComponent<AudioSource>();
		if (coinSound == null)
			coinSound = GameObject.FindObjectOfType<PowerupManager>().coinSFX;
	}

	private void OnEnable()
	{
		leaderboardButton.SetActive(!GameSceneManager.instance.noInternet);

		if (GameManager.instance.playerData.matchPlayed == 0)
		{
			GameManager.instance.AFTrackRichEvent("af_tutorial_completion");
		}

		string rewardText = GameConfig.CheckFreeSkinFromMatchPlayed();

		if (!rewardText.Equals(""))
		{
			rewardtab.GetComponent<TabMovment>().SetTextInTab(rewardText);
			rewardtab.SetActive(true);
		}

		GameManager.instance.AFTrackRichEvent("sendScore", null, (long)Mathf.Round(GameSceneManager.instance.scoreManager.scoreCount));

		LeaderboardController.instance.UpdateHighScore((long)Mathf.Round(GameSceneManager.instance.scoreManager.scoreCount));

		//topUpBanner.SetActive(!GameManager.instance.CheckGachaAvailability());
		gachaBanner.SetActive(GameManager.instance.CheckGachaAvailability());

		earnCoinContainer.SetActive(false);
		innerAdsBanner.SetActive(true);
		adsButton.SetActive(true);

		GameSceneManager.instance.currentAdsCoin = Random.Range(30, 100);
		GameManager.instance.playerData.matchPlayed++;
		GameConfig.CheckFreeSkin();
		GameManager.instance.playerData.SaveData();

		currentScore.text = ((long)Mathf.Round(GameSceneManager.instance.scoreManager.scoreCount)).ToString();
		hiScore.text = LocalizedString.GetString("best") + " " + LeaderboardController.instance.playerHighScore.ToString();
		todayScore.text = LocalizedString.GetString("today") + " " + LeaderboardController.instance.playerHighScoreDaily.ToString();

	}

	public void UpdateCoinEarn()
	{
		innerAdsBanner.SetActive(false);
		adsButton.SetActive(false);
		earnCoinContainer.SetActive(true);

        
		adsCoin.text = string.Format(LocalizedString.GetString("coinEarn"), GameSceneManager.instance.currentAdsCoin);

		string rewardText = GameConfig.CheckFreeSkinFromAdsWatched();

		if (!rewardText.Equals(""))
		{
			rewardtab.GetComponent<TabMovment>().SetTextInTab(rewardText);
			rewardtab.SetActive(true);
		}

		//do some coin animation here
		coinRewardParticle.SetActive(true);
		GameSceneManager.instance.CoinControl(GameSceneManager.instance.currentAdsCoin);
		StartCoroutine(PlayCoinSound());
	}

	private IEnumerator PlayCoinSound()
	{
		for(int i = 0; i < 5; i++)
		{
			audioSource.PlayOneShot(coinSound);
			yield return new WaitForSeconds(0.5f);
		}
	}
}
