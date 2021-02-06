using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Game
{
    //[SerializeField] private static GameObject chessPiece;

    //positions and team for each chessman
    [SerializeField] private static GameObject[,] boardMatrix = new GameObject[8, 8];
    [SerializeField] private static HashSet<GameObject> whiteArmy = new HashSet<GameObject>();
    [SerializeField] private static HashSet<GameObject> blackArmy = new HashSet<GameObject>();


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
    public static HashSet<GameObject> PlayerWhite { get => whiteArmy; private set => whiteArmy = value; }
    public static HashSet<GameObject> PlayerBlack { get => blackArmy; private set => blackArmy = value; }
    public static bool BlacksTurn { get => blacksTurn; private set => blacksTurn = value; }

    /// <summary>
    /// Adds chessman to the hashsets that comprise player armies.
    /// </summary>
    /// <param name="piece">The piece to be indexed.</param>
    /// <returns>The colour of the chessman added.</returns>
    public static Chessman.Colours IndexChessman(GameObject piece)
    {
        Chessman chessman = piece.GetComponent<Chessman>();
        if (chessman.Colour == Chessman.Colours.Black)
        {
            blackArmy.Add(piece);
        }
        else
        {
            whiteArmy.Add(piece);
        }

        return chessman.Colour;
    }

    /// <summary>
    /// Removes chessman from the hashsets that comprise player armies.
    /// </summary>
    /// <param name="piece">The piece to be removed.</param>
    /// <returns>The colour of the chessman removed.</returns>
    public static Chessman.Colours UnIndexChessman(GameObject piece)
    {

        Chessman chessman = piece.GetComponent<Chessman>();
        if (chessman.Colour == Chessman.Colours.Black)
        {
            blackArmy.Remove(piece);
        }
        else
        {
            whiteArmy.Remove(piece);
        }

        return chessman.Colour;
    }

    /// <summary>
    /// Adds chessman to the board matrix. This clobbers existing chessmen in the matrix, so be careful not to have unreferenced pieces.
    /// </summary>
    /// <param name="newPiece">The piece to be added.</param>
    public static void AddPieceToMatrix(GameObject newPiece)
    {
        Chessman cm = newPiece.GetComponent<Chessman>();
        BoardMatrix[cm.XBoard, cm.YBoard] = newPiece;
    }

    /// <summary>
    /// Removes chessmen from the board matrix.
    /// </summary>
    /// <param name="x">Board x of removal target.</param>
    /// <param name="y">Board y of removal target.</param>
    public static void SetSquareEmpty(int x, int y)
    {
        BoardMatrix[x, y] = null;
    }

    /// <summary>
    /// Returns chessman at the specified board location.
    /// </summary>
    /// <param name="x">>Board x of the query.</param>
    /// <param name="y">>Board y of query.</param>
    /// <returns>The chessman.</returns>
    public static GameObject PieceAtPosition(int x, int y)
    {
        return BoardMatrix[x, y];
    }

    /// <summary>
    /// Checks if the specifiec coordinates are on the board.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <returns><c>true</c> if the position is on the board, <c>false</c> otherwise.</returns>
    public static bool PositionIsValid(int x, int y)
    {
        if (x < 0 || y < 0 || x > BoardXYMax || y > BoardXYMax) return false;
        return true;
    }

    //legacy method
    internal static void RefreshBoard()
    {
        throw new System.NotSupportedException("This method is an unsupported legacy method.");

        /*
        //clear positions
        boardMatrix = new GameObject[8, 8];


        foreach (GameObject piece in whiteArmy)
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

        foreach (GameObject piece in blackArmy)
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

}
