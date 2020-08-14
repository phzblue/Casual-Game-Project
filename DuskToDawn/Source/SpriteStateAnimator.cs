using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteStateAnimator : MonoBehaviour
{
	private Coroutine runningCoroutine;
	private SpriteRenderer sr;

	[SerializeField]
	GameObject skinSpriteContainer = null;
	[SerializeField]
	int skinID;
	[SerializeField]
	public string currentState = "";
	[SerializeField]
	bool loop = true;
	[SerializeField]
	int stateIndex = 0;

	List<Sprite> currentSpriteLoop;

	private void Awake()
	{
		if(sr == null)
			sr = GetComponent<SpriteRenderer>();
	}

	public void StartAnimations(string state)
	{
		if (currentState.Equals(state)) {
			return;
		}

		if (!sr.enabled)
		{
			sr.enabled = true;
		}

		currentState = state;

		if (runningCoroutine != null)
		{
			StopCoroutine(runningCoroutine);
			stateIndex = 0;
		}

		loop = false;
		switch (state)
		{
			case "Idle":
				runningCoroutine = StartCoroutine(Animates(1f, skinSpriteContainer.transform.GetChild(skinID).GetComponent<SpriteKeeper>().idle));
				break;
			case "Run":
				loop = true;
				runningCoroutine = StartCoroutine(Animates(.1f, skinSpriteContainer.transform.GetChild(skinID).GetComponent<SpriteKeeper>().run));
				break;
			case "Jump":
				runningCoroutine = StartCoroutine(Animates(0f, skinSpriteContainer.transform.GetChild(skinID).GetComponent<SpriteKeeper>().jump));
				break;
			case "SpeedBoost":
				loop = true;

				runningCoroutine = StartCoroutine(Animates(.02f, skinSpriteContainer.transform.GetChild(skinID).GetComponent<SpriteKeeper>().run));
				break;
			case "Death":
				StopCoroutine(runningCoroutine);
				sr.enabled = false;
				break;

		}
	}

	public SpriteStateAnimator SetSkinID(int skinID)
	{
		this.skinID = skinID - 1;
		currentState = "";

		return this;
	}

	public Sprite GetIdleSprite()
	{
		return skinSpriteContainer.transform.GetChild(skinID).GetComponent<SpriteKeeper>().idle[0];
	}

	private IEnumerator Animates(float frameInterval, List<Sprite> spriteList)
	{
		while (true)
		{
			if (stateIndex >= spriteList.Count)
			{

				stateIndex = 0;
				if (!loop)
				{
					StopCoroutine(runningCoroutine);
					yield break;
				}
			}

			sr.sprite = spriteList[stateIndex];

			yield return new WaitForSeconds(frameInterval);
			stateIndex++;
		}
	}
}
