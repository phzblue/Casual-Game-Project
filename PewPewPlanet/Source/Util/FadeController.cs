using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FadeController : MonoBehaviour
{
	public static IEnumerator FadeIn(CanvasGroup canvasGroup)
	{
		float t = .1f;
		while (canvasGroup.alpha != 1)
		{
			canvasGroup.alpha = Mathf.Lerp(0, 1, t);
			t += 0.1f + Time.deltaTime;

			yield return new WaitForEndOfFrame();
		}
	}

	public static IEnumerator FadeIn(SpriteRenderer sprite)
	{
		sprite.DOFade(1, 0.5f);
		yield return new WaitForSeconds(0.5f);
	}

	public static IEnumerator FadeOut(SpriteRenderer sprite)
	{
		sprite.DOFade(0, 0.5f);
		yield return new WaitForSeconds(0.5f);
	}

	public static IEnumerator FadeOut(CanvasGroup canvasGroup)
	{
		float t = .1f;
		while (canvasGroup.alpha != 0)
		{
			canvasGroup.alpha = Mathf.Lerp(1, 0, t);

			t += 0.1f + Time.deltaTime;

			yield return new WaitForEndOfFrame();
		}

		//SceneManager.UnloadSceneAsync(4);
	}
}
