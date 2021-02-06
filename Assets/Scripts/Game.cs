using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Game
{
    //[SerializeField] private static GameObject chessPiece;

    //positions and team for each chessman
    [SerializeField] private static GameObject[,] boardMatrix = new GameObject[8, 8];
    [SerializeField] private static LinkedList<GameObject> playerWhite = new LinkedList<GameObject>();
    [SerializeField] private static LinkedList<GameObject> playerBlack = new LinkedList<GameObject>();


    private static bool blacksTurn = false;
    private static bool gameOver = false;
    public static readonly int BoardXYMax = 7;
    public static readonly int BoardXYMin = 0;
    public static readonly bool DEBUG = true;
    public static readonly int boardOffset = 4;

    static Game()
    {
        /*
        PlayerWhite = new GameObject[]
        {
            Create(Chessman.Colours.White, Chessman.Types.Bishop, 2, 0),
        };
        */
        //RefreshBoard();
    }

    public static GameObject[,] BoardMatrix { get => boardMatrix; private set => boardMatrix = value; }
    public static LinkedList<GameObject> PlayerWhite { get => playerWhite; private set => playerWhite = value; }
    public static LinkedList<GameObject> PlayerBlack { get => playerBlack; private set => playerBlack = value; }
    public static bool BlacksTurn { get => blacksTurn; private set => blacksTurn = value; }

    //legacy method
    internal static void RefreshBoard()
    {
        //clear positions
        boardMatrix = new GameObject[8, 8];


        foreach (GameObject piece in playerWhite)
        {
            if (piece != null)
            {
                //add to board matrix
                Chessman chessman = piece.GetComponent<Chessman>();
                if (BoardMatrix[chessman.XBoard, chessman.YBoard] != null)
                {
                    Console.WriteLine("Board Matrix Collision Detected.");
                }
                BoardMatrix[chessman.XBoard, chessman.YBoard] = piece;
            }
        }

        foreach (GameObject piece in playerBlack)
        {
            if (piece != null)
            {
                //add to board matrix
                Chessman chessman = piece.GetComponent<Chessman>();
                if (BoardMatrix[chessman.XBoard, chessman.YBoard] != null)
                {
                    Console.WriteLine("Board Matrix Collision Detected.");
                }
                BoardMatrix[chessman.XBoard, chessman.YBoard] = piece;
            }
        }

        /*
        if (DEBUG)
        {
            foreach (GameObject g in boardMatrix)
            {
                SpriteRenderer s = g.GetComponent<SpriteRenderer>();
                s.flipY = true;
            }
        }
        */
    }

    //legacy method
    private static GameObject Create(Chessman.Colours colour, Chessman.Types type, int x, int y)
    {
        GameObject newChessman = new GameObject();
        Chessman chessman = newChessman.AddComponent<Chessman>();
        chessman.Colour = colour;
        chessman.Type = type;
        chessman.XBoard = x;
        chessman.YBoard = y;
        //chessman.Activate();

        return newChessman;
    }

    //adds chessman to the linkedlists that comprise player armies
    public static Chessman.Colours IndexChessman(GameObject piece)
    {
        //not very smart, but it prevents duplicates of the same chess piece from existing,
        //and also ensures that new chesspieces get added to the masterlists
        //if (playerBlack.Contains(piece)) Game.PlayerBlack.Remove(piece);
        //if (playerWhite.Contains(piece)) Game.PlayerWhite.Remove(piece);

        Chessman chessman = piece.GetComponent<Chessman>();
        if (chessman.Colour == Chessman.Colours.Black)
        {
            if (!playerBlack.Contains(piece)) playerBlack.AddLast(piece);
        }
        else
        {
            if (!playerBlack.Contains(piece)) playerWhite.AddLast(piece);
        }

        return chessman.Colour;
    }

    //adds chessman to the board matrix.
    //this clobbers existing chessmen in the matrix, so be careful not to have unreferenced pieces.
    public static void AddPieceToMatrix(GameObject newPiece)
    {
        Chessman cm = newPiece.GetComponent<Chessman>();
        BoardMatrix[cm.XBoard, cm.YBoard] = newPiece;
    }

    public static void SetSquareEmpty(int x, int y)
    {
        BoardMatrix[x, y] = null;
    }

    public static GameObject PieceAtPosition(int x, int y)
    {
        return BoardMatrix[x, y];
    }

    public static bool PositionIsValid(int x, int y)
    {
        if (x < 0 || y < 0 || x >= boardMatrix.Length || y >= boardMatrix.Length) return false;
        return true;
    }
}
