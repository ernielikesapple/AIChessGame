using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fakeBoardManager 
{
    public static fakeBoardManager Instance { set; get; }

    public Chessman[,] Chessmans { set; get; }  //Chessman array and a property
    public Chessman selectedChessman;
    public bool[,] allowedMoves { set; get; }



    public void MoveChessEssenceLogic(int x, int y)
    {

        if (allowedMoves[x, y])
        {
            Chessman c = Chessmans[x, y]; // 落子点
            //
            if (c != null )
            {
                Chessmans[x, y] = null;
            }

            ////EnPassantMove(The first nove of the black Pawn is two square, then the white Pawn can remove it)
            //if (x == EnPassantMove[0] && y == EnPassantMove[1] && smartOpponentDoingTrials == false && smartOpponent == false)  //出错点！！
            //{
            //    //White turn(black Pawn move 2 squares)
            //    if (isWhiteTurn)
            //    {
            //        c = Chessmans[x, y - 1];
            //        activeChessman.Remove(c.gameObject);
            //        Destroy(c.gameObject);
            //    }
            //    //Black turn(white Pawn move 2 squares)
            //    else
            //    {
            //        c = Chessmans[x, y + 1];
            //        activeChessman.Remove(c.gameObject);
            //        Destroy(c.gameObject);
            //    }
            //}
            //EnPassantMove[0] = -1;
            //EnPassantMove[1] = -1;


            if (selectedChessman.GetType() == typeof(Pawn))
            {
                if (y == 7)  // whit pawn become a queen
                {
                    Chessmans[x, y] = null;
                    
                    SpawnChessman(1, x, y);
                    selectedChessman = Chessmans[x, y];
                }
                else if (y == 0) // black pawn become a queen
                {
                    Chessmans[x, y] = null;
                    SpawnChessman(7, x, y);
                    selectedChessman = Chessmans[x, y];
                }

                ////White Pawn
                //if (selectedChessman.CurrentY == 1 && y == 3 && smartOpponentDoingTrials == false && smartOpponent == false)
                //{
                //    //Possible move of the black Pawn 
                //    EnPassantMove[0] = x;
                //    EnPassantMove[1] = y - 1;
                //}
                ////Black Pawn
                //else if (selectedChessman.CurrentY == 6 && y == 4 && smartOpponentDoingTrials == false && smartOpponent == false)
                //{
                //    //Possible move of the white Pawn
                //    EnPassantMove[0] = x;
                //    EnPassantMove[1] = y + 1;
                //}

            }


       
            Chessmans[selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
            selectedChessman.SetPosition(x, y);
            Chessmans[x, y] = selectedChessman;  // 更新当前棋子要移向点的棋子

            Debug.Log("fake board manager 这边 棋盘上的棋子在" + "坐标x：" + x + "坐标y：" + y + "更新了棋子名字为：" + Chessmans[x, y].GetType().ToString());
            
           
            
            

        }

        selectedChessman = null;//Select next Chessman
    }


    public void MoveChessBackEssenceLogic(int x, int y)
    {

        if (selectedChessman.GetType() == typeof(Pawn))
        {
            //if (y == 7)  // whit pawn become a queen   // todo: add logic to deal with the situation let the queen back to pawn
            //{
            //    Chessmans[x, y] = null;

            //    SpawnChessman(1, x, y);
            //    selectedChessman = Chessmans[x, y];
            //}
            //else if (y == 0) // black pawn become a queen
            //{
            //    Chessmans[x, y] = null;
            //    SpawnChessman(7, x, y);
            //    selectedChessman = Chessmans[x, y];
            //}

            ////White Pawn
            //if (selectedChessman.CurrentY == 1 && y == 3 && smartOpponentDoingTrials == false && smartOpponent == false)
            //{
            //    //Possible move of the black Pawn 
            //    EnPassantMove[0] = x;
            //    EnPassantMove[1] = y - 1;
            //}
            ////Black Pawn
            //else if (selectedChessman.CurrentY == 6 && y == 4 && smartOpponentDoingTrials == false && smartOpponent == false)
            //{
            //    //Possible move of the white Pawn
            //    EnPassantMove[0] = x;
            //    EnPassantMove[1] = y + 1;
            //}

        }



        Chessmans[selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
        selectedChessman.SetPosition(x, y);
        Chessmans[x, y] = selectedChessman;  // 更新当前棋子要移向点的棋子


        selectedChessman = null;//Select next Chessman
    }








    public void SpawnChessman(int index, int x, int y)    // index represent chess piece type
    {

        if (index == 0)
        {
            Chessmans[x, y] = new King();
            Chessmans[x, y].isWhite = true;
        }
        else if (index == 1)
        {
            Chessmans[x, y] = new Queen();
            Chessmans[x, y].isWhite = true;
        }
        else if (index == 2)
        {
            Chessmans[x, y] = new Rook();
            Chessmans[x, y].isWhite = true;
        }
        else if (index == 3)
        {
            Chessmans[x, y] = new Bishop();
            Chessmans[x, y].isWhite = true;
        }
        else if (index == 4)
        {
            Chessmans[x, y] = new Horse();
            Chessmans[x, y].isWhite = true;
        }
        else if (index == 5)
        {
            Chessmans[x, y] = new Pawn();
            Chessmans[x, y].isWhite = true;
        }
        else if (index == 6)
        {
            Chessmans[x, y] = new King();
            Chessmans[x, y].isWhite = false;
        }
        else if (index == 7)
        {
            Chessmans[x, y] = new Queen();
            Chessmans[x, y].isWhite = false;
        }
        else if (index == 8)
        {
            Chessmans[x, y] = new Rook();
            Chessmans[x, y].isWhite = false;
        }
        else if (index == 9)
        {
            Chessmans[x, y] = new Bishop();
            Chessmans[x, y].isWhite = false;
        }
        else if (index == 10)
        {
            Chessmans[x, y] = new Horse();
            Chessmans[x, y].isWhite = false;
        }
        else if (index == 11)
        {
            Chessmans[x, y] = new Pawn();
            Chessmans[x, y].isWhite = false;
        }


    }


    private void printCurrentBoardToConsole()
    {
        Debug.Log("-----棋盘样子---\n");
        string stringVersion = "";
        for (int uu = 7; uu >= 0; uu--)
        {
            for (int yy = 0; yy < 8; yy++)
            {
                if (Chessmans[yy, uu] != null)
                {

                    stringVersion += yy.ToString() + " " + uu.ToString() + " " + Chessmans[yy, uu].GetType().ToString() + "       ";

                }
                else
                {

                    stringVersion += yy.ToString() + " " + uu.ToString() + " " + "空子   " + "       ";
                }

            }
            stringVersion += "\n";
        }
        Debug.Log(stringVersion);
        Debug.Log("-----棋盘样子---\n");

    }

}
