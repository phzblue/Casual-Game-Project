using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerups : MonoBehaviour
{
    public bool kunai;
    public bool smokeBomb;
    public bool onigiri;
	public bool shield;
	public bool life;
	public bool coin;
	public bool magnet;

    public float powerupLength;

    private PowerupManager thePowerupManager;
	Animator animator;
	public GameObject shieldParticles;
	public GameObject lifeParticles;
	public GameObject coinParticles;

	int powerSelector = 0;

    // Start is called before the first frame update
    void Awake()
    {
		thePowerupManager = FindObjectOfType<PowerupManager>();
		animator = GetComponent<Animator>();
    }

	public void SetPower(int powerupSelector)
	{
		gameObject.tag = "Untagged";
		FalseAll();
		switch (powerupSelector)
		{
			case 0:
				animator.SetInteger("Power", 0);
				shield = true;
				break;
			case 1:
				animator.SetInteger("Power", 1);
				life = true;
				break;
			case 2:
				animator.SetInteger("Power", 2);
				coin = true;
				tag = "coin";
				break;
			case 3:
				animator.SetInteger("Power", 3);
				magnet = true;
				break;
		}
		
		powerSelector = powerupSelector;
	}

    private void OnEnable()
    {
		gameObject.GetComponent<SpriteRenderer>().enabled = true;

	}

	private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Contains("Player") || collision.name.Contains("Fever"))
        {			
			GetComponent<Rigidbody2D>().velocity = Vector2.zero;

			switch (powerSelector)
			{
				case 0:
					shieldParticles.SetActive(true);
					shieldParticles.GetComponent<ParticleSystem>().Play();
					break;
				case 1:
					lifeParticles.SetActive(true);
					lifeParticles.GetComponent<ParticleSystem>().Play();
					break;
				case 2:
					coinParticles.SetActive(true);
					coinParticles.GetComponent<ParticleSystem>().Play();
					break;
			}

			thePowerupManager.ActivatePowerup(kunai, smokeBomb, onigiri, shield, life, coin, magnet, powerupLength, collision.gameObject);
			gameObject.GetComponent<SpriteRenderer>().enabled = false;
			gameObject.GetComponent<CoinMagnet>().magnetted = false;
			FalseAll();
		}
	}

    //prevent powerup from spawning on obstacles
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<ObjectDestroyer>().isActiveAndEnabled)
        {
            gameObject.SetActive(false);
        }
    }

    private void FalseAll()
    {
        kunai = false;
        smokeBomb = false;
        onigiri = false;
		shield = false;
		life = false;
		coin = false;

	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.name.Contains("branch"))
		{
			gameObject.SetActive(false);
		}else if (gameObject.tag.Contains("coin") && collision.name.Contains("power"))
		{
			collision.gameObject.SetActive(false);
		}
	}
}
