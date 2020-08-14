using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersionChecker : MonoBehaviour
{
	public Text versionLabel = null;
    // Start is called before the first frame update
    void Start()
    {
		versionLabel.text = "v" + Application.version + " © 2019 goGame";
    }
}
