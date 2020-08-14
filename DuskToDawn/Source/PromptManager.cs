using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromptManager : MonoBehaviour
{
	[SerializeField] Text title = null;
	[SerializeField] Text desc = null;
	[SerializeField] Button yesButton = null;
	[SerializeField] Button noButton = null;

	PromptType type;

	public void UpdateText(string title, string desc, PromptType type)
	{
		this.title.text = title;
		this.desc.text = desc;
		this.type = type;

		yesButton.gameObject.SetActive(true);
		noButton.gameObject.SetActive(true);

		yesButton.onClick.RemoveAllListeners();
		yesButton.onClick.AddListener(ClosePrompt);

		switch (type)
		{
			case PromptType.Quit:
				yesButton.onClick.AddListener(QuitGame);
				break;
			case PromptType.Loading:
				yesButton.gameObject.SetActive(false);
				noButton.gameObject.SetActive(false);
				break;
			case PromptType.PurchaseFail:
				noButton.gameObject.SetActive(false);
				break;
		}
	}
	
	private void QuitGame()
	{
		Application.Quit();
	}

	private void ClosePrompt()
	{
		transform.GetChild(0).gameObject.SetActive(false);
		BackButtonManager.instance.SetCurrentScreen(BackButtonManager.instance.prevScreen);
	}


	public enum PromptType
	{
		Quit,
		Loading,
		Restore,
		PurchaseFail
	}

}
