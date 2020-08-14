using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using I2.Loc;
using UnityEngine.SceneManagement;

public class ReviveSceneController : MonoBehaviour
{
	[SerializeField] CanvasGroup reviveScene = null;
	[SerializeField] CanvasGroup adsObject = null;
	[SerializeField] GameObject scoreObject = null;
	[SerializeField] Button adsButton = null;
	[SerializeField] Text adsText = null;
	[SerializeField] Text finalScore = null;
	[SerializeField] Text bestScore = null;
	[SerializeField] Text todayBest = null;
	[SerializeField] Text playerCoin = null;
	[SerializeField] Text level = null;
	[SerializeField] AudioClip loseSound = null;
	[SerializeField] AudioClip bgm = null;

    // Start is called before the first frame update
    void Start()
    {
		SoundManager.instance.PlayBGM(loseSound, false);

		level.text = GameManager.instance.lastLevel.ToString();
		finalScore.text = GameManager.instance.finalScore.ToString();
		bestScore.text = LocalizedString.GetString("best").ToUpper() + " " + Server.instance.playerBestScore;
		todayBest.text = LocalizedString.GetString("today").ToUpper() + " " + Server.instance.playerDailyScore;
		playerCoin.text = GameManager.instance.playerData.playerCoin.ToString();

		bool hasAds = GameManager.instance.IsAdsReady();

		if (hasAds)
		{
			//got ads
			adsButton.interactable = true;
			adsText.text = LocalizedString.GetString("watchAdsPlaying").ToUpper();
		}
		else
		{
			// no ads
			adsButton.interactable = false;
			adsText.text = LocalizedString.GetString("noAdsToWatch").ToUpper();
		}

		StartCoroutine(FadeInScene());
	}

	IEnumerator FadeInScene()
	{
		yield return StartCoroutine(FadeController.FadeIn(reviveScene));

		scoreObject.gameObject.GetComponent<RectTransform>().DOLocalMoveX(1080/2, 1, true);
		//scoreObject.gameObject.transform.DOMoveX(0, 1f, true);
		yield return new WaitForSeconds(1f);
		finalScore.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
		yield return new WaitForSeconds(0.5f);
		bestScore.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
		yield return new WaitForSeconds(0.5f);
		todayBest.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
		yield return new WaitForSeconds(0.5f);

		StartCoroutine(FadeController.FadeIn(adsObject));
	}

	IEnumerator FadeOutScene()
	{
		yield return StartCoroutine(FadeController.FadeOut(adsObject));

		TransitionManager.instance.SwitchScene(6);

	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			NextButton();
		}
	}

	public void CommonButtonSound()
	{
		SoundManager.instance.PlayButtonSound();
	}

	public void NextButton()
	{
		StartCoroutine(FadeOutScene());
	}

	public void ShowAds(string rewardType)
	{
		GameManager.instance.rewardType = rewardType;
		GameManager.instance.ShowRewardAds();
	}

	public void OnAdsCompleted()
	{
		StartCoroutine(FadeController.FadeOut(adsObject));
		GameSceneController.instance.isPlayable = true;
		GameSceneController.instance.CrackPlanet(false);
		SoundManager.instance.PlayBGM(bgm);

		SceneManager.UnloadSceneAsync(5);

	}
}
