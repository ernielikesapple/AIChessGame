using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class minMaxDealer
{
    int maxDepth = 3; // count from 0

    List<Chessman> _blackPieces = new List<Chessman>();   // for a certain round, current left black pieces
    List<Chessman> _whitePieces = new List<Chessman>();   // for a certain round, current left white pieces

    int _whiteScore = 0;
    int _blackScore = 0;
    weightMatrix _weight = new weightMatrix();

    Stack<tempMovesInfo> currentBoardStateStack = new Stack<tempMovesInfo>();

    public bestMoves minMaxCoreAlgorithm()
    {
        bestMoves bestMove = new bestMoves();
        bestMove = AB(0, -100000000, 1000000000, true, bestMove);

        Debug.Log("bestMove 的信息" + "bestMove name" + bestMove.bestSelectedPiece.GetType().ToString() + "bestMove x===" + bestMove.bestMoveTo.x + "bestMove Y===" + bestMove.bestMoveTo.y);
        return bestMove;

    }


    bestMoves AB(int depth, int alpha, int beta, bool max, bestMoves bestmoveInfoForEachNode)
    {
        Debug.Log("ab1");
        getBoardState(); // get current state of the board, pass the value into AI class

        Debug.Log("ab2");

        if (depth == maxDepth)
        {
            Debug.Log("ab3");
            bestMoves bestMove = new bestMoves();
            bestMove.bestScore = _Evaluate();
            Debug.Log("ab3==最后叶节点层分数=="+ bestMove.bestScore);
            bestMove.bestSelectedPiece = bestmoveInfoForEachNode.bestSelectedPiece;
            bestMove.bestMoveTo = bestmoveInfoForEachNode.bestMoveTo;
            return bestMove;

        }
        if (max)
        {
            Debug.Log("黑棋轮次");
            //int bestScore = -10000000;

            bestMoves bestMove = new bestMoves();
            bestMove.bestScore = -10000000;

            foreach (Chessman cm in _blackPieces)
            {
                BoardManager.Instance.allowedMoves = BoardManager.Instance.Chessmans[cm.CurrentX, cm.CurrentY].PossibleMove();

                BoardManager.Instance.selectedChessman = cm;
                Debug.Log("递归层数" + depth + "ab当前黑棋外层 选中棋子" + cm.GetType().ToString() + "坐标x：" + cm.CurrentX + "坐标y：" + cm.CurrentY);
                if (depth == 0)
                {
                    bestMove.bestSelectedPiece = cm;  // 记录第0层选中的黑棋
                }
                else
                {
                    bestMove.bestSelectedPiece = bestmoveInfoForEachNode.bestSelectedPiece;
                }

                // enumerate all the moves
                List<Vector2> possibleMovesGrids = new List<Vector2>(); //电脑黑子可走的位置
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (BoardManager.Instance.allowedMoves[i, j])
                        {
                            possibleMovesGrids.Add(new Vector2(i, j)); // 查看当前棋子有没有可以走的地方
                        }
                    }
                }
                if (possibleMovesGrids.Count > 0)
                {
                    foreach (Vector2 move in possibleMovesGrids)
                    {
                        if (depth == 0)
                        {
                            bestMove.bestMoveTo.x = move.x; // 记录黑棋第零层时，选中黑棋要走向的点的坐标
                            bestMove.bestMoveTo.y = move.y;
                        }
                        else
                        {
                            bestMove.bestMoveTo.x = bestmoveInfoForEachNode.bestMoveTo.x; // 记录黑棋第零层时，选中黑棋要走向的点的坐标
                            bestMove.bestMoveTo.y = bestmoveInfoForEachNode.bestMoveTo.y;
                        }
                        Debug.Log("当前选中棋子有可走地方 当前递归层数" + depth + "black score" + _blackScore + "whit score:" + _whiteScore  + "全体返回出去后选中的黑棋名字是：" + bestMove.bestSelectedPiece.GetType().ToString() + " x===" + bestMove.bestMoveTo.x + " Y===" + bestMove.bestMoveTo.y);
                        // do fake move

                        // 用stack 保存当前走的那一步的坐标， 棋子，cm， move，   吃子信息， 被吃子名 
                        tempMovesInfo currentTempMovesInfo = new tempMovesInfo();
                        currentTempMovesInfo.currentTrialPiece = cm;
                        currentTempMovesInfo.currentTrialPieceCoord = new Vector2(cm.CurrentX, cm.CurrentY);
                        currentTempMovesInfo.movementInfo = move;
                        if ( BoardManager.Instance.Chessmans[(int)move.x,(int)move.y] != null) {
                            currentTempMovesInfo.pieceGotEaten = BoardManager.Instance.Chessmans[(int)move.x, (int)move.y].GetType().ToString();
                        }                        

                        currentBoardStateStack.Push(currentTempMovesInfo);
                        
                        Debug.Log("黑0000黑子名" + cm.GetType().ToString() + "从坐标x， y  (" + cm.CurrentX +"," + cm.CurrentY +  ")" + "要移向点的x信息：" + move.x + "要移向点的y信息：" + move.y);
                        BoardManager.Instance.MoveChessEssenceLogic((int)move.x, (int)move.y);
                        // update score
                        Debug.Log("黑11111棋盘当前在x， y是：" + cm.CurrentX + "," + cm.CurrentY);
                        bestMove = AB(depth + 1, alpha, beta, false, bestMove);
                        int value = bestMove.bestScore;
                        // undo fake move
                        // todo write function to move one piece back， and check if there is another piece then put it back
                        tempMovesInfo formerTempMovesInfo = currentBoardStateStack.Pop();
                        cm.SetPosition((int)formerTempMovesInfo.currentTrialPieceCoord.x, (int)formerTempMovesInfo.currentTrialPieceCoord.y);
                        BoardManager.Instance.selectedChessman = cm;
                        BoardManager.Instance.Chessmans[(int)formerTempMovesInfo.currentTrialPieceCoord.x, (int)formerTempMovesInfo.currentTrialPieceCoord.y] = cm;
                        if (formerTempMovesInfo.pieceGotEaten != null) { // regenerate the eaten piece
                            if (cm.GetType().ToString() == "King")
                            {
                                BoardManager.Instance.SpawnChessman(0, (int)formerTempMovesInfo.movementInfo.x, (int)formerTempMovesInfo.movementInfo.y);
                            }
                            else if (cm.GetType().ToString() == "Queen")
                            {
                                BoardManager.Instance.SpawnChessman(1, (int)formerTempMovesInfo.movementInfo.x, (int)formerTempMovesInfo.movementInfo.y);
                            }
                            else if (cm.GetType().ToString() == "Rook")
                            {
                                BoardManager.Instance.SpawnChessman(2, (int)formerTempMovesInfo.movementInfo.x, (int)formerTempMovesInfo.movementInfo.y);
                            }
                            else if (cm.GetType().ToString() == "Bishop")
                            {
                                BoardManager.Instance.SpawnChessman(3, (int)formerTempMovesInfo.movementInfo.x, (int)formerTempMovesInfo.movementInfo.y);
                            }
                            else if (cm.GetType().ToString() == "Horse")
                            {
                                BoardManager.Instance.SpawnChessman(4, (int)formerTempMovesInfo.movementInfo.x, (int)formerTempMovesInfo.movementInfo.y);
                            }
                            else if (cm.GetType().ToString() == "Pawn")
                            {
                                BoardManager.Instance.SpawnChessman(5, (int)formerTempMovesInfo.movementInfo.x, (int)formerTempMovesInfo.movementInfo.y);
                            }
                        }

                        Debug.Log("黑333 棋盘当前在x， y  (" + cm.CurrentX +"," + cm.CurrentY +") 上的棋子名字是" + BoardManager.Instance.Chessmans[cm.CurrentX, cm.CurrentY].GetType().ToString());
                      
                        bestMove.bestScore = Math.Max(bestMove.bestScore, value);
                        alpha = Math.Max(alpha, bestMove.bestScore);

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
            return bestMove;
        }
        else
        {
            Debug.Log("白棋轮次");
            bestMoves bestMove = new bestMoves();
            bestMove.bestScore = 10000000;

            foreach (Chessman cm in _whitePieces)
            {
                Debug.Log("递归层数" + depth+ "ab当前白棋外层 选中棋子" + cm.GetType().ToString());
                BoardManager.Instance.allowedMoves = BoardManager.Instance.Chessmans[cm.CurrentX, cm.CurrentY].PossibleMove();
                BoardManager.Instance.selectedChessman = cm;

                bestMove.bestSelectedPiece = bestmoveInfoForEachNode.bestSelectedPiece;// 记录第0层选中的黑棋
                Debug.Log("当前选中 白 棋子有可走地方 当前递归层数" + depth + "black score" + _blackScore + "whit score:" + _whiteScore + "全体返回出去后选中的黑棋名字是：" + bestMove.bestSelectedPiece.GetType().ToString() + " x===" + bestMove.bestMoveTo.x + " Y===" + bestMove.bestMoveTo.y);
                // enumerate all the moves
                List<Vector2> possibleMovesGrids = new List<Vector2>(); //电脑白子可走的位置
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (BoardManager.Instance.allowedMoves[i, j])
                        {
                            possibleMovesGrids.Add(new Vector2(i, j)); // 查看当前棋子有没有可以走的地方
                        }
                    }
                }
                if (possibleMovesGrids.Count > 0)
                {
                    foreach (Vector2 move in possibleMovesGrids)
                    {
                        bestMove.bestMoveTo.x = bestmoveInfoForEachNode.bestMoveTo.x; // 记录黑棋第零层时，选中黑棋要走向的点的坐标
                        bestMove.bestMoveTo.y = bestmoveInfoForEachNode.bestMoveTo.y;

                        // do fake move
                        tempMovesInfo currentTempMovesInfo = new tempMovesInfo();
                        currentTempMovesInfo.currentTrialPiece = cm;
                        currentTempMovesInfo.currentTrialPieceCoord = new Vector2(cm.CurrentX, cm.CurrentY);
                        currentTempMovesInfo.movementInfo = move;
                        if (BoardManager.Instance.Chessmans[(int)move.x, (int)move.y] != null)
                        {
                            currentTempMovesInfo.pieceGotEaten = BoardManager.Instance.Chessmans[(int)move.x, (int)move.y].GetType().ToString();
                        }
                        currentBoardStateStack.Push(currentTempMovesInfo);

                        BoardManager.Instance.MoveChessEssenceLogic((int)move.x, (int)move.y);
                        // update score
                        Debug.Log("白11111");
                        bestMove = AB(depth + 1, alpha, beta, true, bestMove);
                        Debug.Log("白2222");
                        int value = bestMove.bestScore;
                        // undo fake move

                        tempMovesInfo formerTempMovesInfo = currentBoardStateStack.Pop();
                        cm.SetPosition((int)formerTempMovesInfo.currentTrialPieceCoord.x, (int)formerTempMovesInfo.currentTrialPieceCoord.y);
                        BoardManager.Instance.selectedChessman = cm;
                        BoardManager.Instance.Chessmans[(int)formerTempMovesInfo.currentTrialPieceCoord.x, (int)formerTempMovesInfo.currentTrialPieceCoord.y] = cm;
                        if (formerTempMovesInfo.pieceGotEaten != null)
                        { // regenerate the eaten piece
                            if (cm.GetType().ToString() == "King")
                            {
                                BoardManager.Instance.SpawnChessman(6, (int)formerTempMovesInfo.movementInfo.x, (int)formerTempMovesInfo.movementInfo.y);
                            }
                            else if (cm.GetType().ToString() == "Queen")
                            {
                                BoardManager.Instance.SpawnChessman(7, (int)formerTempMovesInfo.movementInfo.x, (int)formerTempMovesInfo.movementInfo.y);
                            }
                            else if (cm.GetType().ToString() == "Rook")
                            {
                                BoardManager.Instance.SpawnChessman(8, (int)formerTempMovesInfo.movementInfo.x, (int)formerTempMovesInfo.movementInfo.y);
                            }
                            else if (cm.GetType().ToString() == "Bishop")
                            {
                                BoardManager.Instance.SpawnChessman(9, (int)formerTempMovesInfo.movementInfo.x, (int)formerTempMovesInfo.movementInfo.y);
                            }
                            else if (cm.GetType().ToString() == "Horse")
                            {
                                BoardManager.Instance.SpawnChessman(10, (int)formerTempMovesInfo.movementInfo.x, (int)formerTempMovesInfo.movementInfo.y);
                            }
                            else if (cm.GetType().ToString() == "Pawn")
                            {
                                BoardManager.Instance.SpawnChessman(11, (int)formerTempMovesInfo.movementInfo.x, (int)formerTempMovesInfo.movementInfo.y);
                            }
                        }


                        bestMove.bestScore = Math.Max(bestMove.bestScore, value);
                        beta = Math.Max(beta, bestMove.bestScore);

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
            return bestMove;
        }
    }


    public void getBoardState()
    {
        _blackPieces.Clear();  // declared variables to store active white black info on the current board
        _whitePieces.Clear();
        _blackScore = 0;
        _whiteScore = 0;


        foreach (GameObject activeChessPiece in BoardManager.Instance.activeChessman)  // BoardManager.Instance.activeChessman use the real board chess man to move around and check
        {
            Chessman cm = activeChessPiece.GetComponent<Chessman>();
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





}
