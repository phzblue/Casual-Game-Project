using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideAnimation : MonoBehaviour
{
	public GameObject birdAnim;
	public GameObject[] particles;

	private void OnEnable()
	{
		foreach(GameObject o in particles)
		{
			o.SetActive(false);
		}
		GetComponent<Animator>().SetTrigger("SlideIn");
	}

	public void Close()
	{
		GetComponent<Animator>().SetTrigger("SlideOut");
		birdAnim.SetActive(false);
	}

	public void DisableObject()
	{
		foreach (GameObject o in particles)
		{
			o.SetActive(true);
		}
		gameObject.SetActive(false);
	}

	public void ShowBird()
	{
		birdAnim.SetActive(true);
	}
}
