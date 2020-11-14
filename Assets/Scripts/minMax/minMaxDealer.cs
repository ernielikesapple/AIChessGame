using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class minMaxDealer
{
    int maxDepth = 3; // todo : originally 3, for testing purpose just set 2 temporarily // count from 0

    List<Chessman> _blackPieces = new List<Chessman>();   // for a certain round, current left black pieces
    List<Chessman> _whitePieces = new List<Chessman>();   // for a certain round, current left white pieces

    int _whiteScore = 0;
    int _blackScore = 0;
    weightMatrix _weight = new weightMatrix();

    Stack<tempMovesInfo> currentBoardStateStack = new Stack<tempMovesInfo>();

    bestMoves bestMoveFromMinMax = new bestMoves();

    public bestMoves minMaxCoreAlgorithm(Chessman[,] currentBoardManagerChessmans, Chessman currentSelectedPiece, int maxDepthFromBM)
    {
        maxDepth = maxDepthFromBM;            //maxDepth = maxDepthFromBM;
        fakeBoardManager.Instance = new fakeBoardManager();
        fakeBoardManager.Instance.Chessmans = currentBoardManagerChessmans; // get current board real information
        fakeBoardManager.Instance.selectedChessman = currentSelectedPiece;

        bestMoveFromMinMax.bestScore = -100000000;
        AB(0, -100000000, 1000000000, true);

        Debug.Log("bestMove 的信息" + "bestMove name: ----=!!!" + fakeBoardManager.Instance.Chessmans[(int)bestMoveFromMinMax.bestSelectedPieceCoord.x, (int)bestMoveFromMinMax.bestSelectedPieceCoord.y].GetType().ToString() + "======bestMove x===" + bestMoveFromMinMax.bestMoveTo.x + "bestMove Y===" + bestMoveFromMinMax.bestMoveTo.y);
        return bestMoveFromMinMax;

    }

    string format = " ";
    string formatq = "====================||";
    int AB(int depth, int alpha, int beta, bool max)
    {
     
        format += formatq;
        getBoardState(); // get current state of the board, pass the value into AI class
        if (depth == maxDepth)
        {
            Debug.Log(format+"ab3==最后叶节点层分数=="+ _Evaluate());
            return _Evaluate();
        }
        if (max)
        {
            Debug.Log(format+"黑棋轮次");
            int bestScoreForMaxNode = -10000000;

            for(int zz = 0 ; zz < _blackPieces.Count; zz++) // don't use foreach, since for each does not allow you to modifiy the content of each chessman
            {
                Chessman cm = _blackPieces[zz];
                Debug.Log(format+"黑棋规格1===:"+ _blackPieces.Count);
                fakeBoardManager.Instance.allowedMoves = fakeBoardManager.Instance.Chessmans[cm.CurrentX, cm.CurrentY].PossibleMove();
                fakeBoardManager.Instance.selectedChessman = cm;
                Debug.Log(format+"递归层数" + depth + "ab当前黑棋外层 选中棋子" + cm.GetType().ToString() +"棋的颜色是白色吗" + cm.isWhite + "当前的 坐标x：" + cm.CurrentX + "坐标y：" + cm.CurrentY);
                
                // enumerate all the moves
                List<Vector2> possibleMovesGrids = new List<Vector2>(); //电脑黑子可走的位置

                Debug.Log("当前选中黑棋子在还没进入 possible moves 循环时候 移动前： allowed move的信息");

                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fakeBoardManager.Instance.allowedMoves[i, j])
                        {
                            possibleMovesGrids.Add(new Vector2(i, j)); // 查看当前棋子有没有可以走的地方
                        }
                    }
                }
                if (possibleMovesGrids.Count > 0)
                {
                    foreach (Vector2 move in possibleMovesGrids)
                    {
                        if(bestMoveFromMinMax.bestSelectedPieceCoord.x != -1 && fakeBoardManager.Instance.Chessmans[(int)bestMoveFromMinMax.bestSelectedPieceCoord.x, (int)bestMoveFromMinMax.bestSelectedPieceCoord.y] != null) { 
                            Debug.Log(format+"当前选中棋子有可走地方 当前递归层数" + depth + "black score" + _blackScore + "whit score:" + _whiteScore  + "全体返回出去后选中的黑棋名字是：" + fakeBoardManager.Instance.Chessmans[(int)bestMoveFromMinMax.bestSelectedPieceCoord.x, (int)bestMoveFromMinMax.bestSelectedPieceCoord.y].GetType().ToString() + " x===" + bestMoveFromMinMax.bestMoveTo.x + " Y===" + bestMoveFromMinMax.bestMoveTo.y);
                        }
                        // do fake move

                        // 用stack 保存当前走的那一步的坐标， 棋子，cm， move，   吃子信息， 被吃子名 
                        tempMovesInfo currentTempMovesInfo = new tempMovesInfo();
                        currentTempMovesInfo.currentTrialPiece = cm;
                        currentTempMovesInfo.currentTrialPieceCoord = new Vector2(cm.CurrentX, cm.CurrentY);
                        currentTempMovesInfo.movementInfo = move;
                        if ( fakeBoardManager.Instance.Chessmans[(int)move.x,(int)move.y] != null) {  // 被吃子名 
                            currentTempMovesInfo.pieceGotEaten = fakeBoardManager.Instance.Chessmans[(int)move.x, (int)move.y];
                        }                        

                        currentBoardStateStack.Push(currentTempMovesInfo);
                        
                        Debug.Log(format+"黑0000黑子名" + cm.GetType().ToString() + "从坐标x， y  (" + cm.CurrentX +"," + cm.CurrentY +  ")" + "要移向点的x信息：" + move.x + "要移向点的y信息：" + move.y);


                        Debug.Log("黑棋 移动前棋盘样子" + " 黑棋现存个数： " + _blackPieces.Count + "\n");
                        printCurrentBoardToConsole();

                        fakeBoardManager.Instance.selectedChessman = cm;
                        fakeBoardManager.Instance.allowedMoves = cm.PossibleMove(); //todo： 是否需要在这里更新？？这行代码有点多余？？

                        Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@@当前选中棋子在尝试不同 possible moves 时候 移动前： allowed move的信息");
                        printAllowedMBoardToConsole();

                        fakeBoardManager.Instance.MoveChessEssenceLogic((int)move.x, (int)move.y);
                        // update score
                        Debug.Log(format+"黑11111棋盘当前在x， y是：" + cm.CurrentX + "," + cm.CurrentY);

                        Debug.Log("黑棋 移动后棋盘样子" + " 黑棋现存个数： " + _blackPieces.Count + "\n");
                        printCurrentBoardToConsole();

                        int value = AB(depth + 1, alpha, beta, false);

                        Debug.Log("黑棋 移动后棋盘样子 value value value" + value);

                        format = format.Substring(0, format.Length - formatq.Length);

                        // undo fake move
                        tempMovesInfo formerTempMovesInfo = currentBoardStateStack.Pop();
                        //fakeBoardManager.Instance.Chessmans[(int)cm.CurrentX, (int)cm.CurrentY] = null; // 把原来位置置空
                        //Debug.Log("黑棋前一步移动过的棋子的位置坐标x:" + (int)cm.CurrentX + "Y:" + (int)cm.CurrentY);
                        //cm.SetPosition((int)formerTempMovesInfo.currentTrialPieceCoord.x, (int)formerTempMovesInfo.currentTrialPieceCoord.y);

                        fakeBoardManager.Instance.selectedChessman = formerTempMovesInfo.currentTrialPiece;

                        fakeBoardManager.Instance.MoveChessBackEssenceLogic((int)formerTempMovesInfo.currentTrialPieceCoord.x, (int)formerTempMovesInfo.currentTrialPieceCoord.y);
                        //fakeBoardManager.Instance.selectedChessman = cm;

                        fakeBoardManager.Instance.Chessmans[(int)formerTempMovesInfo.currentTrialPieceCoord.x, (int)formerTempMovesInfo.currentTrialPieceCoord.y] = cm;
                        if (formerTempMovesInfo.pieceGotEaten != null)  // regenerate the eaten piece //   after undo fake move， and check if there is another piece then put it back
                        {
                            fakeBoardManager.Instance.Chessmans[(int)formerTempMovesInfo.movementInfo.x, (int)formerTempMovesInfo.movementInfo.y] = formerTempMovesInfo.pieceGotEaten;
                            //if (cm.GetType().ToString() == "King") 
                            //{
                            //    fakeBoardManager.Instance.SpawnChessman(0, (int)formerTempMovesInfo.movementInfo.x, (int)formerTempMovesInfo.movementInfo.y);
                            //}
                            //else if (cm.GetType().ToString() == "Queen")
                            //{
                            //    fakeBoardManager.Instance.SpawnChessman(1, (int)formerTempMovesInfo.movementInfo.x, (int)formerTempMovesInfo.movementInfo.y);
                            //}
                            //else if (cm.GetType().ToString() == "Rook")
                            //{
                            //    fakeBoardManager.Instance.SpawnChessman(2, (int)formerTempMovesInfo.movementInfo.x, (int)formerTempMovesInfo.movementInfo.y);
                            //}
                            //else if (cm.GetType().ToString() == "Bishop")
                            //{
                            //    fakeBoardManager.Instance.SpawnChessman(3, (int)formerTempMovesInfo.movementInfo.x, (int)formerTempMovesInfo.movementInfo.y);
                            //}
                            //else if (cm.GetType().ToString() == "Horse")
                            //{
                            //    fakeBoardManager.Instance.SpawnChessman(4, (int)formerTempMovesInfo.movementInfo.x, (int)formerTempMovesInfo.movementInfo.y);
                            //}
                            //else if (cm.GetType().ToString() == "Pawn")
                            //{
                            //    fakeBoardManager.Instance.SpawnChessman(5, (int)formerTempMovesInfo.movementInfo.x, (int)formerTempMovesInfo.movementInfo.y);
                            //}
                        }

                        Debug.Log(format+"黑333 棋盘当前在x， y  (" + cm.CurrentX +"," + cm.CurrentY +") 上的棋子名字是" + fakeBoardManager.Instance.Chessmans[cm.CurrentX, cm.CurrentY].GetType().ToString());
                        
                        Debug.Log("黑棋 复盘后 棋盘样子" + " 黑棋现存个数： " + _blackPieces.Count + "\n");
                        printCurrentBoardToConsole();

                        Debug.Log(format + "黑2222---bestValue" + bestScoreForMaxNode + "alpha" + alpha + "beta" + beta);

                        bestScoreForMaxNode = Math.Max(bestScoreForMaxNode, value);

                        if (depth == 0 && bestScoreForMaxNode > bestMoveFromMinMax.bestScore)
                        {
                            bestMoveFromMinMax.bestSelectedPieceCoord.x = cm.CurrentX;
                            bestMoveFromMinMax.bestSelectedPieceCoord.y = cm.CurrentY;
                            bestMoveFromMinMax.bestMoveTo.x = move.x; // 记录黑棋第零层时，选中黑棋要走向的点的坐标
                            bestMoveFromMinMax.bestMoveTo.y = move.y;
                            bestMoveFromMinMax.bestScore = bestScoreForMaxNode;
                        }
                        

                        alpha = Math.Max(alpha, bestScoreForMaxNode);

                        if (beta <= alpha)
                        {
                            break;
                        }
                        Debug.Log(format+"黑棋规格2===:" + _blackPieces.Count);
                    }
                }
                else
                {
                    continue;
                }
            }
            return bestScoreForMaxNode;
        }
        else
        {
            Debug.Log(format+"白棋轮次");
            int bestScoreForMinNode = 10000000;


            for (int zzw = 0; zzw < _whitePieces.Count; zzw++)
            {
                Chessman cm = _whitePieces[zzw];
                Debug.Log(format+"递归层数" + depth+ "ab当前白棋外层 选中棋子" + cm.GetType().ToString() + "棋的颜色是白色吗" + cm.isWhite + "坐标x：" + cm.CurrentX + "坐标y：" + cm.CurrentY);

                fakeBoardManager.Instance.allowedMoves = fakeBoardManager.Instance.Chessmans[cm.CurrentX, cm.CurrentY].PossibleMove();
                fakeBoardManager.Instance.selectedChessman = cm;

                // enumerate all the moves
                List<Vector2> possibleMovesGrids = new List<Vector2>(); //电脑白子可走的位置
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (fakeBoardManager.Instance.allowedMoves[i, j])
                        {
                            possibleMovesGrids.Add(new Vector2(i, j)); // 查看当前棋子有没有可以走的地方
                        }
                    }
                }

                if (possibleMovesGrids.Count > 0)
                {
                    Debug.Log("出错点0 possible moves count:" + possibleMovesGrids.Count);
                    if (bestMoveFromMinMax.bestSelectedPieceCoord.x != -1 && fakeBoardManager.Instance.Chessmans[(int)bestMoveFromMinMax.bestSelectedPieceCoord.x, (int)bestMoveFromMinMax.bestSelectedPieceCoord.y] != null)
                    {
                        Debug.Log(format + "当前选中 白 棋子有可走地方 当前递归层数" + depth + "black score" + _blackScore + "whit score:" + _whiteScore + "全体返回出去后选中的黑棋名字是：" + fakeBoardManager.Instance.Chessmans[(int)bestMoveFromMinMax.bestSelectedPieceCoord.x, (int)bestMoveFromMinMax.bestSelectedPieceCoord.y].GetType().ToString() + " x===" + bestMoveFromMinMax.bestMoveTo.x + " Y===" + bestMoveFromMinMax.bestMoveTo.y);
                    }
                    foreach (Vector2 move in possibleMovesGrids)
                    {
                        // do fake move
                        tempMovesInfo currentTempMovesInfo = new tempMovesInfo();

                        currentTempMovesInfo.currentTrialPiece = cm;
                        Debug.Log("出错点白3！！！" + "cm.current x:" + cm.CurrentX + "cm.current Y:" + cm.CurrentY);
                        currentTempMovesInfo.currentTrialPieceCoord = new Vector2(cm.CurrentX, cm.CurrentY);
                        currentTempMovesInfo.movementInfo = move;
                        if (fakeBoardManager.Instance.Chessmans[(int)move.x, (int)move.y] != null)
                        {
                            currentTempMovesInfo.pieceGotEaten = fakeBoardManager.Instance.Chessmans[(int)move.x, (int)move.y];
                        }
                        currentBoardStateStack.Push(currentTempMovesInfo);

                        Debug.Log("白棋 移动前棋盘样子" + " 白棋现存个数： " + _blackPieces.Count + "\n");
                        printCurrentBoardToConsole();

                        fakeBoardManager.Instance.selectedChessman = cm;
                        fakeBoardManager.Instance.allowedMoves = cm.PossibleMove();
                        fakeBoardManager.Instance.MoveChessEssenceLogic((int)move.x, (int)move.y);
                        // update score
                        Debug.Log(format+"白11111");

                        Debug.Log("白棋 移动后棋盘样子" + " 白棋现存个数： " + _blackPieces.Count + "\n");
                        printCurrentBoardToConsole();

                        int value = AB(depth + 1, alpha, beta, true);

                        format = format.Substring(0, format.Length - formatq.Length);
                        Debug.Log(format+"白2222");
                        // undo fake move
                        Debug.Log(format + "白2222---value" + bestScoreForMinNode + "alpha" + alpha + "beta" + alpha);
                        tempMovesInfo formerTempMovesInfo = currentBoardStateStack.Pop();

                        //fakeBoardManager.Instance.Chessmans[(int)cm.CurrentX, (int)cm.CurrentY] = null; // 把原来位置置空
                        //Debug.Log("出错点白1！！！" + "cm.current x:" + cm.CurrentX + "cm.current Y:" + cm.CurrentY);
                        //cm.SetPosition((int)formerTempMovesInfo.currentTrialPieceCoord.x, (int)formerTempMovesInfo.currentTrialPieceCoord.y);
                        //Debug.Log("出错点白2！！！" + "cm.current x:" + cm.CurrentX + "cm.current Y:" + cm.CurrentY);

                        fakeBoardManager.Instance.selectedChessman = formerTempMovesInfo.currentTrialPiece;
                        fakeBoardManager.Instance.MoveChessBackEssenceLogic((int)formerTempMovesInfo.currentTrialPieceCoord.x, (int)formerTempMovesInfo.currentTrialPieceCoord.y);
                        //fakeBoardManager.Instance.selectedChessman = cm;

                        fakeBoardManager.Instance.Chessmans[(int)formerTempMovesInfo.currentTrialPieceCoord.x, (int)formerTempMovesInfo.currentTrialPieceCoord.y] = cm;
                        if (formerTempMovesInfo.pieceGotEaten != null)
                        { // regenerate the eaten piece
                            
                            fakeBoardManager.Instance.Chessmans[(int)formerTempMovesInfo.movementInfo.x, (int)formerTempMovesInfo.movementInfo.y] = formerTempMovesInfo.pieceGotEaten;

                            //if (cm.GetType().ToString() == "King")
                            //{
                            //    fakeBoardManager.Instance.SpawnChessman(6, (int)formerTempMovesInfo.movementInfo.x, (int)formerTempMovesInfo.movementInfo.y);
                            //}
                            //else if (cm.GetType().ToString() == "Queen")
                            //{
                            //    fakeBoardManager.Instance.SpawnChessman(7, (int)formerTempMovesInfo.movementInfo.x, (int)formerTempMovesInfo.movementInfo.y);
                            //}
                            //else if (cm.GetType().ToString() == "Rook")
                            //{
                            //    fakeBoardManager.Instance.SpawnChessman(8, (int)formerTempMovesInfo.movementInfo.x, (int)formerTempMovesInfo.movementInfo.y);
                            //}
                            //else if (cm.GetType().ToString() == "Bishop")
                            //{
                            //    fakeBoardManager.Instance.SpawnChessman(9, (int)formerTempMovesInfo.movementInfo.x, (int)formerTempMovesInfo.movementInfo.y);
                            //}
                            //else if (cm.GetType().ToString() == "Horse")
                            //{
                            //    fakeBoardManager.Instance.SpawnChessman(10, (int)formerTempMovesInfo.movementInfo.x, (int)formerTempMovesInfo.movementInfo.y);
                            //}
                            //else if (cm.GetType().ToString() == "Pawn")
                            //{
                            //    fakeBoardManager.Instance.SpawnChessman(11, (int)formerTempMovesInfo.movementInfo.x, (int)formerTempMovesInfo.movementInfo.y);
                            //}
                        }

                        Debug.Log("白棋 复盘后 棋盘样子" + " 黑棋现存个数： " + _blackPieces.Count + "\n");
                        printCurrentBoardToConsole();

                        bestScoreForMinNode = Math.Min(bestScoreForMinNode, value);
                        beta = Math.Min(beta, bestScoreForMinNode);

                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    continue;
                }
            }
            return bestScoreForMinNode;
        }
    }


    public void getBoardState()
    {
        _blackPieces.Clear();  // declared variables to store active white black info on the current board
        _whitePieces.Clear();
        _blackScore = 0;
        _whiteScore = 0;


        foreach (Chessman cm in fakeBoardManager.Instance.Chessmans)  // fakeBoardManager.Instance.activeChessman use the real board chess man to move around and check
        {
            if (cm != null)
            {
                if (cm.isWhite == false) //黑子
                {
                    if (cm.GetType().ToString() == "King")
                    {
                        int x1 = cm.CurrentX;
                        int y1 = cm.CurrentY;

                        // calculate the score for black part

                        _blackScore += 1000000;
                        _blackPieces.Add(cm);
                    }
                    else if (cm.GetType().ToString() == "Queen")
                    {
                        int x1 = cm.CurrentX;
                        int y1 = cm.CurrentY;

                        _blackScore += 9;
                        _blackPieces.Add(cm);
                    }
                    else if (cm.GetType().ToString() == "Rook")
                    {
                        int x1 = cm.CurrentX;
                        int y1 = cm.CurrentY;
                        _blackScore += 5;
                        _blackPieces.Add(cm);

                    }
                    else if (cm.GetType().ToString() == "Bishop")
                    {
                        int x1 = cm.CurrentX;
                        int y1 = cm.CurrentY;

                        _blackScore += 3;
                        _blackPieces.Add(cm);

                    }
                    else if (cm.GetType().ToString() == "Horse")
                    {
                        int x1 = cm.CurrentX;
                        int y1 = cm.CurrentY;
                        _blackScore += 3;
                        _blackPieces.Add(cm);
                    }
                    else if (cm.GetType().ToString() == "Pawn")
                    {
                        int x1 = cm.CurrentX;
                        int y1 = cm.CurrentY;

                        _blackScore += 1;
                        _blackPieces.Add(cm);
                    }

                }
                else // 对应的种类的白子
                {
                    if (cm.GetType().ToString() == "King")
                    {
                        int x1 = cm.CurrentX;
                        int y1 = cm.CurrentY;
                        // calculate the score for white part
                        _whiteScore += 1000000;
                        _whitePieces.Add(cm);
                    }
                    else if (cm.GetType().ToString() == "Queen")
                    {
                        int x1 = cm.CurrentX;
                        int y1 = cm.CurrentY;
                        _whiteScore += 9;
                        _whitePieces.Add(cm);
                    }
                    else if (cm.GetType().ToString() == "Rook")
                    {
                        int x1 = cm.CurrentX;
                        int y1 = cm.CurrentY;
                        _whiteScore += 5;
                        _whitePieces.Add(cm);
                    }
                    else if (cm.GetType().ToString() == "Bishop")
                    {
                        int x1 = cm.CurrentX;
                        int y1 = cm.CurrentY;
                        _whiteScore += 3;
                        _whitePieces.Add(cm);
                    }
                    else if (cm.GetType().ToString() == "Horse")
                    {
                        int x1 = cm.CurrentX;
                        int y1 = cm.CurrentY;
                        _whiteScore += 3;
                        _whitePieces.Add(cm);
                    }
                    else if (cm.GetType().ToString() == "Pawn")
                    {
                        int x1 = cm.CurrentX;
                        int y1 = cm.CurrentY;
                        _whiteScore += 1;
                        _whitePieces.Add(cm);
                    }
                }
            }
        }

    }


    int _Evaluate()
    {
        float pieceDifference = 0;
        float whiteWeight = 0;
        float blackWeight = 0;

        foreach (Chessman cm in _whitePieces)
        {
            Vector2 position = new Vector2((int)cm.CurrentX, (int)cm.CurrentY);
            whiteWeight += _weight.GetBoardWeight(cm.GetType().ToString(), position, "white");
        }
        foreach (Chessman cm in _blackPieces)
        {
            Vector2 position = new Vector2((int)cm.CurrentX, (int)cm.CurrentY);
            blackWeight += _weight.GetBoardWeight(cm.GetType().ToString(), position, "black");
        }
        pieceDifference = (_blackScore + (blackWeight / 100)) - (_whiteScore + (whiteWeight / 100));
        return Mathf.RoundToInt(pieceDifference * 100);
    }




    private void printCurrentBoardToConsole() {

        Debug.Log("-----棋盘样子---\n");
        string stringVersion = "";
        for (int uu = 7; uu >= 0; uu--)
        {
            for (int yy = 0; yy < 8; yy++)
            {
                if (fakeBoardManager.Instance.Chessmans[yy, uu] != null)
                {

                    stringVersion += yy.ToString() + " " + uu.ToString() + " " + fakeBoardManager.Instance.Chessmans[yy, uu].GetType().ToString() + "       ";

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


    private void printAllowedMBoardToConsole() {

        Debug.Log("-----棋盘allowedmoves样子---\n");
        string stringVersion = "";
        for (int uu = 7; uu >= 0; uu--)
        {
            for (int yy = 0; yy < 8; yy++)
            {
                

             stringVersion += yy.ToString() + " " + uu.ToString() + " " + fakeBoardManager.Instance.allowedMoves[yy, uu] + "       ";

               
            }
            stringVersion += "\n";
        }
        Debug.Log(stringVersion);
        Debug.Log("-----棋盘allowedmoves样子---\n");

    }

    

}
