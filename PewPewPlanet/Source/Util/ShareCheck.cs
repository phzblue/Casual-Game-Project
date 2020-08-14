using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;

public class ShareCheck : MonoBehaviour
{
	[SerializeField] GameObject freeObj = null;
	[SerializeField] Text freeText = null;

    // Start is called before the first frame update
    void Start()
    {
		freeObj.SetActive(!GameManager.instance.playerData.hasShare);
		freeText.text = LocalizedString.GetString("free").ToUpper() + " 20";
	}

}
