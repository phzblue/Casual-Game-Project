using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText;
    //public Text highScoreText;

    [HideInInspector]
    public float scoreCount;

	public float pointsPerSecond;
    public bool scoreIncreasing;

	public int num = 300;
	public int term = 1;

	public GameObject highscoreMarque;
	public GameObject highscoreTab;
	public GameObject mileStonetab;
	public GameObject rewardtab;

	string rewardText = "";

    //public bool shouldDouble;

    // Start is called before the first frame update
    //void Start()
    //{
        //if (PlayerPrefs.HasKey("HighScore"))
        //{
        //    highScoreCount = PlayerPrefs.GetFloat("HighScore");
        //}
    //}

    // Update is called once per frame
    void Update()
    {
        if (scoreIncreasing)
        {
            scoreCount += pointsPerSecond * Time.deltaTime;
            scoreText.text = Mathf.Round(scoreCount).ToString();

			if(Mathf.Round(scoreCount) == 100){
				GameObject.FindObjectOfType<FeverGenerator>().GenerateFeverPower();
			}else if (Mathf.Round(scoreCount) == (num * term))
			{
				term++;
				GameObject.FindObjectOfType<FeverGenerator>().GenerateFeverPower();
			}

			if (LeaderboardController.instance.playerHighScore != 0 &&
				(long)Mathf.Round(scoreCount) == LeaderboardController.instance.playerHighScore)
			{
				highscoreMarque.SetActive(true);
				highscoreTab.SetActive(true);
			}

			rewardText = GameConfig.CheckFreeSkinFromScore((int)Mathf.Round(scoreCount));

			if (!rewardText.Equals(""))
			{
				rewardtab.GetComponent<TabMovment>().SetTextInTab(rewardText);
				rewardtab.SetActive(true);
			}

			if (scoreCount >= 600 && scoreCount < 602)
			{
				GameSceneManager.instance.objectGenerator.GetComponent<ObjectGenerator>().randomBranchThreshold = 60;
			}

			if(Mathf.Round(scoreCount) > 0 && Mathf.Round(scoreCount) % 500 == 0 && Mathf.Round(scoreCount) > GameSceneManager.instance.playerData.lastMileStone)
			{
				GameManager.instance.playerData.lastMileStone = Mathf.RoundToInt(scoreCount);
				mileStonetab.GetComponent<TabMovment>().SetTextInTab(Mathf.Round(scoreCount).ToString());
				mileStonetab.SetActive(true);
			}
			
		}
		//highScoreText.text = "High Score: " + Mathf.Round(highScoreCount);
	}
    public void AddScore(int pointsToAdd)
    {
        scoreCount += pointsToAdd;
    }
}
