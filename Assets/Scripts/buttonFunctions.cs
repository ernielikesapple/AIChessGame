using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class buttonFunctions : MonoBehaviour
{
    public Dropdown dropdown;

    public void changeToRandomPlayer()
    {
        BoardManager.Instance.smartOpponent = false;
    }


    public void changeToMinmaxPlayer()
    {
        BoardManager.Instance.smartOpponent = true;
    }

    public void changeDepth(int index)
    {
        BoardManager.Instance.maxDepth = index;
    }

}
