using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using I2.Loc;
using DG.Tweening;

public class InternetPrompt : MonoBehaviour
{
	public int tutHint;
	[SerializeField] Text buttonText = null;
	[SerializeField] GameObject hint1 = null;
	[SerializeField] GameObject hint2 = null;
	[SerializeField] GameObject hint3 = null;
	[SerializeField] GameObject hint4 = null;

	public void OKButton()
	{
		SoundManager.instance.PlayButtonSound();
		SceneManager.UnloadSceneAsync(9);
	}

	public void TutNextButton()
	{
		SoundManager.instance.PlayButtonSound();

		if (tutHint == 4)
		{
			TransitionManager.instance.SwitchScene(1);
		}
		else
		{
			buttonText.transform.parent.GetComponent<Button>().interactable = false;
			StartCoroutine(FadeObjects(tutHint));
		}
	}

	public IEnumerator FadeObjects(int num)
	{
		buttonText.GetComponentInParent<CanvasGroup>().DOFade(0, 0.5f);
		switch (num)
		{
			case 1:
				hint1.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
				hint2.transform.DOLocalMoveX(0, 0.5f);
				break;
			case 2:
				hint2.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
				hint3.transform.DOLocalMoveX(0, 0.5f);
				break;
			case 3:
				hint3.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
				hint4.transform.DOLocalMoveX(0, 0.5f);
				break;
		}
		yield return new WaitForSeconds(0.5f);

		if (tutHint + 1 == 4)
		{
			buttonText.text = LocalizedString.GetString("roger");
		}
		tutHint++;

		buttonText.GetComponentInParent<CanvasGroup>().DOFade(1, 0.5f);
		buttonText.transform.parent.GetComponent<Button>().interactable = true;

		yield return new WaitForSeconds(1f);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if(tutHint != -1)
			{
				TutNextButton();
			}
			else
			{
				OKButton();
			}
		}
	}
}
