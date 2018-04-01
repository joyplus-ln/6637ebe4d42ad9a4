using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using strange.extensions.mediation.impl;

public class MapLocation : View {
	public Transform nextLocationConnector;
	public Transform previousLocationConnector;
	[HideInInspector]
	public int number = 0;
	[HideInInspector]
	public MapLocation nextLocation = null;
	[HideInInspector]
	public MapLocation previousLocation = null;

	MapWorld map;
	Rect mapRect;

	public Sprite background;

	void Start() {


		CreateButtons();
	}

	void CreateButtons() {
		


	}



	public int GetLevelCount() {
		return transform.Find("Buttons").childCount;    
	}

	public void ApplyBackground() {
	}


}
