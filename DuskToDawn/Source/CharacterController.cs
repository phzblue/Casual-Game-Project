using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;

public class CharacterController : MonoBehaviour
{
	public GameObject skinsContainer;

	public Toggle duskToggle;
	public Toggle dawnToggle;

	public GameObject duskLightObj;
	public GameObject dawnLightObj;
	public GameObject dawnRingObj;
	public GameObject duskRingObj;

	public GameObject duskAvatar;
	public GameObject dawnAvatar;

	public Text avatarName;
	public Text avatarDesc;

	private void Start()
	{
		if(GameManager.instance.playerData.leftRunnerID == 0)
		{
			duskAvatar.SetActive(false);
			duskAvatar.transform.parent.GetChild(0).gameObject.SetActive(true);
		}
		else
		{
			duskAvatar.GetComponent<Image>().sprite =
				skinsContainer.transform.GetChild(GameManager.instance.playerData.leftRunnerID)
				.GetComponent<Image>().sprite;
		}

		if (GameManager.instance.playerData.rightRunnerID == 0)
		{
			dawnAvatar.SetActive(false);
			dawnAvatar.transform.parent.GetChild(0).gameObject.SetActive(true);
		}
		else
		{
			dawnAvatar.GetComponent<Image>().sprite =
				skinsContainer.transform.GetChild(GameManager.instance.playerData.rightRunnerID)
				.GetComponent<Image>().sprite;

		}
	}

	public void UpdateDesc(int index)
	{
		if (GameConfig.skinList[index] != null)
		{
			if(GameConfig.skinList[index].skinName != null)
			{
                avatarName.text = LocalizedString.GetString(GameConfig.skinList[index].skinName);
				avatarDesc.text = LocalizedString.GetString(GameConfig.skinList[index].skinDesc);
			}
			else
            {
                avatarName.text = LocalizedString.GetString("skinNameID" + index);
				avatarDesc.text = LocalizedString.GetString("skinDescID" + index);
			}
		}
	}

	public void Toggle()
	{
		duskLightObj.SetActive(duskToggle.isOn);
		dawnLightObj.SetActive(!duskToggle.isOn);
		duskRingObj.SetActive(duskToggle.isOn);
		dawnRingObj.SetActive(!duskToggle.isOn);

		if (!duskToggle.isOn)
		{
			foreach (RectTransform o in GameObject.FindObjectOfType<Carousel>().introImages)
			{
				if (GameObject.FindObjectOfType<Carousel>().introImages[0] != o)
					o.rotation = new Quaternion(0, 180, 0, 1);
			}
		}
		else
		{
			foreach (RectTransform o in GameObject.FindObjectOfType<Carousel>().introImages)
			{
				if (GameObject.FindObjectOfType<Carousel>().introImages[0] != o)
					o.localRotation = Quaternion.identity;
			}
		}

	}

	public void UpdateCharacter()
	{
		int charID = GameObject.FindObjectOfType<Carousel>().currentIndex;
	
		if (duskToggle.isOn)
		{
			if(charID == 0)
			{
				duskAvatar.SetActive(false);
				duskAvatar.transform.parent.GetChild(0).gameObject.SetActive(true);
			}
			else
			{
				duskAvatar.SetActive(true);
				duskAvatar.transform.parent.GetChild(0).gameObject.SetActive(false);

				duskAvatar.GetComponent<Image>().sprite = GameObject.FindObjectOfType<Carousel>().introImages[charID].GetComponent<Image>().sprite;
			}

			GameManager.instance.playerData.leftRunnerID = charID;
		}
		else
		{
			if (charID == 0)
			{
				dawnAvatar.SetActive(false);
				dawnAvatar.transform.parent.GetChild(0).gameObject.SetActive(true);
			}
			else
			{
				dawnAvatar.SetActive(true);
				dawnAvatar.transform.parent.GetChild(0).gameObject.SetActive(false);

				dawnAvatar.GetComponent<Image>().sprite = GameObject.FindObjectOfType<Carousel>().introImages[charID].GetComponent<Image>().sprite;
			}

			GameManager.instance.playerData.rightRunnerID = charID;
		}
		GameManager.instance.playerData.SaveData();
	}
}
