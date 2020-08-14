using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeverGenerator : MonoBehaviour
{
	public ObjectPooler feverPowerUpL;
	public ObjectPooler feverPowerUpR;

	public void GenerateFeverPower()
	{
		GameObject feverR = feverPowerUpR.GetPooledObject();
		GameObject feverL = feverPowerUpL.GetPooledObject();

		feverR.transform.position = GameSceneManager.instance.objectGenerator.transform.position + new Vector3(0.8f, 0, 0);
		feverL.transform.position = GameSceneManager.instance.objectGenerator.transform.position - new Vector3(0.8f, 0, 0);

		feverR.SetActive(true);
		feverL.SetActive(true);

	}
}
