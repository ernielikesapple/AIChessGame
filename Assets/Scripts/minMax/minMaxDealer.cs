using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class minMaxDealer 
{
    int maxDepth = 4;

    List<Chessman> _blackPieces = new List<Chessman>();   // for a certain round, current left black pieces
    List<Chessman> _whitePieces = new List<Chessman>();   // for a certain round, current left white pieces

    int _whiteScore = 0;
    int _blackScore = 0;


    private bool[,] allowedMoves { set; get; } // check if the move on the grid is allowed

    public void minMaxCoreAlgorithm() {
        

        AB(maxDepth, -100000000, 1000000000, true);

        Debug.Log("black score" + _blackScore + "whit score:" + _whiteScore);
        
    }


    int AB(int depth, int alpha, int beta, bool max)
    {
        getBoardState();

        if (depth == 0)
        {
            return _Evaluate();
        }
        if (max)
        {
            int score = -10000000;
            List<movement> allMoves = _GetMoves("BLACK");

            foreach (movement move in allMoves)
            {
                moveStack.Push(move);

                _DoFakeMove(move.firstPosition, move.secondPosition);

                score = AB(depth - 1, alpha, beta, false);

                _UndoFakeMove();

                if (score > alpha)
                {
                    move.score = score;
                    if (move.score > bestMove.score && depth == maxDepth)
                    {
                        bestMove = move;
                    }
                    alpha = score;
                }
                if (score >= beta)
                {
                    break;
                }
            }
            return alpha;
        }
        else
        {
            int score = 10000000;
            List<movement> allMoves = _GetMoves("WHITE");
            foreach (movement move in allMoves)
            {
                moveStack.Push(move);

                _DoFakeMove(move.firstPosition, move.secondPosition);

                score = AB(depth - 1, alpha, beta, true);

                _UndoFakeMove();

                if (score < beta)
                {
                    move.score = score;
                    beta = score;
                }
                if (score <= alpha)
                {
                    break;
                }
            }
            return beta;
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


    List<movement> _GetMoves(string PieceType)
    {
        List<movement> turnMove = new List<movement>();
        List<Chessman> pieces = new List<Chessman>();

        if (PieceType == "BLACK")
            pieces = _blackPieces;
        else pieces = _whitePieces;

        foreach (Chessman tile in pieces)
        {
            MoveFactory factory = new MoveFactory(_board);
            List<Move> pieceMoves = factory.GetMoves(tile.CurrentPiece, tile.Position);
            // found out for this pieces all the possible moves



            foreach (Move move in pieceMoves)
            {
                Move newMove = _CreateMove(move.firstPosition, move.secondPosition);
                turnMove.Add(newMove);
            }
        }
        return turnMove;
    }



   
}
