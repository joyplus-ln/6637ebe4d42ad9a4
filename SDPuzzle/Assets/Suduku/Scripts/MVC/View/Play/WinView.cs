using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using strange.extensions.mediation.impl;
using strange.extensions.dispatcher.eventdispatcher.api;

public class WinView : EventView
{
	public const string CLICK_EVENT = "Complete";
	[Inject]
	public ILevel model{ get; set;}

	[Inject]
	public IEventDispatcher dispatcher{get;set;}

    // Use this for initialization
    public void Init()
    {
        transform.DOScale(Vector3.one, 0.5f);
    }

    public void ClickNest()
    {
		if (model.currentlevel == model.unlocklevel)
        {
			model.currentlevel++;
			model.UnlockLevel ();
        }
		dispatcher.Dispatch(CLICK_EVENT);
    }
}
