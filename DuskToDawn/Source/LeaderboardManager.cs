using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour
{
	[SerializeField] GameObject setNameUI = null;
	[SerializeField] Button confirmButton = null;

	public Text playerNameText = null;
	public Text playerHighScoreText = null;
	public Text playerRank = null;
	public Text playerNameFailReason = null;
	public ObjectPooler leaderboardEntryPool = null;
	public List<GameObject> leaderboardEntries = new List<GameObject>();
	public GameObject empty = null;

	string playerName = "";

    // Start is called before the first frame update
    void Start()
    {
		BackButtonManager.instance.SetCurrentScreen("leaderboard");
		LeaderboardController.instance.CallGetLeaderboard();
	}

	public void RequestName()
	{
		setNameUI.SetActive(true);
	}

	public void OnInputChanged(string name)
	{
		playerName = name;
	}

	public void OnEndEdit(string value)
	{
		confirmButton.interactable = !value.Equals("");
	}

	public void CreatePlayer()
	{
		LeaderboardController.instance.CreatePlayer(playerName);
	}

	public void CloseSetNameUI()
	{
		setNameUI.SetActive(false);
	}

	public void ShowInterstitialAds()
	{
		GameManager.instance.ShowInterstitialAds();
	}
}
