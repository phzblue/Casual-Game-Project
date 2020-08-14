using System.Collections;
using com.igi.game.common.webserver.hcg.leaderboard;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;

public class LeaderboardSceneController : MonoBehaviour
{
	[SerializeField] GameObject leaderboardContent = null;
	[SerializeField] GameObject noData = null;
	[SerializeField] Text playerName = null;
	[SerializeField] Text playerRank = null;
	[SerializeField] Text playerScore = null;
	[SerializeField] GameObject entryPrefab = null;

    // Start is called before the first frame update
    void Start()
    {
		playerName.text = GameManager.instance.playerName;
		if (Server.instance.yourRank != -1)
		{
			playerRank.text = LocalizedString.GetString("yourRank").ToUpper() +" "+ Server.instance.yourRank;
		}
		else
		{
			playerRank.text = LocalizedString.GetString("yourRank").ToUpper();
		}
		playerScore.text = LocalizedString.GetString("highscore").ToUpper() + " : " + Server.instance.yourScore;

		if (Server.instance.leaderboardData.Count > 0)
		{
			noData.SetActive(false);

			int i = 1;
			foreach(LeaderboardPlayer p in Server.instance.leaderboardData)
			{
				GameObject entry = Instantiate(entryPrefab, leaderboardContent.transform);
				entry.GetComponent<LeaderboardEntry>().UpdateLeaderboardEntry(i,p);
				i++;
			}
		}
	}

	public void BackButton()
	{
		TransitionManager.instance.SwitchScene(0);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			BackButton();
		}
	}

	public void CommonButtonSound()
	{
		SoundManager.instance.PlayButtonSound();
	}
}
