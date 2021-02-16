using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Game
{
    //the board state and contents of each army
    private static GameObject[,] boardMatrix = new GameObject[8, 8];
    private static DummyChessman[,] reducedBoardMatrix = new DummyChessman[8, 8];
    private static HashSet<GameObject> whiteArmy = new HashSet<GameObject>();
    private static HashSet<GameObject> blackArmy = new HashSet<GameObject>();

    //mutable game information
    private static Chessman.Colours playerTurn = Chessman.Colours.White;
    private static bool gameOver = false;
    private static bool usingAI = false;

    //board information
    public static readonly int BoardXYMax = 7;
    public static readonly int BoardXYMin = 0;
    public static readonly int boardOffset = 4;

    //aliases for useful data values.
    public static readonly char[] BoardXAlias = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' }; //for standard chess coordinate notation
    public static readonly int[] PieceValues = { 990, 90, 50, 30, 30, 10 }; //These should correspond do the correct enum -> integer expansion in Chessman.Types
    public static readonly bool DEBUG = true;
    public static readonly bool ignoreTurns = false;
    

    
    static Game()
    {
        /*
        PlayerWhite = new GameObject[]
        {
            Create(Chessman.Colours.White, Chessman.Types.Bishop, 2, 0),
        };
        */
        //RefreshBoard();
        playerTurn = Chessman.Colours.White;
        GameOver = false;
    }

    public static GameObject[,] BoardMatrix { get => boardMatrix; private set => boardMatrix = value; }
    public static DummyChessman[,] ReducedBoardMatrix { get => reducedBoardMatrix; private set => reducedBoardMatrix = value; }
    public static HashSet<GameObject> WhiteArmy { get => whiteArmy; private set => whiteArmy = value; }
    public static HashSet<GameObject> BlackArmy { get => blackArmy; private set => blackArmy = value; }

    public static Chessman.Colours PlayerTurn { get => playerTurn; set => playerTurn = value; }
    public static bool GameOver { get => gameOver; private set => gameOver = value; }
    public static bool UsingAI { get => usingAI; private set => usingAI = value; }

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

    internal static void NextTurn()
    {
        PlayerTurn = PlayerTurn == Chessman.Colours.Black ? Chessman.Colours.White : Chessman.Colours.Black;
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
    /// Adds chessman to the board matrix. This clobbers existing chessmen in the matrix, so be careful not to have unreferenced pieces in the scene.
    /// </summary>
    /// <param name="newPiece">The piece to be added.</param>
    public static void AddPieceToMatrix(GameObject newPiece)
    {
        Chessman cm = newPiece.GetComponent<Chessman>();
        BoardMatrix[cm.File, cm.Rank] = newPiece;
        ReducedBoardMatrix[cm.File, cm.Rank] = new DummyChessman(cm.Colour, cm.Type);
    }

    /// <summary>
    /// Removes chessmen from the board matrix.
    /// </summary>
    /// <param name="x">Board x of removal target.</param>
    /// <param name="y">Board y of removal target.</param>
    /// <returns>The piece removed from the board.</returns>
    public static GameObject SetSquareEmpty(int x, int y)
    {
        GameObject removed = boardMatrix[x, y];
        BoardMatrix[x, y] = null;
        ReducedBoardMatrix[x, y] = null;

        return removed;
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
    /// Checks if the specified coordinates are on the board.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <returns><c>true</c> if the position is on the board, <c>false</c> otherwise.</returns>
    public static bool PositionIsValid(int x, int y)
    {
        if (x < 0 || y < 0 || x > BoardXYMax || y > BoardXYMax) return false;
        return true;
    }

    /// <summary>
    /// Legacy method.
    /// </summary>
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

    /// <summary>
    /// The menus should call this function on scene load.
    /// </summary>
    internal static void ResetGame()
    {
        //clear positions
        boardMatrix = new GameObject[8, 8];
        reducedBoardMatrix = new DummyChessman[8, 8];

        WhiteArmy = new HashSet<GameObject>();
        BlackArmy = new HashSet<GameObject>();

        playerTurn = Chessman.Colours.White;
        GameOver = false;
    }

    /// <summary>
    /// Legacy Method.
    /// </summary>
    /// <param name="colour"></param>
    /// <param name="type"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private static GameObject Create(Chessman.Colours colour, Chessman.Types type, int x, int y)
    {
        GameObject newChessman = new GameObject();
        Chessman chessman = newChessman.AddComponent<Chessman>();
        chessman.Colour = colour;
        chessman.Type = type;
        chessman.File = x;
        chessman.Rank = y;
        //chessman.Activate();

        return newChessman;
    }

    /// <summary>
    /// The move invocation method. Calling this method will affect the master board state.
    /// </summary>
    /// <param name="move">The Move to be played.</param>
    /// <returns>A copy of the board state as a 2-D array of Dummy Chessmen.</returns>
    public static DummyChessman[,] Play(Move move)
    {
        Vector2Int startSquare = move.StartSquare;
        Vector2Int targetSquare = move.TargetSquare;
        GameObject movingPiece = BoardMatrix[startSquare.x, startSquare.y];
        //Chessman movingChessman = movingPiece.GetComponent<Chessman>();

        if (move.IsCastle)
        {
            //move king to correct spot
            MovePiece(movingPiece, targetSquare);

            //-> move rook to correct spot
            //find the step direction to the empty square next to the king.
            //recall that this spot is guaranteed to exist if the king can castle
            int stepToCastleTarget = PieceAtPosition(targetSquare.x - 1, targetSquare.y) == null ? 1 : -1;
            int stepToNewRookPos = stepToCastleTarget * -1;

            //find the rook to castle with. The nonempty square adjacent to the king contains the rook.
            GameObject castleTarget = PieceAtPosition(targetSquare.x + stepToCastleTarget, targetSquare.y);

            //move the rook to the other side of the king
            Vector2Int castleBoardPos = new Vector2Int(targetSquare.x + stepToNewRookPos, targetSquare.y);
            MovePiece(castleTarget, castleBoardPos);

        }
        else
        {
            GameObject targetPiece = PieceAtPosition(targetSquare.x, targetSquare.y);
            
            //if attacking a piece
            if (targetPiece != null)
            {
                Chessman targetChessman = targetPiece.GetComponent<Chessman>();

                UnIndexChessman(targetPiece);
                SetSquareEmpty(targetSquare.x, targetSquare.y);

                if (targetChessman.Type == Chessman.Types.King)
                {
                    Chessman.Colours winner = targetChessman.Colour == Chessman.Colours.White ? Chessman.Colours.Black : Chessman.Colours.White;
                    WonGame(winner);
                }

                UnityEngine.Object.Destroy(targetPiece);
            }

            //change the coordinates of the chessman in the backend
            MovePiece(movingPiece, targetSquare);

        }
        NextTurn();
        DestroyMovePlates();

        return (DummyChessman[,])(reducedBoardMatrix.Clone());
    }

    /// <summary>
    /// Moves the piece specified from its original position to the target square.
    /// </summary>
    /// <param name="movingPiece">The piece (GameObject) to be moved</param>
    /// <param name="targetSquare">The target square of the piece</param>
    private static void MovePiece(GameObject movingPiece, Vector2Int targetSquare)
    {
        if (movingPiece != null){
            if (PositionIsValid(targetSquare.x, targetSquare.y))
            {
                Chessman movingChessman = movingPiece.GetComponent<Chessman>();
                SetSquareEmpty(movingChessman.File, movingChessman.Rank);
                movingChessman.SetBoardPos(targetSquare);
                movingChessman.HasMoved = true;
                AddPieceToMatrix(movingPiece);
            } 
            else
            {
                throw new IndexOutOfRangeException("Attempted to move Chessman to an index outside of the board.");
            }
        }
        else
        {
            throw new NullReferenceException("Attempted to move a null-valued Chessman.");
        }
    }

    /// <summary>
    /// The theoretical move invocation method. Returns the hypothetical new board state from playing a move.
    /// </summary>
    /// <param name="move">The Move to apply to the board.</param>
    /// <param name="board">The board on which the Move should be applied.</param>
    /// <returns>A copy of the hypothetical board state.</returns>
    public static DummyChessman[,] Play(Move move, DummyChessman[,] board)
    {
        /*
        Vector2Int startSquare = move.StartSquare;
        Vector2Int targetSquare = move.TargetSquare;
        DummyChessman movingPiece = board[startSquare.x, startSquare.y];
        //Chessman movingChessman = movingPiece.GetComponent<Chessman>();

        if (move.IsCastle)
        {
            //move king to correct spot
            board[targetSquare.x, targetSquare.y] = board[startSquare.x, startSquare.y];

            //-> move rook to correct spot
            //find the step direction to the empty square next to the king.
            //recall that this spot is guaranteed to exist if the king can castle
            int stepToCastleTarget = board[targetSquare.x - 1, targetSquare.y] == null ? 1 : -1;
            int stepToNewRookPos = stepToCastleTarget * -1;

            //find the rook to castle with. The nonempty square adjacent to the king contains the rook.
            DummyChessman castleTarget = board[targetSquare.x + stepToCastleTarget, targetSquare.y];

            //move the rook to the other side of the king
            Vector2Int castleBoardPos = new Vector2Int(targetSquare.x + stepToNewRookPos, targetSquare.y);
            MovePiece(castleTarget, castleBoardPos);

        }
        else
        {
            GameObject targetPiece = PieceAtPosition(targetSquare.x, targetSquare.y);

            //if attacking a piece
            if (targetPiece != null)
            {
                Chessman targetChessman = targetPiece.GetComponent<Chessman>();

                UnIndexChessman(targetPiece);
                SetSquareEmpty(targetSquare.x, targetSquare.y);

                if (targetChessman.Type == Chessman.Types.King)
                {
                    Chessman.Colours winner = targetChessman.Colour == Chessman.Colours.White ? Chessman.Colours.Black : Chessman.Colours.White;
                    WonGame(winner);
                }

                UnityEngine.Object.Destroy(targetPiece);
            }

            //change the coordinates of the chessman in the backend
            MovePiece(movingPiece, targetSquare);

        }
        NextTurn();
        DestroyMovePlates();
        */
        return null;
        
    }

    private static void MovePiece(DummyChessman movingPiece, Vector2Int targetSquare, ref DummyChessman[,] board)
    {
        /*
        if (movingPiece != null)
        {
            if (PositionIsValid(targetSquare.x, targetSquare.y))
            {
                board[movingChessman.File, movingChessman.Rank
                movingChessman.SetBoardPos(targetSquare);
                movingChessman.HasMoved = true;
                AddPieceToMatrix(movingPiece);
            }
            else
            {
                throw new IndexOutOfRangeException("Attempted to move Chessman to an index outside of the board.");
            }
        }
        else
        {
            throw new NullReferenceException("Attempted to move a null-valued Chessman.");
        }
        */
    }

    /// <summary>
    /// This deletes all existing moveplates on the board.
    /// </summary>
    public static void DestroyMovePlates()
    {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i = 0; i < movePlates.Length; i++)
        {
            GameObject.Destroy(movePlates[i]);
        }
    }

    public static void WonGame(Chessman.Colours victor)
    {
        gameOver = true;

        //very weak architecture here. should be broadcasting an event.
        Text gameOverText = GameObject.FindGameObjectWithTag("GameOverText").GetComponent<Text>();
        Text returnText = GameObject.FindGameObjectWithTag("ReturnText").GetComponent<Text>();
        gameOverText.enabled = true;
        returnText.enabled = true;

        gameOverText.text = victor.ToString() + " is the Winner";
    }
}