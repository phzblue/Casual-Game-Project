using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using I2.Loc;
using UnityEngine.Android;

public class SettingSceneController : MonoBehaviour
{
	[SerializeField] Sprite greenSprite = null;
	[SerializeField] Sprite redSprite = null;

	[SerializeField] Image bgmBar = null;
	[SerializeField] Image sfxBar = null;
	[SerializeField] Image vibrateBar = null;
	[SerializeField] Text version = null;
	[SerializeField] Text restoreDesc = null;

	[SerializeField] GameObject languageCanvas = null;
	[SerializeField] GameObject hintCanvas = null;
	[SerializeField] GameObject restorePrompt = null;
	[SerializeField] GameObject restorePromptButton = null;
	[SerializeField] GameObject restoreBackButton = null;

	bool isBGMOn = true;
	bool isSFXOn = true;
	bool isVibration = true;

	int sceneNum = 0;

	private void Start()
	{
		if(version != null)
			version.text = "v" + Application.version + " © 2019 goGame";

		isBGMOn = GameManager.instance.playerData.isBGMOn;
		isSFXOn = GameManager.instance.playerData.isSFXOn;
		isVibration = GameManager.instance.playerData.isVibrationOn;

		ReloadData();
	}

	public void SetScreen(int num)
	{
		sceneNum = num;
	}

	public void CommonButtonSound()
	{
		SoundManager.instance.PlayButtonSound();
	}

	public void RestorePurchase()
	{
		CommonButtonSound();
		if (GameManager.instance.noInternet)
		{
			SceneManager.LoadSceneAsync(9, LoadSceneMode.Additive);
		}
		else
		{
			restorePrompt.SetActive(true);
			restoreBackButton.SetActive(true);
			IAPManager.instance.RestoreButtonClick();
		}
	}

	public void SetLanguage(string lan)
	{
		CommonButtonSound();
		LocalizationManager.CurrentLanguageCode = lan;
		sceneNum = 0;
	}

	private void ReloadData()
	{
		
		if (isSFXOn)
		{
			sfxBar.sprite = greenSprite;
		}
		else
		{
			sfxBar.sprite = redSprite;
		}

		if (isBGMOn)
		{
			bgmBar.sprite = greenSprite;
		}
		else
		{
			bgmBar.sprite = redSprite;
		}

		if (isVibration)
		{
			vibrateBar.sprite = greenSprite;
		}
		else
		{
			vibrateBar.sprite = redSprite;
		}
	}

	public void ToggleSFX()
	{
		CommonButtonSound();

		isSFXOn = !isSFXOn;

		if (isSFXOn)
		{
			sfxBar.sprite = greenSprite;
		}
		else
		{
			sfxBar.sprite = redSprite;
		}

		GameManager.instance.playerData.isSFXOn = isSFXOn;
		GameManager.instance.playerData.SaveData();
	}

	public void ToggleBGM()
	{
		CommonButtonSound();

		isBGMOn = !isBGMOn;

		if (isBGMOn)
		{
			bgmBar.sprite = greenSprite;
		}
		else
		{
			bgmBar.sprite = redSprite;
		}

		GameManager.instance.playerData.isBGMOn = isBGMOn;
		GameManager.instance.playerData.SaveData();

		SoundManager.instance.Refresh();
	}

	public void ToggleVibration()
	{
		CommonButtonSound();

		isVibration = !isVibration;

		if (isVibration)
		{
			vibrateBar.sprite = greenSprite;
		}
		else
		{
			vibrateBar.sprite = redSprite;
		}

		GameManager.instance.playerData.isVibrationOn = isVibration;
		GameManager.instance.playerData.SaveData();
	}

	public void ShowPrivacyPolicy()
	{
		CommonButtonSound();

		if (GameManager.instance.noInternet)
		{
			SceneManager.LoadSceneAsync(9, LoadSceneMode.Additive);
		}
		else
		{
			Application.OpenURL("https://gogame.net/privacy-policy/");
		}
	}

	public void ShowTOS()
	{
		CommonButtonSound();

		if (GameManager.instance.noInternet)
		{
			SceneManager.LoadSceneAsync(9, LoadSceneMode.Additive);
		}
		else
		{
			Application.OpenURL("https://gogame.net/terms-of-services/");
		}
	}

	public void ShowCustomerService()
	{
		CommonButtonSound();

#if UNITY_IOS || UNITY_ANDROID
		FindObjectOfType<HelpShift>().ShowConversation();
#endif
	}

	public void ExitGame()
	{
		Time.timeScale = 1;
		GameSceneController.instance.isPlayable = false;
		TransitionManager.instance.SwitchScene(0);
	}

	public void BackButton()
	{
		if (restorePrompt != null && restorePrompt.activeInHierarchy)
		{
			restorePrompt.SetActive(false);
			restoreDesc.text = LocalizedString.GetString("loading");
			restorePromptButton.SetActive(false);
		}
		if (SceneManager.sceneCount > 1)
		{
			try
			{
				GameSceneController.instance.isPlayable = true;
			}
			catch(Exception e)
			{
			}

			SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));

		}
		else
		{
			if(GameSceneController.instance != null)
				GameSceneController.instance.isPlayable = true;
			TransitionManager.instance.SwitchScene(0);
		}
	}

	public void RestoreComplete(bool success)
	{
		if(restorePromptButton != null)
		{
			restorePromptButton.SetActive(true);

			if (success)
			{
				restoreBackButton.SetActive(false);
				restoreDesc.text = LocalizedString.GetString("restoreSuccess");
			}
			else
			{
				restoreDesc.text = LocalizedString.GetString("restoreFail");
			}
		}
		
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if(sceneNum == 0)
			{
				BackButton();
			}
			else
			{
				switch (sceneNum)
				{
					case 1: //language canvas here
						languageCanvas.SetActive(false);
						break;
					case 2: //hint canvas here
						hintCanvas.SetActive(false);
						break;
				}
				sceneNum = 0;
			}
		}
	}
}
