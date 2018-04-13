using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.mediation.impl;
using strange.extensions.dispatcher.eventdispatcher.api;

public class MapLevelButton : EventView {
	public TextMesh levelnumText;
	public Animation animaton;
	public int levelIndex;

	public const string CLICK_EVENT = "CLICK_EVENT";
	[Inject]
	public IEventDispatcher dispatcher{get;set;}

	public void Init(int unlockLevel)
	{
		levelnumText.text = levelIndex.ToString ();

		if (levelIndex > unlockLevel) 
		{
			animaton.enabled = false;
		}
	}


	public void OnClick()
	{
		dispatcher.Dispatch(CLICK_EVENT);
	}
}
