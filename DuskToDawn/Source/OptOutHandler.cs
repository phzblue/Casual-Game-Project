using System;
using UnityEngine;
using UnityEngine.Analytics;

public class OptOutHandler : MonoBehaviour
{
	private void Start()
	{
	}

	static void OnFailure(string reason)
	{
	}

	void OnURLReceived(string url)
	{
		Application.OpenURL(url);
	}

	public void OpenDataURL()
	{
		DataPrivacy.FetchPrivacyUrl(OnURLReceived, OnFailure);
	}
}
