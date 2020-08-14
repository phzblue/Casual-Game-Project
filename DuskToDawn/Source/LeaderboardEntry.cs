using UnityEngine;
using UnityEngine.UI;
using com.igi.game.common.webserver.hcg.leaderboard;

public class LeaderboardEntry : MonoBehaviour
{
    [SerializeField]
    public Text rankNumberText = null;

    [SerializeField]
    Text playerNameText = null;

    [SerializeField]
    Text scoreText = null;
	
    [SerializeField]
    Image crownIcon = null;

    [SerializeField]
    Sprite[] crownSprites = null;


    public void UpdateLeaderboardEntry(int rankNumber, LeaderboardPlayer leaderboardPlayer)
    {
        if (rankNumber <= 3)
        {
            crownIcon.gameObject.SetActive(true);
            crownIcon.sprite = crownSprites[rankNumber - 1];
        }
        else
        {
            crownIcon.gameObject.SetActive(false);
        }

        rankNumberText.text = rankNumber.ToString();
        scoreText.text = leaderboardPlayer.playerScore.ToString();
        playerNameText.text = leaderboardPlayer.playerName;
    }
}
