using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.mediation.impl;

public class WinMediator : EventMediator 
{
	[Inject]
	public WinView view{ get; set;}

	[Inject]
	public ILevel model{ get; set;}

	public override void OnRegister()
	{
		view.dispatcher.AddListener (WinView.CLICK_EVENT, onViewClicked);
		view.Init ();
	}

	public override void OnRemove()
	{
		view.dispatcher.RemoveListener (WinView.CLICK_EVENT, onViewClicked);
	}

	private void onViewClicked()
	{
		Debug.Log("Win view click detected");
		dispatcher.Dispatch(GameEvent.LOAD_SCENE, WorldLevel.Play.ToString());
	}
}
