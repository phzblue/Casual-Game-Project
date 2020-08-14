using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardCheck : MonoBehaviour
{
	public GameObject entryContainer;
	public GameObject emptyText;

	private void OnEnable()
	{
		if (entryContainer.transform.childCount > 0)
		{
			emptyText.SetActive(false);
		}
		else
		{
			emptyText.SetActive(true);
		}
	}
}
