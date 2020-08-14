using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;

public class ReviveMenu : MonoBehaviour
{
	public Text currentScore;
	public Text hiScore;
	public Text todayScore;

	public GameObject deathMenu;
	public GameObject rewardtab;
	public AudioSource resultSound;

	private void OnEnable()
	{
		GetComponent<CanvasGroup>().blocksRaycasts = true;

		currentScore.text = ((long)Mathf.Round(GameSceneManager.instance.scoreManager.scoreCount)).ToString();
		hiScore.text = LocalizedString.GetString("best") + " " + LeaderboardController.instance.playerHighScore.ToString();
		todayScore.text = LocalizedString.GetString("today") + " " +LeaderboardController.instance.playerHighScoreDaily.ToString();
	}

	public void FadeInDeathMenu()
	{
		deathMenu.SetActive(true);
		BackButtonManager.instance.SetCurrentScreen("death");
		resultSound.PlayDelayed(1);
	}

	public void DisableTouch()
	{
		GetComponent<CanvasGroup>().blocksRaycasts = false;
	}

	public void DisableObject()
	{
		gameObject.SetActive(false);
	}
}
