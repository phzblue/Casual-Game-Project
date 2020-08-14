using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SetNameController : MonoBehaviour
{
	[SerializeField] CanvasGroup canvasGroup = null;
	[SerializeField] GameObject failedObject = null;
	[SerializeField] Button setButton = null;
	string inputName = "";

    // Start is called before the first frame update
    void Start()
    {
		failedObject.SetActive(false);
		
		StartCoroutine(FadeController.FadeIn(canvasGroup));
	}

	IEnumerator FadeOut()
	{
		yield return StartCoroutine(FadeController.FadeOut(canvasGroup));
		SceneManager.UnloadSceneAsync(4);
	}

	public void SetFailedString(string s)
	{
		failedObject.SetActive(true);
		failedObject.GetComponentInChildren<Text>().text = s;
	}

	public void OnEndEdit(string s)
	{
		inputName = s;
		setButton.interactable = s != "";
	}

	public void CommonButtonSound()
	{
		SoundManager.instance.PlayButtonSound();
	}

	public void SetName()
	{
		//send create player request here
		Server.instance.CreatePlayer(inputName);
	}

	public void BackButtonPress()
	{
		TransitionManager.instance.SwitchScene(0);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			BackButtonPress();
		}
	}
}
