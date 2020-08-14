using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenSaver : MonoBehaviour
{
	[SerializeField] Sprite[] spriteList = null;
	int currentSprite = -1;
	bool isRunning = false;

	Coroutine runningRoutine = null;

	private IEnumerator Fadein()
	{
		
		if (GameManager.instance.bgIndex != -1 && currentSprite == -1)
		{
			currentSprite = GameManager.instance.bgIndex;
		}
		else
		{
			currentSprite = Random.Range(0, spriteList.Length);
			GameManager.instance.bgIndex = currentSprite;
		}
		GetComponent<Image>().sprite = spriteList[currentSprite];

		isRunning = true;
		GetComponent<Image>().CrossFadeAlpha(1, 3, true);

		yield return new WaitForSecondsRealtime(10f);
		GetComponent<Image>().CrossFadeAlpha(0, 3, true);

		yield return new WaitForSecondsRealtime(3f);
		isRunning = false;
	}


	// Update is called once per frame
	void Update()
    {
		if (!isRunning)
			runningRoutine = StartCoroutine(Fadein());
	}
}
