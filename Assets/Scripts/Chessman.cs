using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Chessman : MonoBehaviour
{
    public int CurrentX { set; get; }
    public int CurrentY { set; get; }
    public bool isWhite { set; get; }    //Know which team this piece is part of

    public void SetPosition(int x, int y)
    {
        CurrentX = x;
        CurrentY = y;
    }

    public virtual bool[,] PossibleMove()  // virtual keyword means, this function will be override by the child class to let the child class add more customized functions
    {
        return new bool[8,8];
    }
}
