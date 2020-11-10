using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AI;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { set; get; }
    public bool[,] allowedMoves { set; get; }

    public Chessman[,] Chessmans { set; get; }  //Chessman array and a property
    public Chessman selectedChessman;
   
    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;

    private int selectionX = -1;
    private int selectionY = -1;

    public List<GameObject> chessmanPrefabs;
    public List<GameObject> activeChessman = new List<GameObject>();

    public int[] EnPassantMove { set; get; }

    public bool smartOpponent = false;
    public bool smartOpponentDoingTrials = false;
    AlphaBeta ab = new AlphaBeta();

    /*private Quaternion orientation = Quaternion.Euler(0, 0, 0);*/

    public bool isWhiteTurn = true;

    private void Start()
    {
        Instance = this;
        SpawnAllChessmans();


        smartOpponent = true; // test purpose;
    }

    private void Update()
    {
        DrawChessBoard();
        UpdateSelection();

        if (Input.GetMouseButtonDown(0)) // todo: add bool to let this step wait for ai's move finish then allow click
        {
            if(selectionX >= 0 && selectionY >= 0) 
            {
                if(selectedChessman == null)
                {
                    //select the chessman
                    SelectChessman(selectionX, selectionY);
                }
                else
                {
                    //move the chessman
                    // ⚠️： 此时selectionX， selectionY和上面if里的selectionX， selectionY 是不一样的，此时的是有一个棋子被选中后，下次再点击时候的xy
                    MoveChessman(selectionX, selectionY);
                }
            }
        }
    }

    private void SelectChessman(int x, int y)
    {
        if (Chessmans[x, y] == null) // 选中的位置没有棋子
            return;
        if (Chessmans[x, y].isWhite != isWhiteTurn)// Once pick a black piece while it is the white turn so that does not work
            return;
        allowedMoves = Chessmans[x, y].PossibleMove();  // possible moves is a 8*8 2d array initial value false， 重要🌟：since this function is override by the subchild , so it wont return orginal 8*8 false bool matrix , but a meaning one followed the rules

        selectedChessman = Chessmans[x, y];

        BoardHighlights.Instance.HighlightAllowedMoves(allowedMoves);

    }

    public void MoveChessman(int x,int y)  // 棋子落点坐标
    {
        MoveChessEssenceLogic(x, y);
        computerMove();
    }

    public void MoveChessEssenceLogic(int x, int y) {
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
                Destroy(c.gameObject);
            }
            //EnPassantMove(The first nove of the black Pawn is two square, then the white Pawn can remove it)
            if (x == EnPassantMove[0] && y == EnPassantMove[1])
            {
                //White turn(black Pawn move 2 squares)
                if (isWhiteTurn)
                {
                    c = Chessmans[x, y - 1];
                    activeChessman.Remove(c.gameObject);
                    Destroy(c.gameObject);
                }
                //Black turn(white Pawn move 2 squares)
                else
                {
                    c = Chessmans[x, y + 1];
                    activeChessman.Remove(c.gameObject);
                    Destroy(c.gameObject);
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

            NavMeshAgent agent = selectedChessman.GetComponent<NavMeshAgent>();
            agent.destination = new Vector3(x + 0.5f, 0, y + 0.5f);
            
            Animator animator = selectedChessman.GetComponent<Animator>();
            animator.SetBool("walking", true);
          



            Chessmans[x, y] = selectedChessman;
           

            isWhiteTurn = !isWhiteTurn;
          
        }

        BoardHighlights.Instance.HideHighlights();
        selectedChessman = null;//Select next Chessman

    }

    private void DrawChessBoard()
    {
        Vector3 widthLine = Vector3.right * 8;
        Vector3 heightLine = Vector3.forward * 8;

        for(int i = 0; i <= 8; i++)
        {
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine(start, start + widthLine);
            for (int j = 0; j <= 8; j++)
            {
                start = Vector3.right * j;
                Debug.DrawLine(start, start + heightLine);
            }
        }

        //Draw the selection
        if(selectionX >= 0 && selectionY >= 0)
        {
            Debug.DrawLine(Vector3.forward * selectionY + Vector3.right * selectionX,
                Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1));

            Debug.DrawLine(Vector3.forward * (selectionY + 1) + Vector3.right * selectionX,
                Vector3.forward * selectionY + Vector3.right * (selectionX + 1));
        }
    }
    
    private void SpawnChessman(int index, int x,int y)    // index represent chess piece type
    {
        GameObject go = Instantiate(chessmanPrefabs[index], GetTileCenter(x,y), Quaternion.identity) as GameObject;
        go.transform.SetParent(transform);
        Chessmans[x, y] = go.GetComponent<Chessman>();
        Chessmans[x, y].SetPosition(x, y);
        activeChessman.Add(go);
    }

    private void SpawnAllChessmans()
    {
        activeChessman = new List<GameObject>();
        Chessmans = new Chessman[8, 8];
        EnPassantMove = new int[2] { -1, -1 };

        //Spawn the white team

        //King
        SpawnChessman(0, 3, 0);

        //Queen
        SpawnChessman(1, 4, 0);

        //Rook
        SpawnChessman(2, 0, 0);
        SpawnChessman(2, 7, 0);

        //Bishop
        SpawnChessman(3, 2, 0);
        SpawnChessman(3, 5, 0);

        //Horse
        SpawnChessman(4, 1, 0);
        SpawnChessman(4, 6, 0);

        //Pawns
       for(int i = 0; i < 8; i++)
        {
            SpawnChessman(5, i, 1);
        }

        //Spawn the black team

        //King
        SpawnChessman(6, 3, 7);

        //Queen
        SpawnChessman(7, 4, 7);

        //Rooks
        SpawnChessman(8, 0, 7);
        SpawnChessman(8, 7, 7);

        //Bishops
        SpawnChessman(9, 2, 7);
        SpawnChessman(9, 5, 7);

        //Horses
        SpawnChessman(10, 1, 7);
        SpawnChessman(10, 6, 7);

        //Pawns
        for (int i = 0; i < 8; i++)
        {
            SpawnChessman(11, i, 6);
        }
    }
    private Vector3 GetTileCenter(int x, int y)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;
        return origin;
    }
    private void UpdateSelection()
    {
        if (!Camera.main)
            return;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, LayerMask.GetMask("ChessPlane")))
        {
            /*Debug.Log(hit.point);*/
            Debug.DrawLine(Input.mousePosition, hit.point, Color.red);
            selectionX = (int)hit.point.x;
            selectionY = (int)hit.point.z;
        }
        else
        {
            selectionX = -1;
            selectionY = -1;
        }
    }
    public void EndGame()
    {
        if (isWhiteTurn)
            Debug.Log("White team wins !!!");
        else
            Debug.Log("Black team wins !!!");
        foreach (GameObject go in activeChessman)
            Destroy(go);
        //Rebegin the game and white piece first /who wins who goes first if not write "isWhiteTurn = true;" 
        isWhiteTurn = true;
        BoardHighlights.Instance.HideHighlights();
        SpawnAllChessmans();
    }

    private void computerMove() {

        isWhiteTurn = !isWhiteTurn;

        if (!smartOpponent)
        {
            doRandomMove();   // add choose random move or real ai move type
        }
        else {
            doAIMove();
        }
        isWhiteTurn = !isWhiteTurn;//Black piece turn if white piece has been moved(switch turn)
    }

    private void doRandomMove()
    {
        bool moveFinished = false;
        while (moveFinished != true) {
            int movePieceType = Random.Range(0, 5);
            if (movePieceType == 0)  // MOVE KING
            {
                List<Vector2> possibleMovesGrids = chooseArandBlackPieceToMove("King");

                if (possibleMovesGrids.Count > 0)
                {
                    int randomMove = Random.Range(1, possibleMovesGrids.Count);
                    randomMove = randomMove - 1;
                    MoveChessEssenceLogic((int)possibleMovesGrids[randomMove].x, (int)possibleMovesGrids[randomMove].y);
                    moveFinished = true;
                }
                else {
                    continue;
                }
            }
            else if (movePieceType == 1) // Queen
            {
                List<Vector2> possibleMovesGrids = chooseArandBlackPieceToMove("Queen");
                if (possibleMovesGrids.Count > 0)
                {
                    int randomMove = Random.Range(1, possibleMovesGrids.Count);
                    randomMove = randomMove - 1;
                    MoveChessEssenceLogic((int)possibleMovesGrids[randomMove].x, (int)possibleMovesGrids[randomMove].y);
                    moveFinished = true;
                }
            }
            else if (movePieceType == 2) // Rook
            {
                List<Vector2> possibleMovesGrids = chooseArandBlackPieceToMove("Rook");
                if (possibleMovesGrids.Count > 0)
                {
                    int randomMove = Random.Range(1, possibleMovesGrids.Count);
                    randomMove = randomMove - 1;
                    MoveChessEssenceLogic((int)possibleMovesGrids[randomMove].x, (int)possibleMovesGrids[randomMove].y);
                    moveFinished = true;
                }
                else
                {
                    continue;
                }
            }
            else if (movePieceType == 3) // Bishop
            {
                List<Vector2> possibleMovesGrids = chooseArandBlackPieceToMove("Bishop");
                if (possibleMovesGrids.Count > 0)
                {
                    int randomMove = Random.Range(1, possibleMovesGrids.Count);
                    randomMove = randomMove - 1;
                    MoveChessEssenceLogic((int)possibleMovesGrids[randomMove].x, (int)possibleMovesGrids[randomMove].y);
                    moveFinished = true;
                }
                else
                {
                    continue;
                }
            }
            else if (movePieceType == 4) // Horse
            {
                List<Vector2> possibleMovesGrids = chooseArandBlackPieceToMove("Horse");

                if (possibleMovesGrids.Count > 0)
                {
                    int randomMove = Random.Range(1, possibleMovesGrids.Count);
                    randomMove = randomMove - 1;
                    MoveChessEssenceLogic((int)possibleMovesGrids[randomMove].x, (int)possibleMovesGrids[randomMove].y);
                    moveFinished = true;
                }
                else
                {
                    continue;
                }
            }
            else if (movePieceType == 5) //Pawns
            {
                List<Vector2> possibleMovesGrids = chooseArandBlackPieceToMove("Pawn");

                if (possibleMovesGrids.Count > 0)
                {
                    int randomMove = Random.Range(1, possibleMovesGrids.Count);
                    randomMove = randomMove - 1;
                    MoveChessEssenceLogic((int)possibleMovesGrids[randomMove].x, (int)possibleMovesGrids[randomMove].y);
                    moveFinished = true;
                }
                else
                {
                    continue;
                }
            }
        }
    }
    private List<Vector2> chooseArandBlackPieceToMove(string movePieceType) {  // 从randomMove中抽离出来的代码，为了防止重复
        foreach (GameObject activeChessPiece in activeChessman)
        {
            Chessman cm = activeChessPiece.GetComponent<Chessman>();
            if (cm.isWhite == false) //黑子
            {
                if (cm.GetType().ToString() == movePieceType) { // 对应的种类的黑子
                    int x1 = cm.CurrentX;
                    int y1 = cm.CurrentY;
                    allowedMoves = Chessmans[x1, y1].PossibleMove();  // possible moves is a 8*8 2d array initial value false， 重要🌟：since this function is override by the subchild , so it wont return orginal 8*8 false bool matrix , but a meaning one followed the rules
                    selectedChessman = Chessmans[x1, y1];
                    break;  // 找到一个对应的类型的棋子就不在继续找了
                }
            }
        }

        List<Vector2> possibleMovesGrids = new List<Vector2>(); //电脑黑子可走的位置
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (allowedMoves[i, j])
                {
                    possibleMovesGrids.Add(new Vector2(i, j)); // 查看当前棋子有没有可以走的地方
                }
            }
        }
        return possibleMovesGrids;
    }

    private void doAIMove() {
        // todo: get current state of the board, pass the value into AI class

        // declare 3d variables to store active white black info on the current board

        //Debug.Log("going to move + x" + blackPieceInfoDic[Random.Range(0, 5)].x + "==y==" + blackPieceInfoDic[Random.Range(0, 5)].y);

        //   allowedMoves = Chessmans[x, y].PossibleMove();


        // todo: get the return value for the from the class

        // Move move = ab.GetMove();  // 核心当前ai玩家要走的棋子坐标， 主角棋子被ai玩家吃掉的棋子的坐标

        // todo: add logic to move the piece , similiar to the logic in MoveChessman method
        //_DoAIMove(move);


        minMaxDealer minMaxDealerForBlackPiece = new minMaxDealer();

        minMaxDealerForBlackPiece.minMaxCoreAlgorithm();

        // todo: get the return value from minMaxDealerForBlackPiece let it pass to MoveChessEssenceLogic
        //MoveChessEssenceLogic((int)possibleMovesGrids[randomMove].x, (int)possibleMovesGrids[randomMove].y);

    }




}
