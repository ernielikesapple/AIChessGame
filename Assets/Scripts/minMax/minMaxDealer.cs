using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class minMaxDealer 
{
    int maxDepth = 4;
    List<GameObject> activeChessmanVarFromTheRealGame; // represent current states of the board

    List<Chessman> _blackPieces = new List<Chessman>();   // for a certain round, current left black pieces
    List<Chessman> _whitePieces = new List<Chessman>();   // for a certain round, current left white pieces

    int _whiteScore = 0;
    int _blackScore = 0;



    private bool[,] allowedMoves { set; get; } // check if the move on the grid is allowed

    public void minMaxCoreAlgorithm(List<GameObject> activeChessman) {
        activeChessmanVarFromTheRealGame = activeChessman;

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

        foreach (GameObject activeChessPiece in activeChessmanVarFromTheRealGame)
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















    private void MoveChessEssenceLogic(int x, int y)
    {
        if (allowedMoves[x, y])
        {
            Chessman c = Chessmans[x, y]; // 落子点

            if (c != null && c.isWhite != isWhiteTurn)
            {
                //Capture a piece

                //If it is the King
                if (c.GetType() == typeof(King))
                {
                    EndGame();
                    //Rerecord the game
                    return;
                }
                activeChessman.Remove(c.gameObject);
                
            }
            //EnPassantMove(The first nove of the black Pawn is two square, then the white Pawn can remove it)
            if (x == EnPassantMove[0] && y == EnPassantMove[1])
            {
                //White turn(black Pawn move 2 squares)
                if (isWhiteTurn)
                {
                    c = Chessmans[x, y - 1];
                    activeChessman.Remove(c.gameObject);
                    
                }
                //Black turn(white Pawn move 2 squares)
                else
                {
                    c = Chessmans[x, y + 1];
                    activeChessman.Remove(c.gameObject);
                    
                }
            }
            EnPassantMove[0] = -1;
            EnPassantMove[1] = -1;
            if (selectedChessman.GetType() == typeof(Pawn))
            {
                if (y == 7)
                {
                    activeChessman.Remove(selectedChessman.gameObject);
                    Destroy(selectedChessman.gameObject);
                    SpawnChessman(1, x, y);
                    selectedChessman = Chessmans[x, y];
                }
                else if (y == 0)
                {
                    activeChessman.Remove(selectedChessman.gameObject);
                    Destroy(selectedChessman.gameObject);
                    SpawnChessman(7, x, y);
                    selectedChessman = Chessmans[x, y];
                }

                //White Pawn
                if (selectedChessman.CurrentY == 1 && y == 3)
                {
                    //Possible move of the black Pawn 
                    EnPassantMove[0] = x;
                    EnPassantMove[1] = y - 1;
                }
                //Black Pawn
                else if (selectedChessman.CurrentY == 6 && y == 4)
                {
                    //Possible move of the white Pawn
                    EnPassantMove[0] = x;
                    EnPassantMove[1] = y + 1;
                }
            }

            Chessmans[selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
            //selectedChessman.transform.position = GetTileCenter(x, y);原本代码

            selectedChessman.SetPosition(x, y); // todo: nav mesh agent 逻辑 change current logic to nav mesh agent mode
            // todo: add nav mesh agent to selectedChessman

            UnityEngine.AI.NavMeshAgent agent = selectedChessman.GetComponent<UnityEngine.AI.NavMeshAgent>();
            agent.destination = new Vector3(x + 0.5f, 0, y + 0.5f);

            Animator animator = selectedChessman.GetComponent<Animator>();
            animator.SetBool("walking", true);




            Chessmans[x, y] = selectedChessman;


            isWhiteTurn = !isWhiteTurn;

        }

        BoardHighlights.Instance.HideHighlights();
        selectedChessman = null;//Select next Chessman

    }
}
