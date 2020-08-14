using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSceneController : MonoBehaviour
{
	public static GameSceneController instance;
	public Dictionary<int, LevelConfigs> levelDetail;

	[SerializeField] List<GameObject> powerUpList = null;

	[SerializeField] Text level = null;
	[SerializeField] Text highscore = null;
	[SerializeField] Text money = null;
	[SerializeField] Text minionNum = null;
	[SerializeField] Image meterBar = null;

	[SerializeField] GameObject warning = null;
	[SerializeField] GameObject alertAnim = null;
	[SerializeField] GameObject minionPool = null;
	[SerializeField] GameObject feverLight = null;
	[SerializeField] GameObject crack = null;
	[SerializeField] GameObject explosion = null;
	[SerializeField] GameObject bomMinion = null;
	[SerializeField] SpriteRenderer ship = null;
	[SerializeField] Animator feverBG = null;

	[SerializeField] AudioClip levelComplete = null;
	[SerializeField] AudioClip bgm = null;
	[SerializeField] AudioClip alien = null;
	[SerializeField] AudioClip chicken = null;
	[SerializeField] AudioClip reverse = null;
	[SerializeField] AudioClip feverTime = null;

	public GameObject ufo = null;
	public GameObject hostageObject = null;
	public PickUp.PickUpType pickUpStore = PickUp.PickUpType.Boomerang;

	public int currentLevel = 1;
	public bool isFever = false;
	public bool hasRevived = false;
	public bool isPlayable = false;
	public float feverDuration = 6;
	public bool hasPickUpStored = false;

	int currentLevelMinionObj = 0;
	int shotLevelMinionObj = 0;
	int score = 0;
	float reverseTimer = -1;
	Coroutine feverRoutine = null;
	
	// Start is called before the first frame update
	private void Awake()
	{
		string data = @"
{'1':{'objectivePercent':0.5,'minionNumber':'10'},'2':{'objectivePercent':0.6,'minionNumber':'11','reverseTimer':15},'3':{'objectivePercent':0.7,'minionNumber':'12','reverseTimer':20},'4':{'objectivePercent':0.8,'minionNumber':'13','reverseTimer':15},'5':{'objectivePercent':0.9,'minionNumber':'15','reverseTimer':20,'isBossStage':true}}
		";

		if (instance == null) instance = this;
		else if (instance != this) Destroy(gameObject);

		levelDetail = Encoder.jsonDecode<Dictionary<int, LevelConfigs>>(data);
	}

	void Start()
    {
		highscore.text = 0.ToString();
		money.text = GameManager.instance.playerData.playerCoin.ToString();

		ship.sprite = Resources.Load<Sprite>("Ships/" + GameManager.instance.playerData.chosenSkin);

		StartPopulateLevel();
	}

	public void CommonButtonSound()
	{
		SoundManager.instance.PlayButtonSound();
	}

	public void UpdateScore(int minionScore)
	{
		score += minionScore;

		highscore.text = score.ToString();
	}

	public IEnumerator PopulateNewLevel()
	{
		if (isFever)
		{
			isFever = false;
			feverLight.SetActive(false);
			feverBG.SetTrigger("FeverOff");
			SoundManager.instance.PlayBGM(bgm);
			FindObjectOfType<BulletManager>().ChangeBulletType(BulletManager.BulletType.Default);

			StopCoroutine(feverRoutine);
		}

		hostageObject.SetActive(false);
		level.text = currentLevel.ToString();
		minionNum.transform.parent.gameObject.SetActive(false);

		yield return PlanetPivot.instance.StartCoroutine(PlanetPivot.instance.RotatePlanet());

		alertAnim.GetComponent<Animator>().SetTrigger("Alert");
		SoundManager.instance.PlaySFX(chicken);
		yield return new WaitForSeconds(2f);

		yield return FadeController.FadeIn(ufo.GetComponent<SpriteRenderer>());
		SoundManager.instance.PlaySFX(alien);

		PopulateLevelInfo();
		minionNum.transform.parent.gameObject.SetActive(true);

	}

	private void FixedUpdate()
	{
		if(isPlayable && reverseTimer != -1)
		{
			if (reverseTimer > 3f)
			{
				reverseTimer -= Time.fixedDeltaTime;
			}
			else
			{
				StartCoroutine(PlayAlertSound());

                if (levelDetail.ContainsKey(currentLevel))
                {
                    reverseTimer = levelDetail[currentLevel].reverseTimer;
                }
                else {
                    reverseTimer = levelDetail.Last().Value.reverseTimer;
                }

			}
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (SceneManager.sceneCount == 1)
			{
				PauseGame();

				//SceneManager.UnloadSceneAsync(8);
			}
		}
	}

	IEnumerator PlayAlertSound()
	{
		warning.SetActive(true);
		for (int i = 0; i < 6; i++)
		{
			warning.SetActive(true);
			SoundManager.instance.PlaySFX(reverse);
			yield return new WaitForSeconds(.5f);
			warning.SetActive(false);
		}

		warning.SetActive(false);
		FindObjectOfType<PlanetManager>().isReverse = !FindObjectOfType<PlanetManager>().isReverse;
	}


	public void PopulateLevelInfo()
	{		
		float round = Mathf.FloorToInt(currentLevel / 5) * 0.5f + 1f;

		int currentLevelMinionNumber = 0;

		switch (currentLevel % 5)
		{
			case 1:
				reverseTimer = levelDetail[1].reverseTimer;
				currentLevelMinionNumber = Mathf.FloorToInt(levelDetail[1].minionNumber * round);
				currentLevelMinionObj = Mathf.FloorToInt(currentLevelMinionNumber * levelDetail[1].objectivePercent);
				break;
			case 2:
				reverseTimer = levelDetail[2].reverseTimer;
				currentLevelMinionNumber = Mathf.FloorToInt(levelDetail[2].minionNumber * round);
				currentLevelMinionObj = Mathf.FloorToInt(currentLevelMinionNumber * levelDetail[2].objectivePercent);
				break;
			case 3:
				reverseTimer = levelDetail[3].reverseTimer;
				currentLevelMinionNumber = Mathf.FloorToInt(levelDetail[3].minionNumber * round);
				currentLevelMinionObj = Mathf.FloorToInt(currentLevelMinionNumber * levelDetail[3].objectivePercent);
				break;
			case 4:
				reverseTimer = levelDetail[4].reverseTimer;
				currentLevelMinionNumber = Mathf.FloorToInt(levelDetail[4].minionNumber * round);
				currentLevelMinionObj = Mathf.FloorToInt(currentLevelMinionNumber * levelDetail[4].objectivePercent);
				break;
			case 0:
				reverseTimer = levelDetail[5].reverseTimer;
				currentLevelMinionNumber = Mathf.FloorToInt(levelDetail[5].minionNumber * round);
				currentLevelMinionObj = Mathf.FloorToInt(currentLevelMinionNumber * levelDetail[5].objectivePercent);
				break;
		}
		//Debug.Log("currentLevelMinionNumber ============================== " + currentLevelMinionNumber);
		//Debug.Log("currentLevelMinionObj ============================== " + currentLevelMinionObj);
		minionNum.text = "0/" + currentLevelMinionObj;

		FindObjectOfType<PlanetManager>().PopulatePlanet(currentLevelMinionNumber);
	}

	public void UpdateMinionCount()
	{
		shotLevelMinionObj++;
		minionNum.text = shotLevelMinionObj + "/" + currentLevelMinionObj;
		UpdateMoneyText();

		FindObjectOfType<BulletManager>().needSlowmo = (shotLevelMinionObj == currentLevelMinionObj-1);

		if (shotLevelMinionObj == currentLevelMinionObj)
		{
			minionNum.transform.parent.gameObject.SetActive(false);

			SoundManager.instance.PlaySFX(levelComplete);
			isPlayable = false;
			FindObjectOfType<BulletManager>().needSlowmo = false;
			currentLevel++;

			shotLevelMinionObj = 0;

			GameManager.instance.playerData.SaveData();

			foreach (Transform child in minionPool.transform)
			{
				if (child.gameObject.activeInHierarchy)
				{
					child.GetComponent<MinionManager>().IsShot();
				}
			}

			ClearLevelActiveObj();

			Invoke("StartPopulateLevel", .8f);
		}
	}

	public void ClearLevelActiveObj()
	{
		foreach(GameObject obj in powerUpList)
		{
			foreach (Transform child in obj.transform)
			{
				child.gameObject.SetActive(false);
			}
		}

		foreach(Transform child in bomMinion.transform)
		{
			if (child.gameObject.activeInHierarchy)
			{
				child.gameObject.SetActive(false);
			}
		}
	}

	public void StartPopulateLevel()
	{
		StartCoroutine(PopulateNewLevel());
	}

	public void UpdateMoneyText()
	{
		money.text = GameManager.instance.playerData.playerCoin.ToString();
	}

	public void CrackPlanet(bool isCrack)
	{
		minionNum.transform.parent.gameObject.SetActive(!isCrack);
		explosion.SetActive(isCrack);
		crack.SetActive(isCrack);
	}

	public void GameOver()
	{
		GameManager.instance.finalScore = score;
		GameManager.instance.lastLevel = currentLevel;

		if (hasRevived)
		{
			TransitionManager.instance.SwitchScene(6);
		}
		else
		{
			SceneManager.LoadScene(5, LoadSceneMode.Additive);
			hasRevived = true;
		}
	}

	public void IncreaseFeverMeter()
	{
		if (!isFever)
		{
			meterBar.fillAmount += 0.15f;
			if (meterBar.fillAmount == 1)
			{
				FindObjectOfType<BulletManager>().ChangeBulletType(BulletManager.BulletType.Ulti);

				isFever = true;
				feverBG.SetTrigger("FeverOn");
				feverLight.SetActive(true);
				feverRoutine = StartCoroutine(StartFeverCountDown());
			}
		}
	}

	IEnumerator StartFeverCountDown()
	{
		SoundManager.instance.PlayBGM(feverTime);

		float sec = feverDuration;
		if (isFever)
		{
			while (meterBar.fillAmount > 0 && sec > 0)
			{
				if (isPlayable)
				{
					sec -= Time.deltaTime;
					meterBar.fillAmount = sec / feverDuration;
				}

				yield return new WaitForEndOfFrame();
			}
		}
		
		feverLight.SetActive(false);
		feverBG.SetTrigger("FeverOff");
		isFever = false;
		SoundManager.instance.PlayBGM(bgm);

		if (hasPickUpStored)
		{
			switch (pickUpStore)
			{
				case PickUp.PickUpType.Boomerang:
					FindObjectOfType<BulletManager>().ChangeBulletType(BulletManager.BulletType.Boomerang);
					break;
				case PickUp.PickUpType.Laser:
					FindObjectOfType<BulletManager>().ChangeBulletType(BulletManager.BulletType.Laser);
					break;
			}

			hasPickUpStored = false;
		}
		else
		{
			FindObjectOfType<BulletManager>().ChangeBulletType(BulletManager.BulletType.Default);
		}
	}

	public void PauseGame()
	{
		if(SceneManager.sceneCount == 1)
		{
			isPlayable = false;
			SceneManager.LoadSceneAsync(8, LoadSceneMode.Additive);
		}
	}

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
            PauseGame();
    }
}
