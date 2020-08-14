using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeController : MonoBehaviour
{
    public GameObject[] Hearts;
    private GameSceneManager gameManager;
	public int life = 0;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameSceneManager.instance;
		life = Hearts.Length;
	}

    public void ResetLife()
    {
		life = Hearts.Length;
		foreach (GameObject heart in Hearts)
        {
            heart.SetActive(true);
		}
		ReColorHeart();
	}

	public void Revive()
	{
		life = 3;
		ReColorHeart();
	}

	public void ReColorHeart()
	{
		for (int i = 0; i < Hearts.Length; i++)
		{
			if (i < life)
			{
				Hearts[i].GetComponent<Image>().color = new Color(Hearts[i].GetComponent<Image>().color.r,
					Hearts[i].GetComponent<Image>().color.g, Hearts[i].GetComponent<Image>().color.b,
					1);
			}
			else
			{
				Hearts[i].GetComponent<Image>().color = new Color(Hearts[i].GetComponent<Image>().color.r,
					Hearts[i].GetComponent<Image>().color.g, Hearts[i].GetComponent<Image>().color.b,
					.3f);
			}
		}
	}

    // Update is called once per frame
    public bool ReduceLife(GameObject collider)
    {
		life--;
		ReColorHeart();
		if (life == 0)
		{
			gameManager.pc.EnableDeathParticle();
			gameManager.isPlayable = false;
			gameManager.GameEnd();
			collider.SetActive(false);
			return true;
		}
		return false;
	}

	public void IncreaseLife()
	{
		if (GameSceneManager.instance.isPlayable && life < Hearts.Length)
		{
			life++;
		}
		
		ReColorHeart();
	}
}
