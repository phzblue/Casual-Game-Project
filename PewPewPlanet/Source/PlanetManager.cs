using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
	[SerializeField] ObjectPooler smallMinionPool = null;
	[SerializeField] ObjectPooler mediumMinionPool = null;
	[SerializeField] ObjectPooler largeMinionPool = null;
	[SerializeField] ObjectPooler hostageMinionPool = null;
	[SerializeField] List<ObjectPooler> pickUpPools = null;

	int enermyNum = 1;

	public bool isReverse = false;
	public GameObject[] wayPoints = null;

	public void PopulatePlanet(int minionNum)
	{
		enermyNum = minionNum;
		//enermyNum = 30;
		StartCoroutine(SpawnEnermies());
	}

	IEnumerator SpawnEnermies()
	{
		Time.timeScale = 5f;
		GameObject minion = null;

		for(int i=0; i<enermyNum; i++)
		{
			switch (Random.Range(0, 3))
			{
				case 0:
					minion = smallMinionPool.GetPooledObject();
					break;
				case 1:
					minion = mediumMinionPool.GetPooledObject();
					break;
				case 2:
					minion = largeMinionPool.GetPooledObject();
					break;
			}

			minion.SetActive(true);
			minion.GetComponent<MinionManager>().ResetMinion();

			yield return new WaitForSeconds(.8f);
		}

		if(GameSceneController.instance.currentLevel / 1 > 0)
		{
			GameSceneController.instance.hostageObject.SetActive(true);

			GameObject hostage1;

			int hostageNum = Mathf.FloorToInt(GameSceneController.instance.currentLevel / 1);

			for(int i = 0; i < hostageNum; i++)
			{
				hostage1 = hostageMinionPool.GetPooledObject();
				hostage1.SetActive(true);
				hostage1.GetComponent<PickUp>().StartMoving();

				yield return new WaitForSeconds(.8f);
			}
		}

		Time.timeScale = 1f;

		GameObject pickUp = pickUpPools[Random.Range(0, pickUpPools.Count)].GetPooledObject();
		pickUp.SetActive(true);
		pickUp.GetComponent<PickUp>().StartMoving();

		yield return FadeController.FadeOut(GameSceneController.instance.ufo.GetComponent<SpriteRenderer>());

		GameSceneController.instance.isPlayable = true;
	}

}
