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

    Stack<Chessman[,]> currentBoardStateStack = new Stack<Chessman[,]>();
    
    public bestMoves minMaxCoreAlgorithm() {
        bestMoves bestMove = new bestMoves();
        bestMove = AB(0, -100000000, 1000000000, true, bestMove);

        return bestMove;
        //Debug.Log("black score" + _blackScore + "whit score:" + _whiteScore);
    }


    bestMoves AB(int depth, int alpha, int beta, bool max, bestMoves bestmoveInfoForEachNode)
    {
        getBoardState();

        if (depth == 3)
        {
            bestMoves bestMove = new bestMoves();
            bestMove.bestScore = _Evaluate();
            bestMove.bestSelectedPiece = bestmoveInfoForEachNode.bestSelectedPiece;
            bestMove.bestMoveTo = bestmoveInfoForEachNode.bestMoveTo;
            return bestMove;

        }
        if (max)
        {
            //int bestScore = -10000000;

            bestMoves bestMove = new bestMoves();
            bestMove.bestScore = -10000000;

            foreach (Chessman cm in _blackPieces)
            {
                BoardManager.Instance.allowedMoves = BoardManager.Instance.Chessmans[cm.CurrentX, cm.CurrentY].PossibleMove();

                
                BoardManager.Instance.selectedChessman = cm;
                if (depth == 0)
                {
                    bestMove.bestSelectedPiece = cm;
                    
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
                if(possibleMovesGrids.Count > 0)
                {
                    foreach (Vector2 move in possibleMovesGrids)
                    {
                        if (depth == 0)
                        {
                            bestMove.bestMoveTo.x = cm.CurrentX;
                            bestMove.bestMoveTo.y = cm.CurrentY;
                        }
                        // do fake move
                        currentBoardStateStack.Push(BoardManager.Instance.Chessmans);
                        BoardManager.Instance.MoveChessEssenceLogic((int)move.x, (int)move.y);
                        // update score
                        bestMove = AB(depth + 1, alpha, beta, false, bestmoveInfoForEachNode);
                        int value = bestMove.bestScore;
                        // undo fake move
                        BoardManager.Instance.Chessmans = currentBoardStateStack.Pop();
                        BoardManager.Instance.ReSpawnAllChessmansAccordingToCurrentChessmans(BoardManager.Instance.Chessmans);

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
            bestMoves bestMove = new bestMoves();
            bestMove.bestScore = 10000000;

            foreach (Chessman cm in _whitePieces)
            {
                BoardManager.Instance.allowedMoves = BoardManager.Instance.Chessmans[cm.CurrentX, cm.CurrentY].PossibleMove();
                BoardManager.Instance.selectedChessman = cm;

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
                        if (depth == 0)
                        {
                            bestMove.bestMoveTo.x = cm.CurrentX;
                            bestMove.bestMoveTo.y = cm.CurrentY;
                        }
                        // do fake move
                        currentBoardStateStack.Push(BoardManager.Instance.Chessmans);
                        BoardManager.Instance.MoveChessEssenceLogic((int)move.x, (int)move.y);
                        // update score
                        bestMove = AB(depth + 1, alpha, beta, true, bestmoveInfoForEachNode);
                        int value = bestMove.bestScore;
                        // undo fake move
                        BoardManager.Instance.Chessmans = currentBoardStateStack.Pop();
                        BoardManager.Instance.ReSpawnAllChessmansAccordingToCurrentChessmans(BoardManager.Instance.Chessmans);

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
        _blackPieces.Clear();
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
                else if (cm.GetType().ToString() == "Queen") {
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
