using System;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class OptOutHandler : MonoBehaviour
{
	static void OnFailure(string reason)
	{
		Debug.LogWarning(String.Format("Failed to get data privacy page URL: {0}", reason));
	}

	void OnURLReceived(string url)
	{
		Application.OpenURL(url);
	}

	public void OpenDataURL()
	{
		SoundManager.instance.PlayButtonSound();
		if (GameManager.instance.noInternet)
		{
			SceneManager.LoadSceneAsync(9, LoadSceneMode.Additive);
		}
		else
		{
			DataPrivacy.FetchPrivacyUrl(OnURLReceived, OnFailure);
		}
	}
}
