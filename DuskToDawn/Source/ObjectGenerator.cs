using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    public GameObject Tree;
	LifeController lifeController;
    public Transform generationPoint;
    private float distanceBetween;

    private float treeWidth;

    public float distanceBetweenMin;
    public float distanceBetweenMax;

    public Transform maxHeightPoint;
    public float maxHeightChange;
    private float heightChange;

	public float randomLifeThreshold;
	public float randomCloudThreshold;
	public float randomShieldThreshold;
	public float randomMagnetThreshold;
	public float randomCoinThreshold;
	public float randomBranchThreshold;
    public ObjectPooler[] branchPools;

    public float powerupHeight;
    public float powerupThreshold; //this is using used as obstacle threshold
    public ObjectPooler heartPowerPool;
    public ObjectPooler shieldPowerPool;
    public ObjectPooler magnetPowerPool;
	public ObjectPooler coinPowerpPool;

	public ObjectPooler cloudsPool;
    public ObjectPooler kunaiRPool;
    public ObjectPooler kunaiLPool;
	public ObjectPooler icePool;

	// Start is called before the first frame update
	void Start()
    {
        treeWidth = Tree.GetComponent<BoxCollider2D>().size.x;
		lifeController = GameObject.FindObjectOfType<LifeController>();
	}

	// Update is called once per frame
	void Update()
    {
		if (transform.position.y < generationPoint.position.y)
        {
			int branchSelector = Random.Range(0, branchPools.Length);
			List<GameObject> powerUps = new List<GameObject>();

			if (!GameSceneManager.instance.feverMode) 
			{
				distanceBetween = Random.Range(distanceBetweenMin, distanceBetweenMax);

				//select which difficulty branch prefab

				//add obstacle
				if (Random.Range(0f, 100f) < powerupThreshold)
				{
					StartCoroutine(CreateKunai());
					FindObjectOfType<PowerupManager>().GetComponent<AudioSource>()
						.PlayOneShot(FindObjectOfType<PowerupManager>().kunaiSFX);
				}

				if (lifeController.life < lifeController.Hearts.Length && Random.Range(0f, 100f) <
					(GameSceneManager.instance.scoreManager.scoreCount > 1000 ? randomLifeThreshold - 20 : randomLifeThreshold))
				{
					GameObject newPowerup = heartPowerPool.GetPooledObject();
					newPowerup.transform.position = transform.position + new Vector3((int)(Random.Range(-powerupHeight, powerupHeight)), distanceBetween / 2f, 0f);

					if (newPowerup.transform.position.x != 0)
					{
						newPowerup.SetActive(true);
						newPowerup.GetComponent<Powerups>().SetPower(1);

						powerUps.Add(newPowerup);
					}
				}

				if (Random.Range(0f, 100f) < randomShieldThreshold)
				{
					GameObject newPowerup3 = shieldPowerPool.GetPooledObject();
					newPowerup3.transform.position = transform.position + new Vector3((int)(Random.Range(-powerupHeight, powerupHeight)), Random.Range(distanceBetweenMin, distanceBetweenMax) / 2f, 0f);

					if (newPowerup3.transform.position.x != 0)
					{
						newPowerup3.SetActive(true);
						newPowerup3.GetComponent<Powerups>().SetPower(0);

						if (powerUps.Count > 0)
						{
							newPowerup3.SetActive(newPowerup3.transform.position.y < (powerUps[powerUps.Count - 1].transform.position.y + 0.5) &&
								newPowerup3.transform.position.y < (powerUps[powerUps.Count - 1].transform.position.y - 0.5));

							if (newPowerup3.activeInHierarchy)
							{
								powerUps.Add(newPowerup3);
							}
						}
					}
				}

				if (Random.Range(0f, 100f) < randomMagnetThreshold)
				{
					//magnet
					GameObject newPowerup4 = magnetPowerPool.GetPooledObject();
					newPowerup4.transform.position = transform.position + new Vector3((int)(Random.Range(-powerupHeight, powerupHeight)), Random.Range(distanceBetweenMin, distanceBetweenMax) / 2f, 0f);

					if (newPowerup4.transform.position.x != 0)
					{
						newPowerup4.SetActive(true);
						newPowerup4.GetComponent<Powerups>().SetPower(3);

						if (powerUps.Count > 0)
						{
							newPowerup4.SetActive(newPowerup4.transform.position.y < (powerUps[powerUps.Count - 1].transform.position.y + 0.5) &&
								newPowerup4.transform.position.y < (powerUps[powerUps.Count - 1].transform.position.y - 0.5));

							if (newPowerup4.activeInHierarchy)
							{
								powerUps.Add(newPowerup4);
							}
						}
					}
				}

				//add random cloud
				if (Random.Range(0, 80) < randomCloudThreshold)
				{
					CreateClouds();
				}
			}

			transform.position = new Vector3(transform.position.x, transform.position.y + distanceBetween, transform.position.z);

			if (Random.Range(0, 100f) < (GameSceneManager.instance.feverMode ? randomBranchThreshold + 20 : randomBranchThreshold))
			{
				GameObject newBranch = branchPools[branchSelector].GetPooledObject();

				float branchYPosition = Random.Range(distanceBetweenMin, distanceBetweenMax);

				float branchXPosition = 0f;

				switch (branchSelector)
				{
					case 0:
						branchXPosition = 0.25f;
						break;
					case 1:
						branchXPosition = 0.525f;
						break;
					case 2:
						branchXPosition = 0.652f;
						break;
				}

				float branchLocation = Random.Range(-100f, 100f);
				Vector3 branchPosition = transform.position;
				Vector3 coinPosition;
				if (branchLocation <= 0)  //left side of tree
				{
					newBranch.GetComponent<SpriteRenderer>().flipX = true;

					branchPosition = new Vector3(-branchXPosition, branchYPosition, 0f);

					if (Random.Range(0f, 100f) < powerupThreshold &&
						!GameObject.FindObjectOfType<PlayerController>().isFeverOn)
					{
						CreateIceFloor(1);
					}

					coinPosition = new Vector3(Random.Range(-2.0f, -2.5f), 0, 0);

				}
				else //right side of tree
				{
					newBranch.GetComponent<SpriteRenderer>().flipX = false;
					branchPosition = new Vector3(branchXPosition, branchYPosition, 0f);
					if (Random.Range(0f, 100f) < powerupThreshold &&
						!GameObject.FindObjectOfType<PlayerController>().isFeverOn)
					{
						CreateIceFloor(0);
					}
					coinPosition = new Vector3(Random.Range(2f, 2.5f), 0, 0);

				}

				newBranch.transform.position = transform.position + branchPosition;
				newBranch.SetActive(true);

				if (Random.Range(0f, 100f) < randomCoinThreshold)
				{
					GameObject newPowerup2 = coinPowerpPool.GetPooledObject();

					newPowerup2.transform.position = newBranch.transform.position + coinPosition;

					newPowerup2.SetActive(true);

					newPowerup2.GetComponent<Powerups>().SetPower(2);

					if (powerUps.Count > 0)
					{
						powerUps[powerUps.Count - 1].SetActive(powerUps[powerUps.Count - 1].transform.position.y < (newPowerup2.transform.position.y + 0.5) &&
							powerUps[powerUps.Count - 1].transform.position.y < (newPowerup2.transform.position.y - 0.5));

						if (newPowerup2.activeInHierarchy)
						{
							powerUps.Add(newPowerup2);
						}
					}
				}
			}

			transform.position = new Vector3(transform.position.x, transform.position.y + treeWidth, transform.position.z);
		}
    }

	public void CreateIceFloor(int side)
	{
		//int iceLocation = Random.Range(0, 2);
		GameObject newIce = icePool.GetPooledObject();
		
		switch (side)
		{
			case 0: //right side
				newIce.transform.position = new Vector2(0.17f, transform.position.y + Random.Range(.5f,2.5f));
				break;
			case 1: //left side
				newIce.transform.position = new Vector2(-0.17f, transform.position.y + Random.Range(.5f, 2.5f));
				break;
		}

		newIce.SetActive(true);
	}

    public void CreateClouds()
    {
        GameObject newClouds = cloudsPool.GetPooledObject();

        newClouds.transform.position = transform.position;
		Color col = newClouds.GetComponent<SpriteRenderer>().color;
		newClouds.GetComponent<SpriteRenderer>().color = new Color(col.r,col.g,col.b,0);

		newClouds.SetActive(true);
    }

    IEnumerator CreateKunai()
    {
		int kunaiLocation = Random.Range(0, 2);

		switch (kunaiLocation)
        {

            case 0:
                GameObject kunaiR = kunaiRPool.GetPooledObject();
				kunaiR.SetActive(true);
				kunaiR.transform.position = new Vector2(4, transform.position.y - Random.Range(3f,7f));
				float xPos = Random.Range(transform.position.x + 0.85f, transform.position.x - 0.25f);
				while (kunaiR.transform.position.x > xPos)
				{
					yield return new WaitForEndOfFrame();

					kunaiR.transform.position = Vector2.MoveTowards(kunaiR.transform.position,
					new Vector2(xPos, kunaiR.transform.position.y), 10 * Time.deltaTime);
				}

				kunaiR.transform.position = new Vector2(xPos, kunaiR.transform.position.y);

				break;
            case 1:
                GameObject kunaiL = kunaiLPool.GetPooledObject();
				kunaiL.SetActive(true);
				kunaiL.transform.position = new Vector2(-4, transform.position.y - Random.Range(3f, 7f));
				float xPosL = Random.Range(transform.position.x - 0.85f, transform.position.x + 0.25f);
				while (kunaiL.transform.position.x < xPosL)
				{
					yield return new WaitForEndOfFrame();

					kunaiL.transform.position = Vector2.MoveTowards(kunaiL.transform.position,
					new Vector2(xPosL, kunaiL.transform.position.y), 10 * Time.deltaTime);
				}

				kunaiL.transform.position = new Vector2(xPosL, kunaiL.transform.position.y);


				break;
        }
	}
}
