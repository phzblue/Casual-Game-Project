using com.igi.game.common.webserver.hcg;
using com.igi.game.common.webserver.hcg.leaderboard;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;

// Field dimensions : 988 x 1420 (world coord ±4.94 x ±7.1)
public class LeaderboardController : MonoBehaviour, HCGLeaderboard.Listener {
	public static LeaderboardController instance;
	private string hcgLeaderboardURL = @"http://live.hcg.gogame.net:9000/hcgleaderboard";
	//http://live.hcg.gogame.net:9000/hcgleaderboard live: 34.87.13.241
	//http://34.87.56.195:9000/hcgleaderboard staging

	private string accountID;
	private string playerName = "";

#if UNITY_WEBGL
	private string game = "DuskDawn_H5";
#else
	private string game = "DuskDawn";
#endif
	private HCGLeaderboard hcgLeaderboard;
	
    public long playerHighScoreDaily = 0;
    public long playerHighScore = 0;

	public Text playerNameFailReason;
	public DateTime utcDate = DateTime.UtcNow.AddHours(8);

	// start
	void Awake() {
		if (instance == null) instance = this;
		else if (instance != this) Destroy(gameObject);
	}

	void Start() {
		if (playerHighScoreDaily == 0)
        {
			playerHighScoreDaily = PlayerPrefs.GetInt("HighScore_"+ utcDate.Date.Day);
			playerHighScore = PlayerPrefs.GetInt("HighScore_Global");

		}

		hcgLeaderboard = new HCGLeaderboard(hcgLeaderboardURL);
		hcgLeaderboard.AddListener(this);

#if UNITY_WEBGL
        accountID = WebManager.instance.GetURLParam("playerID");

		Debug.Log("Call GetPlayer");
		StartCoroutine(hcgLeaderboard.HttpCall(new GetPlayer(accountID)));
#else
		accountID = SystemInfo.deviceUniqueIdentifier;
#endif
	}

    public void OnDestroy() {
		if (hcgLeaderboard != null) {
			hcgLeaderboard.RemoveAllListener();
			hcgLeaderboard = null;
		}
	}

    public void UpdateHighScore(long currentScore)
    {
        if (currentScore > playerHighScoreDaily)
        {
			if(currentScore > playerHighScore)
			{
				playerHighScore = currentScore;
			}
			playerHighScoreDaily = currentScore;
            PlayerPrefs.SetInt("HighScore_" + utcDate.Date.Day, (int)Mathf.Round(playerHighScoreDaily));
            PlayerPrefs.SetInt("HighScore_Global", (int)Mathf.Round(playerHighScore));

		}

		StartCoroutine(hcgLeaderboard.HttpCall(new UpdateHighScore(accountID, game, currentScore)));
	}

    public void CreatePlayer(string playerName)
    {
        Debug.Log("Call CreatePlayer");
        StartCoroutine(hcgLeaderboard.HttpCall(new CreatePlayer(accountID, playerName)));
    }

	public void CallGetPlayer()
	{
		StartCoroutine(hcgLeaderboard.HttpCall(new GetPlayer(accountID)));
	}

    public void CallGetLeaderboard()
    {
		if (playerName.Equals(""))
		{
            Debug.Log("call get player =============================");
			CallGetPlayer();
		}
		else
		{
            Debug.Log("call get leaderboard =============================");

            Debug.Log("Call GetLeaderboard");
			StartCoroutine(hcgLeaderboard.HttpCall(new GetLeaderboard(accountID, game)));
		}
	}

    public string GetAccountID()
    {
        return accountID;
    }

    public void OnError(Type type) {
		Debug.Log(type.Name + " Http error");
	}

	public void OnResult(Type type, Message message) {
		if (message is PlayerResult) {

			PlayerResult playerResult = (PlayerResult)message;
			if (playerResult.success) {
				//Debug.Log("player result success");
				//Debug.Log("PlayerResult : " + type.Name + " success, playerName : " + playerResult.playerName);
				playerName = playerResult.playerName;

				GameObject.FindObjectOfType<LeaderboardManager>().CloseSetNameUI();

#if !UNITY_WEBGL
				if(playerHighScoreDaily > 0)
				{
					UpdateHighScore(playerHighScoreDaily);
				}
				else if(type == typeof(GetPlayer))
				{
					CallGetLeaderboard();
				}
#endif
			}
			else {

				Debug.Log("PlayerResult : " + type.Name + " failed");
				if (type == typeof(GetPlayer)) {
					Debug.Log("No player found. Request name");
					GameObject.FindObjectOfType<LeaderboardManager>().RequestName();
				}
				else
				{
					Debug.Log(playerResult.reason + " " + type);

                    GameObject.FindObjectOfType<LeaderboardManager>().playerNameFailReason.text =
                        LocalizedString.GetString(playerResult.reason);
					GameObject.FindObjectOfType<LeaderboardManager>().playerNameFailReason.gameObject.SetActive(true);
				}
			}
		} else if (message is LeaderboardResult) {

			LeaderboardResult leaderboardResult = (LeaderboardResult)message;

			string leaderboardString = "LeaderboardResult : " + type.Name + ", rank:" + leaderboardResult.playerRank + ", score:" + leaderboardResult.playerScore + ", bestscore:" + leaderboardResult.playerBestScore;
			int rank = 1;
			foreach (LeaderboardPlayer player in leaderboardResult.leaderboard) {
				leaderboardString += "\n " + rank++ + ". " + player.playerName + " : " + player.playerScore;
			}
			Debug.Log(leaderboardString);

			if (type == typeof(GetLeaderboard) || type == typeof(UpdateHighScore)) {
                List<LeaderboardPlayer> lpList = leaderboardResult.leaderboard;
				
				if (playerHighScoreDaily < leaderboardResult.playerScore)
				{
					playerHighScoreDaily = leaderboardResult.playerScore;
				}

				if (playerHighScore < leaderboardResult.playerBestScore)
				{
					playerHighScore = leaderboardResult.playerBestScore;
				}
				
				GameObject.FindObjectOfType<LeaderboardManager>().playerNameText.text = playerName;
				GameObject.FindObjectOfType<LeaderboardManager>().playerHighScoreText.text = LocalizedString.GetString("today") + " " + playerHighScoreDaily.ToString();

				if (leaderboardResult.playerRank == -1)
				{
					GameObject.FindObjectOfType<LeaderboardManager>().playerRank.text
						= LocalizedString.GetString("yourRank") + " #";
				}
				else
				{
					GameObject.FindObjectOfType<LeaderboardManager>().playerRank.text
						= LocalizedString.GetString("yourRank") + " #" +
						leaderboardResult.playerRank.ToString();
				}

				GameObject.FindObjectOfType<LeaderboardManager>().empty.SetActive(lpList.Count < 1);

				for (int i = 0; i < lpList.Count; i++)
				{

					GameObject entry = GameObject.FindObjectOfType<LeaderboardManager>().leaderboardEntryPool.AddToObjectPool();
					GameObject.FindObjectOfType<LeaderboardManager>().leaderboardEntries.Clear();
					GameObject.FindObjectOfType<LeaderboardManager>().leaderboardEntries.Add(entry);

					entry.SetActive(true);
					entry.GetComponent<LeaderboardEntry>().UpdateLeaderboardEntry(i + 1, lpList[i]);
				}
			}
		} else if (message is StringMessage) {
			string stringMessage = ((StringMessage)message).message;
			Debug.Log("StringMessage : " + stringMessage);
		} else {
			Debug.Log("Unknown result type : " + message._class);
		}
	}
}
