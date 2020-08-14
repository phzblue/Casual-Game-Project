using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeverPower : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.name.Contains("Player"))
		{
			//gameObject.GetComponent<SpriteRenderer>().enabled = false;
			Invoke("delaySetFalse", 0f);
		}

	}

	void delaySetFalse()
	{
		gameObject.SetActive(false);
		gameObject.GetComponent<SpriteRenderer>().enabled = true;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.GetComponent<ObjectDestroyer>().isActiveAndEnabled)
		{
			gameObject.SetActive(false);
		}
	}
}
