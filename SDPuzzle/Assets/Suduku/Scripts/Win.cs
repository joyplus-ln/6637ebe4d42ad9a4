using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using strange.extensions.mediation.impl;

public class Win : EventView
{
	[Inject]
	public ILevel model{ get; set;}
    // Use this for initialization
    void Start()
    {
        transform.DOScale(Vector3.one, 0.5f);
    }

    public void ClickNest()
    {
		if (model.currentlevel == model.unlocklevel)
        {
			model.UnlockLevel ();
        }
        Utils.GoToMapScene();
    }
}
