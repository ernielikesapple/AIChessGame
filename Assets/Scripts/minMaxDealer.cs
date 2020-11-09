using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class minMaxDealer 
{

    List<GameObject> activeChessmanVar;

    public void minMaxCoreAlgorithm(List<GameObject> activeChessman) {
        activeChessmanVar = activeChessman;
        getBoardState();




        
    }






    public void getBoardState()
    {
        foreach (GameObject activeChessPiece in activeChessmanVar)
        {
            Chessman cm = activeChessPiece.GetComponent<Chessman>();
            if (cm.isWhite == false) //黑子
            {
                if (cm.GetType().ToString() == "King")
                { 
                    int x1 = cm.CurrentX;
                    int y1 = cm.CurrentY;

                    // calculate the score for black part
                    

                }
                else if (cm.GetType().ToString() == "Queen") {
                    int x1 = cm.CurrentX;
                    int y1 = cm.CurrentY;
                    

                }
                else if (cm.GetType().ToString() == "Rook")
                {
                    int x1 = cm.CurrentX;
                    int y1 = cm.CurrentY;
                    

                }
                else if (cm.GetType().ToString() == "Bishop")
                {
                    int x1 = cm.CurrentX;
                    int y1 = cm.CurrentY;
                    

                }
                else if (cm.GetType().ToString() == "Horse")
                {
                    int x1 = cm.CurrentX;
                    int y1 = cm.CurrentY;
                    

                }
                else if (cm.GetType().ToString() == "Pawn")
                {
                    int x1 = cm.CurrentX;
                    int y1 = cm.CurrentY;
                    

                }

            }
            else // 对应的种类的白子
            {
                if (cm.GetType().ToString() == "King")
                { 
                    int x1 = cm.CurrentX;
                    int y1 = cm.CurrentY;

                    
                    // calculate the score for black part
                    

                }
                else if (cm.GetType().ToString() == "Queen")
                {
                    int x1 = cm.CurrentX;
                    int y1 = cm.CurrentY;
                    
                }
                else if (cm.GetType().ToString() == "Rook")
                {
                    int x1 = cm.CurrentX;
                    int y1 = cm.CurrentY;
                    

                }
                else if (cm.GetType().ToString() == "Bishop")
                {
                    int x1 = cm.CurrentX;
                    int y1 = cm.CurrentY;
                    

                }
                else if (cm.GetType().ToString() == "Horse")
                {
                    int x1 = cm.CurrentX;
                    int y1 = cm.CurrentY;
                    

                }
                else if (cm.GetType().ToString() == "Pawn")
                {
                    int x1 = cm.CurrentX;
                    int y1 = cm.CurrentY;
                    

                }
            }


        }


    }



}
