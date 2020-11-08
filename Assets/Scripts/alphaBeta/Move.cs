using UnityEngine;
using System.Collections;

public class Move
{
    public Tile firstPosition = null; // 当前要走的棋子位置坐标
    public Tile secondPosition = null; // 即将要被吃掉的棋子坐标
    public Piece pieceMoved = null;   // 当前要走的棋子类名
    public Piece pieceKilled = null; //即将要被吃掉的棋子类名
    public int score = -100000000;
}