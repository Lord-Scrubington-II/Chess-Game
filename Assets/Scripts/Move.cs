using System;
using UnityEngine;
/// <summary>
/// The <c>Move</c> struct wraps the information behind a move into a single object.
/// This can be used for performing game analysis and implementing chess-playing algorithms.
/// This is an object that will be instantiated a lot. It needs to be as high-performance and
/// memory-efficient as possible.
/// </summary>
public struct Move
{
    private DummyChessman movingChessman;
    private DummyChessman targetingChessman;

    private Vector2Int startSquare;
    private Vector2Int targetSquare;
    private bool isCastle;

    public Vector2Int StartSquare { get => startSquare; private set => startSquare = value; }
    public Vector2Int TargetSquare { get => targetSquare; private set => targetSquare = value; }
    public DummyChessman MovingChessman { get => movingChessman; private set => movingChessman = value; }
    public DummyChessman TargetingChessman { get => targetingChessman; private set => targetingChessman = value; }
    public bool IsCastle { get => isCastle; private set => isCastle = value; }


    /// <summary>
    /// Constructs a Move object.
    /// </summary>
    /// <param name="fromSquare">The start square.</param>
    /// <param name="toSquare">The target square.</param>
    /// <param name="board">A copy of the board state that consists of dummy chessmen.</param>
    /// <param name="castle">Is this move a castle?</param>
    public Move(Vector2Int fromSquare, Vector2Int toSquare, DummyChessman[,] board, bool castle)
    {

        startSquare = fromSquare;
        targetSquare = toSquare;
        isCastle = castle;

        movingChessman = board[startSquare.x, startSquare.y];
        targetingChessman = board[targetSquare.x, targetSquare.y];
        //TODO: change target to the castling rook if is a castle.
    }


    /// <summary>
    /// Constructs a Move object.
    /// </summary>
    /// <param name="invoker">The Chessman that is moving.</param>
    /// <param name="toSquare">The target square of the Chessman.</param>
    /// <param name="board">A copy of the board state that consists of dummy chessmen.</param>
    /// <param name="castle">Is this move a castle?</param>
    public Move(Chessman invoker, Vector2Int toSquare, DummyChessman[,] board, bool castle)
    {

        startSquare = invoker.BoardCoords;
        targetSquare = toSquare;
        isCastle = castle;

        movingChessman = board[startSquare.x, startSquare.y];
        targetingChessman = board[targetSquare.x, targetSquare.y];
        //TODO: change target to the castling rook if is a castle.
    }

    /// <summary>
    /// Calculates the resulting board state, which is represented by a 2-D array of dummy chessmen.
    /// </summary>
    /// <returns>The resulting board from playing this move.</returns>
    public DummyChessman[,] GetBoardImage(DummyChessman[,] board)
    {
        //return boardMatrixPreImage;
        throw new NotImplementedException();
    }

    /// <summary>
    /// <c>ToString()</c> override for Moves.
    /// </summary>
    /// <returns>The move represented as a sentence.</returns>
    public override string ToString()
    {
        string sRep = MovingChessman.GetName()
            + " moves from " + Game.BoardXAlias[startSquare.x] + "" + (startSquare.y + 1)
            + " to " + Game.BoardXAlias[targetSquare.x] + "" + (targetSquare.y + 1);

        if (TargetingChessman != null) { sRep += ", capturing the " + TargetingChessman.GetName(); }
        else if (IsCastle) { sRep += ", castling with the " + TargetingChessman.GetName(); }

        sRep += ".\n";
        return sRep;
    }
}
