using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.mediation.impl;
using strange.extensions.dispatcher.eventdispatcher.api;

public class MapLevelButton : EventView {
	public TextMesh levelnumText;

	public const string CLICK_EVENT = "CLICK_EVENT";
	[Inject]
	public IEventDispatcher dispatcher{get;set;}


	public void OnClick()
	{
		dispatcher.Dispatch(CLICK_EVENT);
	}
}
