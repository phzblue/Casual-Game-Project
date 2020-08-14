using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using I2.Loc;

public class RestorePrompt : MonoBehaviour
{
	[SerializeField] Text msg = null;
	[SerializeField] GameObject confirmButton = null;
	[SerializeField] GameObject buttonContainer = null;

	public void PendingRestore()
	{
		Debug.Log("================================= here? 2");

		confirmButton.SetActive(false);
		buttonContainer.SetActive(false);
        msg.text = LocalizedString.GetString("loading");
	}

	private void OnDisable()
	{
		confirmButton.SetActive(true);
        msg.text = LocalizedString.GetString("restorePurchaseDesc");
	}

	public void Restore()
	{
		if (Application.platform == RuntimePlatform.WSAPlayerX86 ||
					Application.platform == RuntimePlatform.WSAPlayerX64 ||
					Application.platform == RuntimePlatform.WSAPlayerARM)
		{
			CodelessIAPStoreListener.Instance.ExtensionProvider.GetExtension<IMicrosoftExtensions>()
				.RestoreTransactions();
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer ||
				 Application.platform == RuntimePlatform.OSXPlayer ||
				 Application.platform == RuntimePlatform.tvOS)
		{
			CodelessIAPStoreListener.Instance.ExtensionProvider.GetExtension<IAppleExtensions>()
				.RestoreTransactions(OnTransactionsRestored);
		}
		else if (Application.platform == RuntimePlatform.Android &&
				 StandardPurchasingModule.Instance().appStore == AppStore.SamsungApps)
		{
			CodelessIAPStoreListener.Instance.ExtensionProvider.GetExtension<ISamsungAppsExtensions>()
				.RestoreTransactions(OnTransactionsRestored);
		}
		else if (Application.platform == RuntimePlatform.Android &&
				 StandardPurchasingModule.Instance().appStore == AppStore.CloudMoolah)
		{
			CodelessIAPStoreListener.Instance.ExtensionProvider.GetExtension<IMoolahExtension>()
				.RestoreTransactionID((restoreTransactionIDState) =>
				{
					OnTransactionsRestored(
						restoreTransactionIDState != RestoreTransactionIDState.RestoreFailed &&
						restoreTransactionIDState != RestoreTransactionIDState.NotKnown);
				});
		}
		else
		{
			OnTransactionsRestored(false);
		}
	}

	void OnTransactionsRestored(bool success)
	{
		if (success)
		{
            msg.text = LocalizedString.GetString("restoreSuccess");
		}
		else
		{
            msg.text = LocalizedString.GetString("restoreFail");
		}

		buttonContainer.SetActive(true);

		Debug.Log("Transactions restored: " + success);
	}
}
