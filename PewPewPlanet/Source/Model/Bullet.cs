using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bullet : MonoBehaviour
{
	GameObject planet;
	Rigidbody2D rb;
	bool isTriggerred = false;
	public int bulletSpeed = 20;

	[SerializeField] BulletManager.BulletType type = BulletManager.BulletType.Default;
	[SerializeField] GameObject particle = null;
	[SerializeField] AudioClip particleSound = null;
	[SerializeField] AudioClip gameover = null;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	// Start is called before the first frame update
	void Start()
    {
		planet = FindObjectOfType<PlanetManager>().gameObject;
    }

	private void OnEnable()
	{
		planet = FindObjectOfType<PlanetManager>().gameObject;

		isTriggerred = false;
		GetComponent<SpriteRenderer>().enabled = true;
		if (planet != null)
		{
			if (type == BulletManager.BulletType.Boomerang)
			{
				ShootBoomerang();
			}
			else
			{
				StartCoroutine(ShootBullet());
			}
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		bulletSpeed = 10;
	}

	private void Update()
	{
		if(type == BulletManager.BulletType.Boomerang && !isTriggerred)
		{
			transform.Rotate(new Vector3(0, 0, 5));
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.name.Contains("Minion"))
		{
			Debug.Log(collision.gameObject.name + " ======================================= by " + gameObject.name);

			isTriggerred = true;

			rb.velocity = Vector2.zero;
			KillMinion(collision.gameObject.GetComponent<MinionManager>());
		}
		else if (collision.gameObject.name.Contains("Planet") ||
			collision.gameObject.name.Contains("Wall"))
		{
			rb.AddForce(250 * collision.GetContact(0).normal);

		}
		
		else if (collision.gameObject.name.Contains("Power"))
		{
			isTriggerred = true;

			rb.velocity = Vector2.zero;
			collision.gameObject.GetComponent<PickUp>().ActivatePickUp();
			GetComponent<SpriteRenderer>().enabled = false;


			particle.SetActive(true);
			Invoke("HideBullet", 2.5f);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!isTriggerred)
		{

			Debug.Log(collision.name + " ======================================= 2");

			if (collision.name.Contains("Minion"))
			{
				isTriggerred = true;

				if (collision.name.Contains("Hostage"))
				{
					collision.transform.GetComponent<MinionManager>().IsShot();

					if (!GameSceneController.instance.isFever)
					{
						GameSceneController.instance.isPlayable = false;
						SoundManager.instance.PlaySFX(gameover);
						GameSceneController.instance.Invoke("GameOver", 1f);
					}
				}
				else
				{
					KillMinion(collision.transform.GetComponent<MinionManager>());
				}
				gameObject.SetActive(false);
			}
			else if (collision.name.Contains("Power"))
			{
				isTriggerred = true;

				if (GameSceneController.instance.isFever)
				{
					GameSceneController.instance.hasPickUpStored = true;
					GameSceneController.instance.pickUpStore = collision.GetComponent<PickUp>().type;
					collision.GetComponent<PickUp>().gameObject.SetActive(false);
				}
				else
				{
					collision.GetComponent<PickUp>().ActivatePickUp();
				}

				GetComponent<SpriteRenderer>().enabled = false;
				gameObject.SetActive(false);

			}
			else if (collision.name.Contains("PlanetCollider"))
			{
				isTriggerred = true;
				
				if (!GameSceneController.instance.isFever)
				{
					GameSceneController.instance.isPlayable = false;
					GameSceneController.instance.CrackPlanet(collision.transform.name.Contains("PlanetCollider"));
					SoundManager.instance.PlaySFX(gameover);
					GameSceneController.instance.Invoke("GameOver", 1f);
				}
				
				gameObject.SetActive(false);
			}

			Time.timeScale = 1f;

		}

	}

	public void KillMinion(MinionManager minion)
	{
		isTriggerred = true;

		GameManager.instance.playerData.playerCoin++;
		GameSceneController.instance.UpdateScore(10 * GameSceneController.instance.currentLevel);

		GetComponent<SpriteRenderer>().enabled = false;

		if (type == BulletManager.BulletType.Boomerang)
		{
			particle.SetActive(true);
			SoundManager.instance.PlaySFX(particleSound);
			Invoke("HideBullet",2.5f);
		}
		else
		{
			gameObject.SetActive(false);
		}

		GameSceneController.instance.IncreaseFeverMeter();
		GameSceneController.instance.UpdateMinionCount();
		minion.IsShot();
	}

	public void HideBullet()
	{
		if (type == BulletManager.BulletType.Boomerang)
		{
			particle.SetActive(false);
		}
		gameObject.SetActive(false);
	}

	void AddForce()
	{
		rb.AddForce(new Vector2(0, 200));
	}

	private void ShootBoomerang()
	{
		AddForce();
	}

	IEnumerator ShootBullet()
	{
		int delay = 0;
		while(true)
		{
			transform.position = Vector3.MoveTowards(transform.position, planet.transform.position, bulletSpeed * Time.deltaTime);
			if(delay > 5)
			{
				bulletSpeed = 30;
			}
			else
			{
				delay++;
			}
			yield return new WaitForEndOfFrame();
		}
	}
}
