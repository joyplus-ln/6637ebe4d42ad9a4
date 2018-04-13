using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Win : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        transform.DOScale(Vector3.one, 0.5f);
    }

    public void ClickNest()
    {
        if (Utils.GetCurrentLevel() == Utils.GetMaxUnlockLevel())
        {
            Utils.SetMaxUnlockLevel();
        }
        Utils.GoToMapScene();
    }
}
