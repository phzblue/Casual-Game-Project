using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;

public class LanguageSelector : MonoBehaviour
{
	public void ChangeLanguage(string lan)
	{
		LocalizationManager.CurrentLanguageCode = lan;
	}
}
