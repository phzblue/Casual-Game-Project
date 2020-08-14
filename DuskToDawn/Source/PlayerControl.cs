using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private PlayerController pc;
	private PowerupManager powerupManager;
	public SpriteStateAnimator spriteStateAnimator;
	private SpriteRenderer spriteRenderer;

    public float jumpForce;
    public float jumpTime;
    private float jumpTimeCounter;

    private float invulnerableDuration = 2f;
    private int speedBoostTotal = 0;

    private bool stoppedJumping = true;
	private bool canDoubleJump = true; 

	public bool magnetOn;

	private Rigidbody2D myRigidBody;
    public bool invulnerable;

    public bool grounded;
    public LayerMask whatIsGround;
    public Transform groundCheck;
    public float groundCheckRadius;

    public GameObject[] shadows;
	public GameObject shield;
	public GameObject trail;
	public GameObject magnetParticles;

	public bool ateFever = false;
	public bool feverMood = false;
	public GameObject FeverFire;
	public int feverDuration;

	public GameObject deathParticle;
	[SerializeField]private BoxCollider2D selfCollider2D;

	List<Coroutine> coroutines = new List<Coroutine>();

    private void Awake()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
		powerupManager = GameObject.FindObjectOfType<PowerupManager>();
		spriteStateAnimator = GetComponent<SpriteStateAnimator>();
		selfCollider2D = GetComponent<BoxCollider2D>();
	}

	private void Start()
    {
		jumpTimeCounter = jumpTime;

		pc = PlayerController.instance;
		spriteRenderer = GetComponent<SpriteRenderer>();

		if (gameObject.name.Contains("L1"))
		{
			spriteStateAnimator.SetSkinID(GameManager.instance.tempRanRunnerIDL).StartAnimations("Idle");
			foreach (GameObject shadow in shadows)
			{
				shadow.gameObject.GetComponent<SpriteRenderer>().sprite = spriteStateAnimator.GetIdleSprite();
			}
		}
		else
		{
			spriteStateAnimator.SetSkinID(GameManager.instance.tempRanRunnerIDR).StartAnimations("Idle");
			foreach (GameObject shadow in shadows)
			{
				shadow.gameObject.GetComponent<SpriteRenderer>().sprite = spriteStateAnimator.GetIdleSprite();
			}
		}
	}

    // Update is called once per frame
    void Update()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        if (grounded)
        {
			if (spriteStateAnimator.currentState.Equals("Jump"))
			{
				if (myRigidBody.velocity.y > 1f)
				{
					spriteStateAnimator.StartAnimations("Run");
				}
				else
				{
					spriteStateAnimator.StartAnimations("Idle"); ;
				}
			}


		}
		else
		{
			spriteStateAnimator.StartAnimations("Jump");
		}
    }

    private void FixedUpdate()
    {
        if (!stoppedJumping)
        {
            if (jumpTimeCounter > 0)
            {
                myRigidBody.velocity = new Vector2(jumpForce, pc.moveSpeed);
                jumpTimeCounter -= Time.deltaTime;
            }
        }
    }

	public void EnableDeathParticle()
	{
		spriteRenderer.enabled = false;
		deathParticle.SetActive(true);
		deathParticle.GetComponent<ParticleSystem>().Play();

		foreach (GameObject shadow in shadows)
		{
			shadow.SetActive(false);
		}
	}

    public void UpdateVelocity()
    {
		myRigidBody.velocity = new Vector2(0, pc.moveSpeed);
    }

    public void SetVelocity(float moveSpeed)
    {
        myRigidBody.velocity = new Vector2(0, moveSpeed);
    }

    public void ResetPlayer()
    {
		GetComponentInChildren<TrailRenderer>().enabled = false;

		//reset flash effect
		StopAllCoroutines();

		foreach(Coroutine co in coroutines)
		{
			StopCoroutine(co);
		}

		coroutines.Clear();

		GetComponent<SpriteRenderer>().enabled = true;
		invulnerable = false;
        speedBoostTotal = 0;
		spriteStateAnimator.StartAnimations("Death");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
		if (!feverMood && collision.IsTouching(selfCollider2D))
		{
			if (collision.gameObject.tag == "killbox")
			{
				if (!invulnerable)
				{
#if UNITY_WEBGL
#else
					Handheld.Vibrate();
#endif

					pc.obstacleSound.Play();
					StartCoroutine(Flashing());
					pc.theLifeController.ReduceLife(collision.gameObject);
				}
			}
			else if (collision.gameObject.tag == "ice")
			{
				powerupManager.ActivatePowerup(false, false, true, false, false, false,false, 3.5f, gameObject); //3.5f
			}
			else if (collision.gameObject.tag == "Cloud")
			{
				collision.GetComponent<Animator>().SetTrigger("CloudAppear");
				collision.transform.GetChild(0).gameObject.SetActive(true);
				collision.transform.GetChild(0).GetComponent<ParticleSystem>().Play(true);
			}
			else if (collision.gameObject.name.Equals("FadeBox"))
			{
				collision.GetComponentInParent<Animator>().SetTrigger("Fade");
			}
			else if (GameSceneManager.instance.isPlayable && collision.gameObject.name.Contains("Fever"))
			{
				ateFever = true;
				pc.EnableFeverMode();
			}
		}	
	}

	public IEnumerator ActivateFever()
	{
		ateFever = false;
		feverMood = true;
		trail.SetActive(false);
		FeverFire.SetActive(true);

		for (float timer = feverDuration; timer > 1; timer -= Time.deltaTime)
		{
			yield return null;
		}

		int i = 4;
		while(i > 0)
		{
			i--;
			FeverFire.SetActive(!FeverFire.activeSelf);
			yield return new WaitForSeconds(0.25f);
		}
		
		feverMood = false;
		pc.FeverIncrease(false);
		trail.SetActive(true);
		FeverFire.SetActive(false);
		ateFever = false;
	}


	public void ActivateSpeedBoost(float time)
    {
		spriteStateAnimator.StartAnimations("SpeedBoost");
		// add speed boost framerate change

		foreach (GameObject shadow in shadows)
        {
            shadow.SetActive(true);
        }

		Coroutine speedBoost = StartCoroutine(SpeedBoostSequence(time));
		coroutines.Add(speedBoost);
	} 

    public IEnumerator SpeedBoostSequence(float speedBoostTimer)
    {
        speedBoostTotal++;
        yield return StartCoroutine(translateOverTime(3.5f, new Vector3(1,0,0) * 1.5f)); //0.3
		//yield return new WaitForSeconds(speedBoostTimer);
		for (float timer = speedBoostTimer; timer >= 0; timer -= Time.deltaTime)
		{
			yield return null;
		}
		yield return StartCoroutine(translateOverTime(3.5f, new Vector3(-1, 0, 0) * 1.5f)); //0.3
        speedBoostTotal--;

        if (speedBoostTotal == 0)
        {
			// add speed boost framerate change
			spriteStateAnimator.StartAnimations("Run");

			foreach (GameObject shadow in shadows)
            {
                shadow.SetActive(false);
            }
        }
    }

    IEnumerator translateOverTime(float duration, Vector3 distance)
    {
        float elapsedTime = 0;
        float lastActedTime = 0;
        float translateIncrements;

        while (elapsedTime < duration)
        {
            translateIncrements = (elapsedTime - lastActedTime) / duration;
            transform.Translate(distance * translateIncrements);
            //transform.position += distance * translateIncrements;
            lastActedTime = elapsedTime;
            yield return null;
            elapsedTime += Time.deltaTime;
        }

        translateIncrements = (duration - lastActedTime) / duration;
        transform.Translate(distance * translateIncrements);
    }

    public IEnumerator Flashing(float duration = 0f)
    {
        invulnerable = true;
		float timer;
		if (System.Math.Abs(duration) > 0)
		{
			timer = duration;
		}
		else
		{
			timer = invulnerableDuration;
		}
		while (timer >= 0)
		{
			timer -= 0.2f;
			//timer -= Time.deltaTime;

			spriteRenderer.enabled = !spriteRenderer.enabled;

			yield return new WaitForSeconds(0.2f);
		}

		spriteRenderer.enabled = true;
		invulnerable = false;
    }

	public IEnumerator ShieldUp()
	{
		shield.SetActive(true);
		invulnerable = true;

		yield return new WaitForSeconds(2f);
		
		int i = 4;
		while(i > 0)
		{
			i--;
			shield.SetActive(!shield.activeSelf);
			yield return new WaitForSeconds(0.15f);
		}
		shield.SetActive(false);
		invulnerable = false;
	}

	public IEnumerator MagnetOn()
	{
		magnetOn = true;
		magnetParticles.SetActive(true);
		yield return new WaitForSeconds(5.5f);
		magnetParticles.SetActive(false);
		magnetOn = false;
	}

	public void JumpKeyDown()
    {
        if (grounded && GameSceneManager.instance.isPlayable)
        {
			jumpTimeCounter = jumpTime;
			canDoubleJump = true;
			stoppedJumping = false;
            pc.jumpSound.Play();
        }

		//uncomment for double jump
		if (!grounded && canDoubleJump && GameSceneManager.instance.isPlayable)
		{
			canDoubleJump = false;
			jumpTimeCounter = jumpTime;
			//myRigidBody.velocity = new Vector2(jumpForce, otherPlayer.velocity.y);
			myRigidBody.velocity = new Vector2(jumpForce, pc.moveSpeed);
			stoppedJumping = false;
			pc.jumpSound.Play();
		}
	}

    public void JumpKeyUp()
    {
        jumpTimeCounter = 0;
        stoppedJumping = true;
    }
}
