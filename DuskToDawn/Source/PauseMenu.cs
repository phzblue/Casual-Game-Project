using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
	public GameObject pauseMenu;
	public GameObject quitMenu;
	public GameObject infoPanel;
	public Text countdownText;
	public GameObject backButton;
	public GameObject muteButton;

	public void PauseGame()
	{
		BackButtonManager.instance.SetCurrentScreen("pause_1");
		GameSceneManager.instance.isPlayable = false;
		GameSceneManager.instance.scoreManager.scoreIncreasing = false;

		Time.timeScale = 0f;
		
		pauseMenu.SetActive(true);
		infoPanel.SetActive(true);
		backButton.SetActive(true);
		muteButton.SetActive(true);

		countdownText.text = "3";
	}

	public void ResumeGame(bool isRevive)
	{
		infoPanel.SetActive(false);
		backButton.SetActive(false);
		muteButton.SetActive(false);
		pauseMenu.SetActive(true);

		StartCoroutine(StartCountdownToPlay(isRevive));
	}

	public void CloseQuitUI()
	{
		quitMenu.SetActive(false);
		BackButtonManager.instance.SetCurrentScreen("pause_1");
	}

	public void OpenQuitUI()
	{
		quitMenu.SetActive(true);
		BackButtonManager.instance.SetCurrentScreen("pause_2");
	}

	public IEnumerator StartCountdownToPlay(bool isRevive = false)
	{
		countdownText.gameObject.SetActive(true);
		
		for (int i = 3; i > 0; i--)
		{
			countdownText.text = i.ToString();
			yield return new WaitForSecondsRealtime(1);
		}
		countdownText.text = "Go!";
		yield return new WaitForSecondsRealtime(1);

		countdownText.gameObject.SetActive(false);
		pauseMenu.SetActive(false);

		Time.timeScale = 1f;

		GameSceneManager.instance.isPlayable = true;
		GameSceneManager.instance.scoreManager.scoreIncreasing = true;

		BackButtonManager.instance.SetCurrentScreen("game");

		if (isRevive)
		{
			GameSceneManager.instance.pc.gameObject.SetActive(true);
			GameSceneManager.instance.pc.InitPlayers(true);
			GameSceneManager.instance.pc.EnableFlash(5f);
		}

		GameSceneManager.instance.CheckReward();
	}
	
	public void ResumeTime()
	{
		Time.timeScale = 1f;
	}
}
