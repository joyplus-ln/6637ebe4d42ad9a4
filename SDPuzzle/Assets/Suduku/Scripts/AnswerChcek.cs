using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerChcek
{

    public static bool CheckAnswerIsRight(List<cell> cells)
    {
        for (int i = 1; i < 9; i++)
        {
            bool right = false;
            bool righth = false;
            bool rightb = false;
            List<cell> lists = cells.FindAll(x => x.horizontal == i);
            if (lists != null)
            {
                right = CheckHasTwoSameNum(lists);
                //Debug.Log("H:" + lists.Count + right + i);
            }
            List<cell> listsv = cells.FindAll(x => x.vertical == i);
            if (listsv != null)
            {
                righth = CheckHasTwoSameNum(listsv);
                //Debug.Log("v:" + listsv.Count + right + i);
            }
            List<cell> listsb = cells.FindAll(x => x.box == i);
            if (listsb != null)
            {
                rightb = CheckHasTwoSameNum(listsb);
                //Debug.Log("b:" + listsb.Count + right + i);
            }
            if (!(right && righth && rightb))
            {
                return false;
            }

        }
        return true;


    }

    public static bool CheckCellIsRight(List<cell> cells, cell cel)
    {
        bool right = false;
        bool righth = false;
        bool rightb = false;
        List<cell> lists = cells.FindAll(x => x.horizontal == cel.horizontal && x.solution == cel.solution);
        if (lists.Count == 1)
        {
            right = true;
            //Debug.Log("H:" + lists.Count + right + i);
        }
        List<cell> listsv = cells.FindAll(x => x.vertical == cel.vertical && x.solution == cel.solution);
        if (listsv.Count == 1)
        {
            righth = true;
            //Debug.Log("v:" + listsv.Count + right + i);
        }
        List<cell> listsb = cells.FindAll(x => x.box == cel.box && x.solution == cel.solution);
        if (listsb.Count == 1)
        {
            rightb = true;
            //Debug.Log("b:" + listsb.Count + right + i);
        }
        if ((right && righth && rightb))
        {
            return true;
        }
        return false;
    }

    static bool CheckHasTwoSameNum(List<cell> celllist)
    {
        for (int i = 1; i < 9; i++)
        {
            List<cell> cellList = celllist.FindAll(x => x.solution == i);
            if (cellList.Count != 1)
            {
                Debug.Log(i + "shuliang" + cellList.Count);
                return false;
            }
        }
        return true;
    }

    public static string GetLevel(int level = 0)
    {
        TextAsset text = Resources.Load<TextAsset>("LevelsSD");
        string[] levels = text.text.Split(',');
        return levels[level];
    }
}
