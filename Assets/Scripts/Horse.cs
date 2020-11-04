using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horse : Chessman
{
    public override bool[,] PossibleMove()
    {
        bool[,] r = new bool[8, 8];
        //Up Left
        HorseMove(CurrentX - 1, CurrentY + 2, ref r);
        //Up Right
        HorseMove(CurrentX + 1, CurrentY + 2, ref r);
        //Left Up
        HorseMove(CurrentX - 2, CurrentY + 1, ref r);
        //Right Up
        HorseMove(CurrentX + 2, CurrentY + 1, ref r);
        //Left Down
        HorseMove(CurrentX - 2, CurrentY - 1, ref r);
        //Right Down
        HorseMove(CurrentX + 2, CurrentY - 1, ref r);
        //Down Left
        HorseMove(CurrentX - 1, CurrentY - 2, ref r);
        //Right Down
        HorseMove(CurrentX + 1, CurrentY - 2, ref r);


        return r;
    }
    public void HorseMove(int x, int y, ref bool[,] r)
    {
        Chessman c;
        if(x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            c = BoardManager.Instance.Chessmans[x, y];
            if (c == null)
                r[x, y] = true;
            else if (isWhite != c.isWhite)
                r[x, y] = true;
          
        }
    }
}
