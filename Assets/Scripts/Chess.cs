using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// This static class implements the chess engine behind Bad Chess.
/// </summary>
public static class Chess
{
    //the board state and contents of each army
    private static GameObject[,] boardMatrix = new GameObject[8, 8];
    private static DummyChessman[,] reducedBoardMatrix = new DummyChessman[8, 8];
    private static HashSet<GameObject> whiteArmy = new HashSet<GameObject>();
    private static HashSet<GameObject> blackArmy = new HashSet<GameObject>();

    //mutable game information
    private static Chessman.Colours toMove = Chessman.Colours.White;
    private static bool gameOver = false;

    //static settings
    private static bool usingAI = false;
    private static Chessman.Colours AIColour = Chessman.Colours.Black;
    private static bool allForOne = false; //win condition: capture one king (true), or capture them all (false)?
    private static bool usingAnims = false; //will chessmen slide across the board? this should be a setting

    //board information
    public static readonly int BoardXYMax = 7;
    public static readonly int BoardXYMin = 0;
    public static readonly int boardOffset = 4;

    //aliases for useful data values.
    public static readonly char[] BoardXAlias = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' }; //for standard chess coordinate notation
    public static readonly int[] PieceValues = { 990, 90, 50, 30, 30, 10 }; //These should correspond do the correct enum -> integer expansion in Chessman.Types
    
    //debug options
    public static readonly bool DEBUG = true;
    public static readonly bool ignoreTurns = false;

    internal static AudioSwitch moveSounds;
    
    //static constructor. It gets called on first reference to static class.
    static Chess()
    {
        /*
        PlayerWhite = new GameObject[]
        {
            Create(Chessman.Colours.White, Chessman.Types.Bishop, 2, 0),
        };
        */
        //RefreshBoard();
        moveSounds = GameObject.Find("Board").GetComponent<AudioSwitch>();
        toMove = Chessman.Colours.White;
        GameOver = false;
    }

    public static GameObject[,] BoardMatrix { get => boardMatrix; private set => boardMatrix = value; }
    public static DummyChessman[,] ReducedBoardMatrix { get => reducedBoardMatrix; private set => reducedBoardMatrix = value; }
    public static HashSet<GameObject> WhiteArmy { get => whiteArmy; private set => whiteArmy = value; }
    public static HashSet<GameObject> BlackArmy { get => blackArmy; private set => blackArmy = value; }

    public static Chessman.Colours PlayerToMove { get => toMove; set => toMove = value; }
    public static bool GameOver { get => gameOver; private set => gameOver = value; }
    public static bool UsingAI { get => usingAI; private set => usingAI = value; }
    public static bool UsingAnims { get => usingAnims; set => usingAnims = value; }

    /// <summary>
    /// Swaps the turn state.
    /// </summary>
    internal static void NextTurn()
    {
        PlayerToMove = PlayerToMove == Chessman.Colours.Black ? Chessman.Colours.White : Chessman.Colours.Black;
        if(PlayerToMove == AIColour && ArmyOf(AIColour).Count > 0 && !GameOver)
        {
            Move toPlay = AIModule.SelectMove(reducedBoardMatrix);
            AIModule.AIPlayMove(toPlay);
        }
    }

    public static HashSet<GameObject> ArmyOf(Chessman.Colours colour)
    {
        if(colour == Chessman.Colours.Black)
        {
            return BlackArmy;
        }

        return WhiteArmy;
    }

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
    /// Adds chessman to the board matrix. This clobbers existing chessmen in the matrix, so be careful not to have unreferenced pieces in the scene.
    /// </summary>
    /// <param name="newPiece">The piece to be added.</param>
    public static void AddPieceToMatrix(GameObject newPiece)
    {
        Chessman cm = newPiece.GetComponent<Chessman>();
        BoardMatrix[cm.File, cm.Rank] = newPiece;
        ReducedBoardMatrix[cm.File, cm.Rank] = new DummyChessman(cm.Colour, cm.Type, cm.BoardCoords);
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
    /// The menus should call this function on scene deload.
    /// </summary>
    internal static void ResetGame()
    {
        //clear positions
        boardMatrix = new GameObject[8, 8];
        reducedBoardMatrix = new DummyChessman[8, 8];

        WhiteArmy = new HashSet<GameObject>();
        BlackArmy = new HashSet<GameObject>();

        toMove = Chessman.Colours.White;
        GameOver = false;
        Chessman.ControlsFrozen = false;
        moveSounds = GameObject.Find("Board").GetComponent<AudioSwitch>();
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

        if(movingPiece == null)
        {
            throw new ObjectDisposedException("A chess piece was somehow null-valued at an attempted move origin square.");
        }

        Chessman movingChessman = movingPiece.GetComponent<Chessman>();

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
                CheckWinConditions(targetChessman);

                UnityEngine.Object.Destroy(targetPiece);
            }

            //change the coordinates of the chessman in the backend
            MovePiece(movingPiece, targetSquare);


            //pawn promotion
            if (movingChessman.HasMoved
                && movingChessman.Type == Chessman.Types.Pawn
                && (movingChessman.Rank == BoardXYMax || movingChessman.Rank == BoardXYMin))
            {
                movingChessman.Type = Chessman.Types.Queen;
                movingChessman.SelectSprite();
            }

        }
        NextTurn();
        DestroyMovePlates();

        return (DummyChessman[,])(reducedBoardMatrix.Clone());
    }

    /// <summary>
    /// Gives the value of a piece by dereferencing the piece valeus array via enum-integer aliasing.
    /// </summary>
    /// <param name="chessman">The chessman to evaluate.</param>
    /// <returns>The value of the chessman.</returns>
    public static int PieceValueOf(IComputableChessman chessman)
    {
        return Chess.PieceValues[(int)chessman.Colour];
    }

    /// <summary>
    /// Invokes the <c>WonGame()</c> method if the win conditions are detected.
    /// </summary>
    /// <param name="justCaptured">The piece just captured.</param>
    private static void CheckWinConditions(Chessman justCaptured)
    {
        if (allForOne)
        {
            if (justCaptured.Type == Chessman.Types.King)
            {
                Chessman.Colours winner = justCaptured.Colour == Chessman.Colours.White ? Chessman.Colours.Black : Chessman.Colours.White;
                WonGame(winner);
            }
        } 
        else
        {
            bool hasAKing = false;

            //check the number of kings in the opponent's hash set
            if(toMove == Chessman.Colours.Black)
            {
                foreach(var piece in WhiteArmy)
                {
                    Chessman chessmanCandidate = piece.GetComponent<Chessman>();
                    if (chessmanCandidate.Type == Chessman.Types.King) hasAKing = true;
                }
            }
            else
            {
                foreach (var piece in BlackArmy)
                {
                    Chessman chessmanCandidate = piece.GetComponent<Chessman>();
                    if (chessmanCandidate.Type == Chessman.Types.King) hasAKing = true;
                }
            }

            if (!hasAKing)
            {
                WonGame(toMove);
            }
        }
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

                moveSounds.PlayChessmanSound(true);
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
        
        Vector2Int startSquare = move.StartSquare;
        Vector2Int targetSquare = move.TargetSquare;
        DummyChessman movingPiece = board[startSquare.x, startSquare.y];

        if(movingPiece == null)
        {
            throw new InvalidOperationException("Theoretical move invocation method attempted to apply an invalid move.");
        }

        if (move.IsCastle)
        {
            //move king to correct spot
            board[targetSquare.x, targetSquare.y] = movingPiece;

            //-> move rook to correct spot
            //find the step direction to the empty square next to the king.
            //recall that this spot is guaranteed to exist if the king can castle
            int stepToCastleTarget = board[targetSquare.x - 1, targetSquare.y] == null ? 1 : -1;
            int stepToNewRookPos = stepToCastleTarget * -1;

            //find the rook to castle with. The nonempty square adjacent to the king contains the rook.
            DummyChessman castleTarget = board[targetSquare.x + stepToCastleTarget, targetSquare.y];

            //move the rook to the other side of the king
            Vector2Int castleBoardPos = new Vector2Int(targetSquare.x + stepToNewRookPos, targetSquare.y);
            MovePiece(castleTarget, castleBoardPos, ref board);

        }
        else
        {
            DummyChessman targetChessman = board[targetSquare.x, targetSquare.y];

            //if attacking a piece
            if (targetChessman != null)
            {
                board[targetSquare.x, targetSquare.y] = null;
            }

            //change the coordinates of the chessman in the backend
            MovePiece(movingPiece, targetSquare, ref board);

        }
        
        return board;
        
    }

    /// <summary>
    /// Moves a dummy chessman across a reduced board matrix.
    /// </summary>
    /// <param name="movingChessman">The dummy chessman to be moved.</param>
    /// <param name="targetSquare">The target square on the reduced board.</param>
    /// <param name="board">A reduced board matrix, passed by reference.</param>
    private static void MovePiece(DummyChessman movingChessman, Vector2Int targetSquare, ref DummyChessman[,] board)
    {
        
        if (movingChessman != null)
        {
            if (PositionIsValid(targetSquare.x, targetSquare.y))
            {
                board[movingChessman.File, movingChessman.Rank] = null;
                movingChessman.BoardCoords = targetSquare;
                board[movingChessman.File, movingChessman.Rank] = movingChessman;
                movingChessman.HasMoved = true;
            }
            else
            {
                throw new IndexOutOfRangeException("Theoretical move invocation method attempted to move Dummy Chessman to an index outside of the board.");
            }
        }
        else
        {
            throw new NullReferenceException("Theoretical move invocation method attempted to move a null-valued Dummy Chessman.");
        }
        
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

    /// <summary>
    /// Calculates a masterlist of all the possible moves for the current board state.
    /// </summary>
    /// <param name="toMove">Player to move.</param>
    /// <returns>All current possible moves.</returns>
    public static List<Move> IndexPossibleMoves(Chessman.Colours toMove)
    {
        List<Move> allMoves = new List<Move>();

        foreach(IComputableChessman chessman in ReducedBoardMatrix)
        {
            if(chessman != null && chessman.Colour == toMove) allMoves.AddRange(chessman.GenerateMoves(ReducedBoardMatrix));
        }

        return allMoves;
    }

    /// <summary>
    /// Calculates a masterlist of all the possible moves for a given board state.
    /// </summary>
    /// <param name="board">The board state, expressed as a 2-D Matrix of Dummy Chessmen.</param>
    /// <param name="toMove">Player to move.</param>
    /// <returns>All possible moves on the given board.</returns>
    public static List<Move> IndexPossibleMoves(in IComputableChessman[,] board, Chessman.Colours toMove)
    {
        List<Move> allMoves = new List<Move>();

        foreach (IComputableChessman chessman in board)
        {
            if (chessman != null && chessman.Colour == toMove) allMoves.AddRange(chessman.GenerateMoves(board));
        }

        return allMoves;
    }

    /// <summary>
    /// This function passes two signed ints by reference and swaps their values.
    /// </summary>
    /// <param name="i">The first integer.</param>
    /// <param name="j">The second integer.</param>
    public static void BitSwapInts(ref int i, ref int j)
    {
        i ^= j;
        j ^= i;
        i ^= j;
    }

    /// <summary>
    /// The board evaluation function.
    /// </summary>
    /// <param name="board">The board state to evaluate.</param>
    /// <param name="BlackToMove">Is it black to move?</param>
    /// <returns></returns>
    public static int Evaluate(in DummyChessman[,] board, bool BlackToMove)
    {
        int whiteMaterial = 0;
        int blackMaterial = 0;
        int playerMult = BlackToMove ? -1 : 1;

        foreach (DummyChessman chessman in board)
        {
            if(chessman != null)
            {
                if(chessman.Colour == Chessman.Colours.White)
                {
                    whiteMaterial += PieceValueOf(chessman);
                } else
                {
                    blackMaterial += PieceValueOf(chessman);
                }
            }
        }

        int boardEvaluation = whiteMaterial + blackMaterial;

        return boardEvaluation * playerMult;
    }

    public static void WonGame(Chessman.Colours victor)
    {
        gameOver = true;
        Chessman.ControlsFrozen = true;
        Text gameOverText;
        Text returnText;
        //very weak architecture here. should be broadcasting an event.
        GameObject GOTxt = GameObject.FindGameObjectWithTag("GameOverText");
        GameObject RETxt = GameObject.FindGameObjectWithTag("ReturnText");

        if (GOTxt != null && RETxt != null)
        {
            gameOverText = GOTxt.GetComponent<Text>();
            returnText = RETxt.GetComponent<Text>();

            gameOverText.enabled = true;
            returnText.enabled = true;

            gameOverText.text = victor.ToString() + " is the Winner";
        }
    }

    public static void MainMenuBack()
    {
        ResetGame();
        SceneManager.LoadScene(0);
    }


    internal static class AIModule
    {
        public delegate Move SelectMoveDelegate(IComputableChessman[,] board);
        private static Chessman.Colours aiColour;
        public static SelectMoveDelegate SelectMove;

        public static Chessman.Colours AIColour { get => aiColour; set => aiColour = value; }

        static AIModule()
        {
            AIColour = Chess.AIColour;

            //TODO: multiple types of move selection.
            SelectMove = SelectRandomMove;
        }

        public static Move SelectRandomMove()
        {
            List<Move> allMoves = Chess.IndexPossibleMoves(AIColour);
            return allMoves[UnityEngine.Random.Range(0, allMoves.Count)];
        }

        public static Move SelectRandomMove(IComputableChessman[,] board)
        {
            List<Move> allMoves = Chess.IndexPossibleMoves(board, AIColour);
            Console.WriteLine(allMoves.Count);
            return allMoves[UnityEngine.Random.Range(0, allMoves.Count)];
        }

        public static void AIPlayMove(Move m)
        {
            Chess.Play(m);
        }
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

}