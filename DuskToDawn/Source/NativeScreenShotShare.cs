using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class NativeScreenShotShare : MonoBehaviour
{
	public GameObject freeBubble;
	public Button shareButton;
	public string type;

	private string shareSubject, shareMessage = "";
	private string screenshotName = "";

	void Start()
	{
#if UNITY_WEBGL
		shareButton.gameObject.SetActive(false);
#endif

		freeBubble.SetActive(!GameManager.instance.playerData.hasShare);
	}

	public void SetMessageAndShare()
	{
		if (freeBubble.activeInHierarchy)
		{
			GameManager.instance.playerData.playerCoin += 30;
			GameManager.instance.playerData.hasShare = true;
			GameManager.instance.playerData.SaveData();
			freeBubble.SetActive(false);
		}

		switch (type)
		{
			case "gacha":
				screenshotName = "gacha.png";
				shareSubject = "Check out my new character!";
				shareMessage = "Join me now at Dusk to Dawn\nhttps://go.onelink.me/ENcC/44cb3e1c";
				GameObject.FindObjectOfType<GachaSceneManager>().RefreshCoinDisplay();

				break;
			case "score":
				screenshotName = "score.png";
				shareSubject = "Check out my score!";
				shareMessage = "Compete with me at Dusk to Dawn\nhttps://go.onelink.me/ENcC/44cb3e1c";
				GameObject.FindObjectOfType<GameSceneManager>().RefreshCoinDisplay();
				break;
			default:
				screenshotName = "share.png";
				shareSubject = "Play with me now!";
				shareMessage = "Join me now at Dusk to Dawn\nhttps://go.onelink.me/ENcC/44cb3e1c";
				break;
		}

		StartCoroutine(TakeSSAndShare());
	}

	private IEnumerator TakeSSAndShare()
	{
		yield return new WaitForEndOfFrame();

		Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		ss.Apply();

		string filePath = Path.Combine(Application.temporaryCachePath, screenshotName);
		File.WriteAllBytes(filePath, ss.EncodeToPNG());

		// To avoid memory leaks
		Destroy(ss);

		new NativeShare().AddFile(filePath).SetSubject(shareSubject).SetText(shareMessage).Share();

		GameManager.instance.AFTrackRichEvent("af_share");

		// Share on WhatsApp only, if installed (Android only)
		//if( NativeShare.TargetExists( "com.whatsapp" ) )
		//	new NativeShare().AddFile( filePath ).SetText( "Hello world!" ).SetTarget( "com.whatsapp" ).Share();
	}
}