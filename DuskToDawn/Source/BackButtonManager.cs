using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;

public class BackButtonManager : MonoBehaviour
{
	public static BackButtonManager instance;

	[SerializeField] PromptManager promptUI;
	[SerializeField] GameObject languageUI;

	[SerializeField] string currentScreen;
	public string prevScreen;

	void Awake()
	{
		if (instance == null) instance = this;
		else if (instance != this) Destroy(gameObject);
	}

	public void RefreshObject()
	{
		promptUI = GameObject.FindObjectOfType<PromptManager>();
		languageUI = GameObject.FindGameObjectWithTag("LanguageUI");
	}

	public void ShowNoInternetPrompt()
	{
		prevScreen = currentScreen;
		currentScreen = "noInternet";
		promptUI.UpdateText(
						LocalizedString.GetString("internet"),
						LocalizedString.GetString("requiresNetwork"), PromptManager.PromptType.PurchaseFail);
		promptUI.transform.GetChild(0).gameObject.SetActive(true);

	}

	public void SetCurrentScreen(string curScreen)
	{
		currentScreen = curScreen;
	}

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			switch (currentScreen)
			{
				case "noInternet":
				case "quit":
					promptUI.transform.GetChild(0).gameObject.SetActive(false);
					currentScreen = prevScreen;
					break;
				case "hamburger":
					FindObjectOfType<MoreButton>().ToggleSetting();
					currentScreen = "title";
					break;
				case "title":
					//quit game
					if(promptUI != null)
					{
						promptUI.UpdateText(
						LocalizedString.GetString("quit"),
						LocalizedString.GetString("sure"), PromptManager.PromptType.Quit);
						promptUI.transform.GetChild(0).gameObject.SetActive(true);
						currentScreen = "quit";
					}
					break;
				case "language":
					languageUI.transform.GetChild(0).gameObject.SetActive(false);
					currentScreen = "title";
					break;
				case "skin":
				case "gacha_main":
				case "gacha_1":
				case "restore":
					currentScreen = "title";
					TransitionManager.instance.SwitchScene(0);
					break;
				case "gacha_2":
					GameObject.FindObjectOfType<GachaSceneManager>().ConfirmButton();
					currentScreen = "gacha_main";
					break;
				case "game":
					GameObject.FindObjectOfType<GameSceneManager>().ShowPauseMenu();
					currentScreen = "pause_1";
					break;
				case "pause_1":
					GameObject.FindObjectOfType<PauseMenu>().ResumeGame(false);
					currentScreen = "";
					break;
				case "pause_2":
					GameObject.FindObjectOfType<PauseMenu>().CloseQuitUI();
					currentScreen = "pause_1";
					break;
				case "revive":
					GameObject.FindObjectOfType<ReviveMenu>().GetComponent<Animator>().SetTrigger("fadeout");
					currentScreen = "death";
					break;
				case "death":
				case "leaderboard":
					TransitionManager.instance.SwitchScene(0);
					GameManager.instance.ShowInterstitialAds();
					currentScreen = "title";
					break;
			}
		}
	}
}
