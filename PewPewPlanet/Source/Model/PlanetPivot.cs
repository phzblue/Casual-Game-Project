using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlanetPivot : MonoBehaviour
{
	public static PlanetPivot instance;

	private void Awake()
	{
		if (instance == null) instance = this;
		else if (instance != this) Destroy(gameObject);
	}

	public IEnumerator RotatePlanet()
	{
		transform.DORotate(new Vector3(0, 0, 360), 1.5f,RotateMode.LocalAxisAdd);
		yield return new WaitForSeconds(.8f);
		FindObjectOfType<PlanetManager>().GetComponent<SpriteRenderer>().color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f,1,1);
		yield return new WaitForSeconds(.7f);
		transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
	}
}
