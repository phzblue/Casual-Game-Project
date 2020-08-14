using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class SkinSceneManager : MonoBehaviour
{
	public GameObject cartoonWindow;
	public GameObject skinContainer;
	public GameObject useButton;
	public GameObject skinRequirements;
	public GameObject iapGroup_ios;
	public GameObject iapGroup_adr;
	public Text skinNum;

	GameObject currentIAPActiveButton;
	Dictionary<int, Transform> characterIndex = new Dictionary<int, Transform>();
	Dictionary<string, GameObject> iapButtonID = new Dictionary<string, GameObject>();
	bool purchaseProcess = false;
	public bool noInternet = false;
	int skinID = -1;

	private void Awake()
	{
#if UNITY_IOS
		foreach (Transform iapButton in iapGroup_ios.transform)
		{
			iapButtonID.Add(iapButton.name, iapButton.gameObject);
		}
//#elif UNITY_ANDRIOD
#else
		foreach (Transform iapButton in iapGroup_adr.transform)
		{
			iapButtonID.Add(iapButton.name, iapButton.gameObject);
		}
#endif
	}

	private void Start()
	{
		BackButtonManager.instance.SetCurrentScreen("skin");
		BackButtonManager.instance.RefreshObject();
		UpdateOwnSkinNum();
	}

	private void FixedUpdate()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			noInternet = true;
		}
		else
		{
			noInternet = false;
		}
	}

	public void DisableUsageButton(bool disable, int index)
	{
		skinRequirements.SetActive(false);
		if(currentIAPActiveButton != null)
			currentIAPActiveButton.SetActive(false);

		useButton.SetActive(!disable);

		if(disable && GameConfig.skinList[index].skinCategory != SkinDataManager.SkinCategory.DEFAULT)
		{
			switch (GameConfig.skinList[index].skinCategory)
			{
				case SkinDataManager.SkinCategory.MATCH:
				case SkinDataManager.SkinCategory.ADS:
				case SkinDataManager.SkinCategory.SCORE:
					skinRequirements.SetActive(true);
					skinRequirements.GetComponent<Text>().text = GameConfig.skinList[index].GetConditionString();
					break;
				case SkinDataManager.SkinCategory.PURCHASE:
					iapButtonID[index.ToString()].SetActive(true);
					currentIAPActiveButton = iapButtonID[index.ToString()];
					break;
				case SkinDataManager.SkinCategory.DEFAULT:
					break;
			}
		}
	}

	public void UpdateOwnSkinNum()
	{
		skinNum.text = "x" + (GameManager.instance.playerData.skinIDContain.Count-1).ToString();
		GameObject.FindObjectOfType<CharacterController>().UpdateDesc(GameObject.FindObjectOfType<Carousel>().currentIndex);
	}

	public void AddSkinIDToPlayer(int id)
	{
		if (!GameManager.instance.playerData.skinIDContain.Contains(id))
		{
			GameManager.instance.playerData.skinIDContain.Add(id);
			GameManager.instance.AFTrackRichEvent("charUnlock");
			GameManager.instance.playerData.SaveData();

			UpdateOwnSkinNum();
		}
	}

	public void EnterPurchaseProcess(int skinID)
	{
		if (noInternet)
		{
			BackButtonManager.instance.ShowNoInternetPrompt();
		}
		else
		{
			purchaseProcess = true;
			this.skinID = skinID;
		}
	}

	public void OnPurchaseComplete(Product product)
	{
		if (purchaseProcess)
		{
			GameManager.instance.ProcessPurchase(product, skinID);
			purchaseProcess = false;
		}
	}

	public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
	{
		if (purchaseProcess)
		{
			purchaseProcess = false;
			switch (reason)
			{
				case PurchaseFailureReason.PurchasingUnavailable:
					Debug.Log("Purchase Fail Reason : ======================================= PurchasingUnavailable");
					break;
				case PurchaseFailureReason.ExistingPurchasePending:
					Debug.Log("Purchase Fail Reason : ======================================= ExistingPurchasePending");
					break;
				case PurchaseFailureReason.ProductUnavailable:
					Debug.Log("Purchase Fail Reason : ======================================= ProductUnavailable");
					break;
				case PurchaseFailureReason.SignatureInvalid:
					Debug.Log("Purchase Fail Reason : ======================================= SignatureInvalid");
					break;
				case PurchaseFailureReason.UserCancelled:
					Debug.Log("Purchase Fail Reason : ======================================= UserCancelled");
					break;
				case PurchaseFailureReason.PaymentDeclined:
					Debug.Log("Purchase Fail Reason : ======================================= PaymentDeclined");
					break;
				case PurchaseFailureReason.DuplicateTransaction:
					Debug.Log("Purchase Fail Reason : ======================================= DuplicateTransaction");
					break;
				case PurchaseFailureReason.Unknown:
					Debug.Log("Purchase Fail Reason : ======================================= Unknown");
					break;
			}
		}
	}
}
