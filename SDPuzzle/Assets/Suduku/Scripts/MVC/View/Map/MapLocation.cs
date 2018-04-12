using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using strange.extensions.mediation.impl;

public class MapLocation : View {
	public Transform nextLocationConnector;
	public Transform previousLocationConnector;
	public Transform levelButtonParent;
	public GameObject levelButton;
	[HideInInspector]
	public int number = 0;
	[HideInInspector]
	public MapLocation nextLocation = null;
	[HideInInspector]
	public MapLocation previousLocation = null;

	MapWorld map;
	Rect mapRect;

	public Sprite background;

	void Start() 
	{
		CreateButtons();
	}

	void CreateButtons() 
	{
		for (int i = 0; i < levelButtonParent.childCount; i++) 
		{
			GameObject button = Instantiate (levelButton);
			button.transform.SetParent(levelButtonParent.GetChild (i));
			button.transform.localPosition=Vector3.zero;
			button.transform.localRotation=Quaternion.identity;
			MapLevelButton script = button.GetComponent<MapLevelButton> ();
			script.levelnumText.text = (number * levelButtonParent.childCount + i + 1).ToString();
		}
	}

	public int GetLevelCount()
	{
		return levelButtonParent.childCount;
	}
}
