using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoreButton : MonoBehaviour
{
	public Image musicButton;
	public Sprite disableMusicImage;
	public Sprite enableMusicImage;
	public GameObject badge;
	bool toggleOn = false;
	
	public void ToggleSetting()
	{
		RefreshMusicButton();
		if (!toggleOn)
		{
			BackButtonManager.instance.SetCurrentScreen("hamburger");
			GetComponent<Animator>().SetBool("SlideOut",true);
		}
		else
		{
			GetComponent<Animator>().SetBool("SlideOut",false);
		}

		toggleOn = !toggleOn;
	}

	public void RefreshMusicButton()
	{
		if (musicButton != null)
		{
			if (GameManager.instance.gamePreference.GetMusicToggle() == 1)
			{
				musicButton.GetComponent<Image>().sprite = enableMusicImage;
				AudioListener.volume = 1;
			}
			else
			{
				musicButton.GetComponent<Image>().sprite = disableMusicImage;
				AudioListener.volume = 0;
			}
		}
	}

	public void ToggleMusic()
	{
		GameManager.instance.gamePreference.SetMusicToggle(GameManager.instance.gamePreference.GetMusicToggle() != 1);

		RefreshMusicButton();

	}
}
