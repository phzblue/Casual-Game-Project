using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
	[SerializeField] AudioClip radar = null;
	[SerializeField] AudioClip success = null;

	public void PlaySound()
	{
		SoundManager.instance.PlaySFX(radar);
	}

    public void FinishAnimation()
	{
		SoundManager.instance.PlaySFX(success);

		FindObjectOfType<SkinSceneController>().SummonAnimFinish();

	}
}
