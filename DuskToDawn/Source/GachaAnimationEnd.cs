using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaAnimationEnd : MonoBehaviour
{
	public Animator showSummonResult;
	public GameObject rerollButton;
	public GameObject shareButton;
	public GameObject backButton;
	public GameObject coin;

	public GameObject light1;
	public GameObject light1a;
	public GameObject light2;

	public AudioSource gachaSound;

	public void PlayeSound()
	{
		gachaSound.Play();
	}

	public void GachaAnimationEndCall()
	{
		showSummonResult.SetBool("Summon", true);
		BackButtonManager.instance.SetCurrentScreen("gacha_2");
	}

	public void GachaAnimationStartCall()
	{
		shareButton.SetActive(false);
		rerollButton.SetActive(false);

		gachaSound.Play();
		light2.GetComponent<ParticleSystem>().Stop();
		light2.GetComponent<ParticleSystem>().Clear();
		light2.SetActive(false);
		
		GameObject.FindObjectOfType<GachaSceneManager>().GachaResultCalculator();

		showSummonResult.GetComponent<CanvasGroup>().alpha = 0;
		showSummonResult.SetBool("Summon", false);
	}

	public void SummonEnd()
	{
		coin.SetActive(true);
		backButton.SetActive(true);
		shareButton.SetActive(true);
		
		if (GameObject.FindObjectOfType<GachaSceneManager>().hasReroll)
		{

			rerollButton.SetActive(GameManager.instance.IsAdsReady() );
			GameObject.FindObjectOfType<GachaSceneManager>().hasReroll = false;
		}
		else
		{
			rerollButton.SetActive(false);
		}

		GameManager.instance.ToggleBGM(true);
	}

	public void ShowLight1()
	{
		//light1.Play();
		light1.SetActive(true);
		light1a.SetActive(true);
		light1.GetComponent<ParticleSystem>().Play();
		light1a.GetComponent<ParticleSystem>().Play();
	}

	public void ShowLight2()
	{
		light1.GetComponent<ParticleSystem>().Stop();
		light1.GetComponent<ParticleSystem>().Clear();
		light1.SetActive(false);

		light1a.GetComponent<ParticleSystem>().Stop();
		light1a.GetComponent<ParticleSystem>().Clear();
		light1a.SetActive(false);

		light2.SetActive(true);
		light2.GetComponent<ParticleSystem>().Play();
	}
}
