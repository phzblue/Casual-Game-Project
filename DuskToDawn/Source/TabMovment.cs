using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;

public class TabMovment : MonoBehaviour
{
	RectTransform rect;
	public Text text;
	[SerializeField] string type = "";

	private void Awake()
	{
		rect = GetComponent<RectTransform>();
	}

	private void OnEnable()
	{
		StartCoroutine(ShowYourself());
	}

	public void SetTextInTab(string value)
	{
		switch (type)
		{
			case "milestone":
				text.text = LocalizedString.GetString("milestoneReach");
				break;
			case "skin":
				text.text = value;
				break;
		}
	}

	public IEnumerator ShowYourself()
	{
		while(rect.anchoredPosition.x <= 0)
		{
			rect.anchoredPosition = new Vector2(rect.anchoredPosition.x + 80, rect.anchoredPosition.y);
			yield return new WaitForEndOfFrame();
		}

		rect.anchoredPosition = new Vector2(0, rect.anchoredPosition.y);
		yield return new WaitForSecondsRealtime(1.5f);

		while (rect.anchoredPosition.x >= -563)
		{
			rect.anchoredPosition = new Vector2(rect.anchoredPosition.x - 80, rect.anchoredPosition.y);
			yield return new WaitForEndOfFrame();
		}

		rect.anchoredPosition = new Vector2(-563, rect.anchoredPosition.y);

		yield return new WaitForSecondsRealtime(0.5f);

		gameObject.SetActive(false);
	}
}
