using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePreference : MonoBehaviour
{
	public int GetMusicToggle()
	{
		if (!PlayerPrefs.HasKey("Music"))
		{
			SetMusicToggle();
		}

		return PlayerPrefs.GetInt("Music");

	}

	public void SetMusicToggle(bool isOn = true)
	{
		PlayerPrefs.SetInt("Music", isOn ? 1 : 0);
	}
}
