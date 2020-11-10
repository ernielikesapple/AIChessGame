using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement 
{
    public Vector2 firstPosition = Vector2.zero; // 当前要走的棋子位置坐标
    public Vector2 secondPosition = Vector2.zero; // 即将要被吃掉的棋子坐标
    public Chessman pieceMoved = null;   // 当前要走的棋子类名
    public Chessman pieceKilled = null; //即将要被吃掉的棋子类名
    public int score = -100000000;
}
