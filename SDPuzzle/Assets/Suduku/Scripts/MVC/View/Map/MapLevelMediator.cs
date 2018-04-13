using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.mediation.impl;

public class MapLevelMediator : EventMediator 
{
	[Inject]
	public MapLevelButton view{ get; set;}

	[Inject]
	public ILevel model{ get; set;}

	public override void OnRegister()
	{
		view.dispatcher.AddListener (MapLevelButton.CLICK_EVENT, onViewClicked);
		view.Init (model.unlocklevel);
	}

	public override void OnRemove()
	{
		view.dispatcher.RemoveListener (MapLevelButton.CLICK_EVENT, onViewClicked);
	}

	private void onViewClicked()
	{
		Debug.Log("View click detected");
		dispatcher.Dispatch(GameEvent.LOAD_SCENE, WorldLevel.Play.ToString());
	}



}
