using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public float moveSpeed;  //max speed possible before physics issue = 645
    [HideInInspector]
    public float moveSpeedStore;
    public float speedMultiplier;

    public float speedIncreaseMilestone;
    [HideInInspector]
    public float speedIncreaseMilestoneStore;

	[HideInInspector]
	public float speedMilestoneCount;
	[HideInInspector]
	public float speedMilestoneCountStore;
	float beforeFeverSpeedMilestone;

    private Rigidbody2D myRigidBody;

    public PlayerControl playerL;
    public PlayerControl playerR;

    public LifeController theLifeController;

    public AudioSource jumpSound;
    public AudioSource deathSound;
	public AudioSource obstacleSound;

	public bool pause = false;

	public GameObject lightParticle;
	public bool isFeverOn = false;
	public GameObject bgImage;

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

		myRigidBody = GetComponent<Rigidbody2D>();
	}

    private void Start()
    {
        theLifeController = FindObjectOfType<LifeController>();
        
    }

    void OnEnable()
    {
		if (!pause)
		{
			speedMilestoneCount = speedIncreaseMilestone;

			moveSpeedStore = moveSpeed;
			speedMilestoneCount = speedIncreaseMilestone;
			speedMilestoneCountStore = speedMilestoneCount;
			speedIncreaseMilestoneStore = speedIncreaseMilestone;
		}
		else
		{
			pause = false;
		}
        
    }

    // Update is called once per frame
    void Update()
    {
		if (GameSceneManager.instance.isPlayable && transform.position.y > speedMilestoneCount)
		{
			speedMilestoneCount += speedIncreaseMilestone;

			speedIncreaseMilestone = speedIncreaseMilestone * speedMultiplier;

			if(moveSpeed < 9)
			{
				moveSpeed = moveSpeed * speedMultiplier;
			}
			myRigidBody.velocity = new Vector2(myRigidBody.velocity.x, moveSpeed);
			playerL.UpdateVelocity();
			playerR.UpdateVelocity();
		}
		
    }

	public void FeverIncrease(bool on)
	{
		GameSceneManager.instance.FeverBgmToggle(on);

		if (on)
		{
			bgImage.GetComponent<Animator>().speed = 5;

			GameSceneManager.instance.feverMode = true;

			beforeFeverSpeedMilestone = moveSpeed;
			moveSpeed = moveSpeed * 3;
			myRigidBody.velocity = new Vector2(myRigidBody.velocity.x, moveSpeed);
			playerL.UpdateVelocity();
			playerR.UpdateVelocity();
		}
		else
		{
			bgImage.GetComponent<Animator>().speed = 1;

			GameSceneManager.instance.feverMode = false;

			lightParticle.SetActive(false);

			speedIncreaseMilestone = speedIncreaseMilestone * speedMultiplier;
			moveSpeed = beforeFeverSpeedMilestone * speedMultiplier;
			myRigidBody.velocity = new Vector2(myRigidBody.velocity.x, moveSpeed);
			playerL.UpdateVelocity();
			playerR.UpdateVelocity();
		}
	}

	public void EnableDeathParticle()
	{
		playerR.EnableDeathParticle();
		playerL.EnableDeathParticle();

		deathSound.Play();
	}

	public void ResetFocuser()
    {
		playerR.JumpKeyUp();
        playerL.JumpKeyUp();

		playerR.StopAllCoroutines();
		playerL.StopAllCoroutines();

		playerR.SetVelocity(0f);
        playerL.SetVelocity(0f);
		
		playerR.ResetPlayer();
		playerL.ResetPlayer();

		if (!pause)
		{
			moveSpeed = moveSpeedStore;
			speedMilestoneCount = speedMilestoneCountStore;
			speedIncreaseMilestone = speedIncreaseMilestoneStore;
		}
	}

	public void ResetPlayerXPos(Vector3 playerRStartPoint, Vector3 playerLStartPoint)
	{
		playerR.spriteStateAnimator.StartAnimations("Idle");
		playerL.spriteStateAnimator.StartAnimations("Idle");

		playerR.transform.position = new Vector3(.5f, playerRStartPoint.y, 0);
		playerL.transform.position = new Vector3(-.5f, playerLStartPoint.y, 0);
	}

	public void EnableFlash(float timer = 0f)
	{
		StartCoroutine(playerL.Flashing(timer));
		StartCoroutine(playerR.Flashing(timer));
	}

	public void EnableFeverMode()
	{
		if(playerL.ateFever || playerR.ateFever)
		{
			lightParticle.SetActive(true);

			StartCoroutine(playerL.ActivateFever());
			StartCoroutine(playerR.ActivateFever());

			FeverIncrease(true);

		}
	}

    public void InitPlayers(bool isMove)
    {
        if (isMove)
        {
			playerR.spriteStateAnimator.StartAnimations("Run");
			playerL.spriteStateAnimator.StartAnimations("Run");

			myRigidBody.velocity = new Vector2(0, moveSpeed);
            playerL.UpdateVelocity();
            playerR.UpdateVelocity();

			playerR.GetComponentInChildren<TrailRenderer>().enabled = true;
			playerL.GetComponentInChildren<TrailRenderer>().enabled = true;

		}
		else
        {
			playerR.spriteStateAnimator.StartAnimations("Idle");
			playerL.spriteStateAnimator.StartAnimations("Idle");

			myRigidBody.velocity = Vector2.zero;
            playerL.SetVelocity(0);
            playerR.SetVelocity(0);

			playerR.GetComponentInChildren<TrailRenderer>().enabled = false;
			playerL.GetComponentInChildren<TrailRenderer>().enabled = false;
		}
    }
}
