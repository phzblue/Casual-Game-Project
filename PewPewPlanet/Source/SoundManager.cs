using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public static SoundManager instance;
	[SerializeField] AudioSource bgm = null;
	[SerializeField] AudioSource sfx = null;
	[SerializeField] AudioClip button = null;

	private void Awake()
	{
		if (instance == null) instance = this;
		else if (instance != this) Destroy(this);
	}

	public void PlayButtonSound()
	{
		PlaySFX(button);
	}

	public void PlaySFX(GameObject o)
	{
		if (GameManager.instance.playerData.isSFXOn)
		{
			sfx.PlayOneShot(o.GetComponent<AudioClip>());
		}
	}

	public void PlaySFX(AudioClip o)
	{
		if (GameManager.instance.playerData.isSFXOn)
		{
			sfx.PlayOneShot(o);
		}
	}

	public void PlayBGM(AudioClip o, bool loop = true)
	{
		bgm.loop = loop;
		bgm.clip = o;

		Refresh();
	}

	public void FeverPitch(float pitch)
	{
		bgm.pitch = pitch;
	}

	public void Refresh()
	{
		if (!GameManager.instance.playerData.isBGMOn)
		{
			bgm.Stop();
		}
		else
		{
			bgm.Play();
		}
	}
}
