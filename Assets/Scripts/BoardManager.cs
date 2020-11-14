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
    private Chessman selectedChessman;

    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;

    private int selectionX = -1;
    private int selectionY = -1;

    public List<GameObject> chessmanPrefabs;
    private List<GameObject> activeChessman = new List<GameObject>();

    //public int[] EnPassantMove { set; get; }

    public bool smartOpponent = false;

    private Quaternion orientation = Quaternion.Euler(0, 0, 0);

    private bool isWhiteTurn = true;
    //
    public AudioSource walkingClip;
    public GameObject chessBoard;
    public GameObject chessPlane;
    public int maxDepth = 2;


    private void Start()
    {
        Instance = this;
        SpawnAllChessmans();

    }


    private void Update()
    {
        DrawChessBoard();
        UpdateSelection();

        if (Input.GetMouseButtonDown(0)) // todo: add bool to let this step wait for ai's move finish then allow click
        {
            if (selectionX >= 0 && selectionY >= 0)
            {
                if (selectedChessman == null)
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
        if (Chessmans[x, y] == null) { // 选中的位置没有棋子
            return;
        }

        if (Chessmans[x, y].isWhite != isWhiteTurn) // Once pick a black piece while it is the white turn so that does not work
        {
            return;
        }
        allowedMoves = Chessmans[x, y].PossibleMove();  // possible moves is a 8*8 2d array initial value false， 重要🌟：since this function is override by the subchild , so it wont return orginal 8*8 false bool matrix , but a meaning one followed the rules

        selectedChessman = Chessmans[x, y];

        BoardHighlights.Instance.HighlightAllowedMoves(allowedMoves);

    }

    public void MoveChessman(int x, int y)  // 棋子落点坐标
    {
        MoveChessEssenceLogic(x, y);
        if (isWhiteTurn == false)
        {
            computerMove();
        }
    }

    public void MoveChessEssenceLogic(int x, int y)
    {

        if (allowedMoves[x, y])
        {
            Chessman c = Chessmans[x, y]; // 落子点
            //
            if (c != null && c.isWhite != isWhiteTurn)
            {
                //Capture a piece

                //If it is the King
                if (c.GetType() == typeof(King))
                {
                    EndGame();
                    //Rerecord the game
                    walkingClip.Stop();
                    return;
                }
                activeChessman.Remove(c.gameObject);
                Destroy(c.gameObject, 2f);
                StartCoroutine(cDie());
                IEnumerator cDie()
                {
                    yield return new WaitForSeconds(0.4f);
                    c.GetComponent<Animator>().SetBool("getHurt", true);
                    AudioSource getHurt = chessPlane.GetComponent<AudioSource>();
                    getHurt.Play();
                    yield return new WaitForSeconds(0.8f);
                    c.GetComponent<Animator>().SetBool("getHurt", false);
                    c.GetComponent<Animator>().SetBool("die", true);
                    AudioSource die = chessBoard.GetComponent<AudioSource>();
                    die.Play();
                }
                
                /*Animator animatorExisted = c.GetComponent<Animator>();
                if (animatorExisted != null)
                {
                    Destroy(animatorExisted);
                }*/
            }

            ////EnPassantMove(The first nove of the black Pawn is two square, then the white Pawn can remove it)
            //if (x == EnPassantMove[0] && y == EnPassantMove[1] && smartOpponentDoingTrials == false && smartOpponent == false)  //出错点！！
            //{
            //    //White turn(black Pawn move 2 squares)
            //    if (isWhiteTurn)
            //    {
            //        c = Chessmans[x, y - 1];
            //        activeChessman.Remove(c.gameObject);
            //        Destroy(c.gameObject,2f);
            //    }
            //    //Black turn(white Pawn move 2 squares)
            //    else
            //    {
            //        c = Chessmans[x, y + 1];
            //        activeChessman.Remove(c.gameObject);
            //        Destroy(c.gameObject，2f);
            //    }
            //}
            //EnPassantMove[0] = -1;
            //EnPassantMove[1] = -1;


            if (selectedChessman.GetType() == typeof(Pawn))
            {
                if (y == 7)  // whit pawn become a queen
                {
                    activeChessman.Remove(selectedChessman.gameObject);
                    Destroy(selectedChessman.gameObject);
                    SpawnChessman(1, x, y);
                    selectedChessman = Chessmans[x, y];
                }
                else if (y == 0) // black pawn become a queen
                {
                    activeChessman.Remove(selectedChessman.gameObject);
                    Destroy(selectedChessman.gameObject);
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
            // todo: nav mesh agent 逻辑 change current logic to nav mesh agent mode
            // todo: add nav mesh agent to selectedChessman
            Debug.Log(selectedChessman.CurrentX);
            NavMeshAgent agent = selectedChessman.GetComponent<NavMeshAgent>();
            agent.SetDestination (new Vector3(x + TILE_OFFSET, 0, y + TILE_OFFSET));
            Animator animator = selectedChessman.GetComponent<Animator>();
           
            animator.SetBool("walking", true);
            walkingClip = GetComponent<AudioSource>();
            walkingClip.Play();
            if (c != null)
            {
                animator.SetBool("attacking", true);
                
            }
            AudioSource attackClip = selectedChessman.GetComponent<AudioSource>();
            StartCoroutine(Stop());
            IEnumerator Stop()
            {
                yield return new WaitForSeconds(1.6f);
                if (c != null)
                {
                    animator.SetBool("attacking", false);
                    attackClip.Play();
                }
                /*if (animator != null)
                {
                    animator.SetBool("walking", false);

                }*/
                animator.SetBool("walking", false);
                walkingClip.Stop();
            }

            //selectedChessman.transform.position = GetTileCenter(x, y);
            selectedChessman.SetPosition(x, y);
            Chessmans[x, y] = selectedChessman;
            isWhiteTurn = false;

        }
        BoardHighlights.Instance.HideHighlights();
        selectedChessman = null;//Select next Chessman
    }


    private void DrawChessBoard()
    {
        Vector3 widthLine = Vector3.right * 8;
        Vector3 heightLine = Vector3.forward * 8;

        for (int i = 0; i <= 8; i++)
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
        if (selectionX >= 0 && selectionY >= 0)
        {
            Debug.DrawLine(Vector3.forward * selectionY + Vector3.right * selectionX,
                Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1));

            Debug.DrawLine(Vector3.forward * (selectionY + 1) + Vector3.right * selectionX,
                Vector3.forward * selectionY + Vector3.right * (selectionX + 1));
        }
    }

    public void SpawnChessman(int index, int x, int y)    // index represent chess piece type
    {
        GameObject go = Instantiate(chessmanPrefabs[index], GetTileCenter(x, y), orientation) as GameObject;
        go.transform.SetParent(transform);
        Chessmans[x, y] = go.GetComponent<Chessman>();
        Chessmans[x, y].SetPosition(x, y);
        activeChessman.Add(go);
    }

    public void SpawnAllChessmans()
    {
        activeChessman = new List<GameObject>();
        Chessmans = new Chessman[8, 8];
        //EnPassantMove = new int[2] { -1, -1 };

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
        for (int i = 0; i < 8; i++)
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
            //Debug.Log(hit.point);
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

    private void computerMove()
    {
        Debug.Log("black piece startmove");
        if (!smartOpponent)
        {
            doRandomMove();   // add choose random move or real ai move type
        }
        else
        {
            doAIMove();
            Debug.Log("2\n");
            printCurrentBoardToConsole();
        }

        isWhiteTurn = true;
        //Black piece turn if white piece has been moved(switch turn)
    }

    private void doRandomMove()
    {
        bool moveFinished = false;
        while (moveFinished != true)
        {
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
                else
                {
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
    public List<Vector2> chooseArandBlackPieceToMove(string movePieceType)
    {  // 从randomMove中抽离出来的代码，为了防止重复
        foreach (GameObject activeChessPiece in activeChessman)
        {
            Chessman cm = activeChessPiece.GetComponent<Chessman>();
            if (cm.isWhite == false) //黑子
            {
                if (cm.GetType().ToString() == movePieceType)
                { // 对应的种类的黑子
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
    private void doAIMove()
    {

        minMaxDealer minMaxDealerForBlackPiece = new minMaxDealer();
        bestMoves bM = minMaxDealerForBlackPiece.minMaxCoreAlgorithm(Chessmans, selectedChessman, maxDepth);

        Debug.Log("board manager 这边bestMove 的信息" + "bestMove name" + bM.bestSelectedPiece.GetType().ToString() + "多说一句移动子行x：" + bM.bestSelectedPiece.CurrentX + "多说一句移动子行Y：" + bM.bestSelectedPiece.CurrentY + "bestMove x===" + bM.bestMoveTo.x + "bestMove Y===" + bM.bestMoveTo.y);
        allowedMoves = bM.bestSelectedPiece.PossibleMove();
        selectedChessman = bM.bestSelectedPiece; // 核心当前ai玩家要走的棋子坐标，


        MoveChessEssenceLogic((int)bM.bestMoveTo.x, (int)bM.bestMoveTo.y); //  主角棋子被ai玩家吃掉的棋子的坐标

        Debug.Log("1\n");
        printCurrentBoardToConsole();

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