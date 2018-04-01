using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public string info = "";
    // Use this for initialization
    void Start()
    {

    }

    public void Init(cell cell)
    {
        cell.label = transform.GetChild(0).GetComponent<Text>();
        info = String.Format("{0}-{1},{2}-{3},{4}-{5},{6}-{7}", "H:", cell.horizontal, "V:", cell.vertical, "B:", cell.box, "I", cell.index);
    }
}
