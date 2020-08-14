using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    //private bool doublePoints;
    private bool kunai;
    private bool smokeBomb;
    private bool onigiri;
	private bool shield;
	private bool life;
	private bool coin;
	private bool magnet;

	private bool powerupActive;

    private float powerupLengthCounter;

	private ScoreManager theScoreManager;
	private ObjectGenerator theObjGenerator;

    private float normalPointsPerSecond;

    private ObjectDestroyer[] kunaiList;

    private GameSceneManager theGameSceneManager;

    public AudioClip kunaiSFX;
    public AudioClip smokeBombSFX;
    public AudioClip onigiriSFX;
	public AudioClip shieldSFX;
	public AudioClip coinSFX;
	public AudioClip lifeSFX; 
	public AudioClip magnetSFX; 

	private AudioSource powerupSFX;

    // Start is called before the first frame update
    void Start()
    {
		theScoreManager = FindObjectOfType<ScoreManager>();
		theObjGenerator = FindObjectOfType<ObjectGenerator>();
		theGameSceneManager = GameSceneManager.instance;
        powerupSFX = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (powerupActive)
        {
            powerupLengthCounter -= Time.deltaTime;

            if (theGameSceneManager.powerupReset)
            {
                powerupLengthCounter = 0;
				theGameSceneManager.powerupReset = false;
            }

            if (kunai)
            {
                powerupLengthCounter = 0;
                //theObjGenerator.CreateKunai();
            }

            if (smokeBomb)
            {
                powerupLengthCounter = 0;
                theObjGenerator.CreateClouds();
            }

            if (powerupLengthCounter <= 0)
            {
                //theScoreManager.pointsPerSecond = normalPointsPerSecond;
                //theScoreManager.shouldDouble = false;

                //theObjGenerator.randomBranchThreshold = branchRate;

                powerupActive = false;
            }
        }
    }

    public void ActivatePowerup(bool isKunai, bool isSmokeBomb, bool isOnigiri, bool isShield, bool isLife, bool isCoin, bool isMagnet, float time, GameObject player)
    {
        kunai = isKunai;
        smokeBomb = isSmokeBomb;
        onigiri = isOnigiri;
		shield = isShield;
		life = isLife;
		coin = isCoin;
        powerupLengthCounter = time;
		magnet = isMagnet;

		if (shield && !GameSceneManager.instance.feverMode)
		{
			powerupSFX.PlayOneShot(shieldSFX);
			StopCoroutine(player.GetComponent<PlayerControl>().ShieldUp());
			StartCoroutine(player.GetComponent<PlayerControl>().ShieldUp());
		}
		else if (life)
		{
			powerupSFX.PlayOneShot(lifeSFX);
			GameObject.FindObjectOfType<LifeController>().IncreaseLife();
		}else if (onigiri)
		{
			powerupSFX.PlayOneShot(onigiriSFX);
			player.GetComponent<PlayerControl>().ActivateSpeedBoost(powerupLengthCounter);
		}else if (coin)
		{
			powerupSFX.PlayOneShot(coinSFX);
			GameSceneManager.instance.CoinControl();
		}
		else if (magnet && !GameSceneManager.instance.feverMode)
		{
			powerupSFX.PlayOneShot(magnetSFX);
			StartCoroutine(player.GetComponent<PlayerControl>().MagnetOn());
		}

		powerupActive = true;
    }
}
