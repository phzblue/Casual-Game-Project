using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipConfig
{
	public Type shipType = Type.Default;
	public string nameTerm = "term";
	public int objective = 0;
	public string iapID = "";

	public enum Type
	{
		Ads,
		Score,
		MatchPlayed,
		Gacha,
		Default		
	}

}
