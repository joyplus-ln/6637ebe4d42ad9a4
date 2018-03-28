using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System;
using System.Collections.Generic;
using DG.Tweening;
using strange.extensions.mediation.impl;

public class MapLocation : View
{
	public Transform nextLocationConnector;
	public Transform previousLocationConnector;
	public List<MapLocation> NextMapLocations = new List<MapLocation>();
	public GameObject mapLevel;
	public Transform locators_folder;
	public SpriteRenderer sprite;
	public TextMesh text;

	public int number = 0;
	MapLocation nextLocation = null;
	MapLocation previousLocation = null;


	private Texture2D subWordBackGround;

	private bool isAbove;
	private GameObject _worldeffect;


	void Start()
	{

	}

	void CreateButtons()
	{
		

	}



	public void OnPositionChanged()
	{
//		if (MapWorld.main == null) return;
//		int p = MapWorld.main.IsVisible(previousLocationConnector.position);
//		int n = MapWorld.main.IsVisible(nextLocationConnector.position);
//		bool aboveMiddle = MapWorld.main.IsAboveMiddle(nextLocationConnector.position);
//
//		if (n != 0 && p != 0 && p == n)
//		{
//			MapWorld.main.RemoveLocation(this);
//			Destroy(gameObject);
//			return;
//		}
//
//		if (nextLocation == null)
//		{
//			if (n == 0)
//			{
//				nextLocation = MapWorld.main.ShowNextLocation(this);
//			}
//		}
//
//		if (previousLocation == null)
//		{
//			if (p == 0)
//			{
//				previousLocation = MapWorld.main.ShowPreviuosLocation(this);
//			}
//		}
//
//		if (isAbove != aboveMiddle)
//		{
//			MapWorld.main.ChangeBackground(aboveMiddle ? this : nextLocation);
//			isAbove = aboveMiddle;
//		}
	}



}
