using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionManager : MonoBehaviour
{
	GameObject[] waypoints = null;
	[SerializeField] float speed = 1f;
	[SerializeField] AudioClip explodeSFX = null;
	int num = 0;
	bool isKilled = false;

	public void ResetMinion()
	{
		waypoints = FindObjectOfType<PlanetManager>().wayPoints;

		StartCoroutine(MoveToWayPoint());
		GetComponent<SpriteRenderer>().enabled = true;

		speed = 1 + (GameSceneController.instance.currentLevel * 0.5f);
	}

	IEnumerator MoveToWayPoint()
	{
		Vector3 newPos;
		while (!isKilled)
		{
			float step = speed * Time.deltaTime;
			newPos = Vector3.MoveTowards(transform.position, waypoints[num].transform.position, step);
			float absSign = newPos.x - transform.position.x;
			
			if(transform.localPosition.y < 0)
			{
				if(Mathf.Sign(absSign) == -1)
				{
					GetComponent<SpriteRenderer>().flipX = true;
				}
				else
				{
					GetComponent<SpriteRenderer>().flipX = false;
				}
			}
			else
			{
				if (Mathf.Sign(absSign) == -1)
				{
					GetComponent<SpriteRenderer>().flipX = false;
				}
				else
				{
					GetComponent<SpriteRenderer>().flipX = true;
				}
			}

			transform.position = newPos;

			if (Vector3.Distance(transform.localPosition, waypoints[num].transform.localPosition) < 0.5f)
			{
				//yield return new WaitForSeconds(2f);
				//num = Random.Range(0, waypoints.Length);
				if (FindObjectOfType<PlanetManager>().isReverse)
				{
					num--;
					if (num == -1)
						num = waypoints.Length - 1;
				}
				else
				{
					num++;
					if (num == waypoints.Length)
						num = 0;
				}
			}

			yield return new WaitForEndOfFrame();
		}
	}

	public void IsShot()
	{
		SoundManager.instance.PlaySFX(explodeSFX);

		isKilled = true;
		GetComponent<SpriteRenderer>().enabled = false;
		GetComponent<Collider2D>().enabled = false;
		transform.GetChild(0).gameObject.SetActive(true);
		Invoke("HideMinion", 0.5f);
	}

	private void HideMinion()
	{
		gameObject.SetActive(false);
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		transform.GetChild(0).gameObject.SetActive(false);
		GetComponent<Collider2D>().enabled = true;
		isKilled = false;
		waypoints = null;
		num = 0;
		transform.position = new Vector3(0, 3, 0);
		transform.rotation = new Quaternion(0, 0, 0,0);
	}

	private void OnParticleCollision(GameObject other)
	{
		Debug.Log("hit by particle collision! =============================== " + other.name);
		IsShot();

		GameSceneController.instance.IncreaseFeverMeter();
		GameSceneController.instance.UpdateMinionCount();
		GameManager.instance.playerData.playerCoin++;
		GameSceneController.instance.UpdateScore(10 * GameSceneController.instance.currentLevel);
	}
}
