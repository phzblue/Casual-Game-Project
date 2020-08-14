using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
	Rigidbody2D rb;
	GameObject planet;

	public PickUpType type = PickUpType.Hostage;
	public float xPos, yPos;
	public AudioClip pickUpSFX = null;
	[SerializeField] GameObject particle = null;

	Vector2 bounceValue;

	public enum PickUpType
	{
		Laser,
		Coin,
		Boomerang,
		Hostage
	}
	
	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	void Start()
	{
		planet = FindObjectOfType<PlanetManager>().gameObject;
		transform.position = planet.transform.position;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		bounceValue = 75 * collision.GetContact(0).normal;
		rb.rotation = Random.Range(60,180);
		rb.AddForce(bounceValue);
	}

	public void StartMoving()
	{
		xPos = Random.Range(-175, 175);
		yPos = Random.Range(-175, 175);

		if(Random.Range(0f,1.0f) > .5f)
		{
			xPos *= -1;
		}

		if (Random.Range(0f, 1.0f) > .5f)
		{
			yPos *= -1;
		}

		rb.AddForce(new Vector2(xPos, yPos));
	}

	public void ActivatePickUp()
	{
		switch (type)
		{
			case PickUpType.Boomerang:
				FindObjectOfType<BulletManager>().ChangeBulletType(BulletManager.BulletType.Boomerang);
				break;
			case PickUpType.Coin:
				GameManager.instance.playerData.playerCoin++;
				GameSceneController.instance.UpdateMoneyText();
				break;
			case PickUpType.Laser:
				FindObjectOfType<BulletManager>().ChangeBulletType(BulletManager.BulletType.Laser);
				break;
		}

		GetComponent<SpriteRenderer>().enabled = false;
		particle.SetActive(true);

		Invoke("HideObject", 1f);

		SoundManager.instance.PlaySFX(pickUpSFX);
	}

	public void HideObject()
	{
		gameObject.SetActive(false);
	}

	private void OnDisable()
	{
		if(planet != null)
			transform.position = planet.transform.position;
		rb.velocity = Vector2.zero;

		GetComponent<SpriteRenderer>().enabled = true;

	}
}
