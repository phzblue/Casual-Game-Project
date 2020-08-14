using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;

public class Tutorial : MonoBehaviour
{
	[SerializeField]
	GameObject tut2CanvasObj = null;
	
	[SerializeField]
	GameObject rightHandIndicator = null;

	[SerializeField]
	GameObject tutorialText = null;

	public void StartTutorial()
	{
		if (tut2CanvasObj.activeInHierarchy)
		{
			StartCoroutine(StartFirstTut());
			StartCoroutine(StartSecondTut());
		}
	}

	IEnumerator StartFirstTut()
	{
		yield return new WaitForSeconds(.5f);
		tutorialText.GetComponent<Text>().text = LocalizedString.GetString("hint1");
        tut2CanvasObj.GetComponent<Animator>().SetBool("LeftHand", true);
		yield return new WaitForSeconds(1.5f);
		tut2CanvasObj.GetComponent<Animator>().SetBool("LeftHand", false);
	}

	IEnumerator StartSecondTut()
	{
		yield return new WaitForSeconds(2.5f);
		rightHandIndicator.SetActive(true);
		tutorialText.SetActive(true);

		tutorialText.GetComponent<Text>().text = LocalizedString.GetString("hint2");
        tut2CanvasObj.GetComponent<Animator>().SetBool("RightHand", true);
		yield return new WaitForSeconds(3f);
		tut2CanvasObj.GetComponent<Animator>().SetBool("RightHand", false);
	}
}
