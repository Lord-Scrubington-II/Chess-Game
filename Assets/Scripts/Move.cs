using System;
using UnityEngine;
/// <summary>
/// The <c>Move</c> struct wraps the information behind a move into a single object.
/// This can be used for performing game analysis and implementing chess-playing algorithms.
/// Since this is an object that will be instantiated a lot, 
/// it needs to be as high-performance and memory-efficient as possible.
/// </summary>
public struct Move : IComparable
{
    // TODO: I wonder if Moves should have an enum associated with them that says something about them?
    // e.g. en passant, castling, double pawn push...
    private DummyChessman movingChessman;
    private DummyChessman targetingChessman;

    private Vector2Int startSquare;
    private Vector2Int targetSquare;
    private bool isCastle;
    private int value;

    public static readonly Move Empty = new Move(new Vector2Int(-1, -1), new Vector2Int(-1, -1), null, false);

    public Vector2Int StartSquare { get => startSquare; private set => startSquare = value; }
    public Vector2Int TargetSquare { get => targetSquare; private set => targetSquare = value; }
    public DummyChessman MovingChessman { get => movingChessman; private set => movingChessman = value; }
    public DummyChessman TargetingChessman { get => targetingChessman; private set => targetingChessman = value; }
    public bool IsCastle { get => isCastle; private set => isCastle = value; }
    public int Value { get => value; set => this.value = value; }


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
        value = 0;
        if (board != null)
        {
            movingChessman = board[startSquare.x, startSquare.y];
            if (castle) //set target to rook
            {
                //find the step direction to the empty square next to the king.
                //recall that this spot is guaranteed to exist if the king can castle
                int stepToCastleTarget = board[targetSquare.x - 1, targetSquare.y] == null ? 1 : -1;
                int stepToNewRookPos = stepToCastleTarget * -1;

                //find the rook to castle with. The nonempty square adjacent to the king contains the rook.
                targetingChessman = board[targetSquare.x + stepToCastleTarget, targetSquare.y];
            }
            else
            {
                targetingChessman = board[targetSquare.x, targetSquare.y];
            }
        }
        else
        {
            movingChessman = null;
            targetingChessman = null;
        }
    }


    /// <summary>
    /// Constructs a Move object.
    /// </summary>
    /// <param name="invoker">The Chessman that is moving.</param>
    /// <param name="toSquare">The target square of the Chessman.</param>
    /// <param name="board">A copy of the board state that consists of dummy chessmen.</param>
    /// <param name="castle">Is this move a castle?</param>
    public Move(IComputableChessman invoker, Vector2Int toSquare, DummyChessman[,] board, bool castle)
    {

        startSquare = invoker.BoardCoords;
        targetSquare = toSquare;
        isCastle = castle;
        value = 0;

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

    public bool isEmpty()
    {
        if(this.movingChessman == Empty.movingChessman
            && this.targetingChessman == Empty.targetingChessman
            && this.startSquare == Empty.startSquare
            && this.targetSquare == Empty.targetSquare)
        {
            return true;
        }
        return false;
    }

    public override bool Equals(object m)
    {
        if(m.GetType() != typeof(Move))
        {
            return false;
        }
        if(this.startSquare == ((Move)m).startSquare && this.targetSquare == ((Move)m).targetSquare)
        {
            return true;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    /// <summary>
    /// <c>ToString()</c> override for Moves.
    /// </summary>
    /// <returns>The move represented as a sentence.</returns>
    public override string ToString()
    {
        string sRep = MovingChessman.GetName()
            + " moves from " + Chess.BoardXAlias[startSquare.x] + "" + (startSquare.y + 1)
            + " to " + Chess.BoardXAlias[targetSquare.x] + "" + (targetSquare.y + 1);

        if (TargetingChessman != null) { sRep += ", capturing the " + TargetingChessman.GetName(); }
        else if (IsCastle) { sRep += ", castling with the " + TargetingChessman.GetName(); }

        sRep += ".\n";
        return sRep;
    }

    public int CompareTo(object other)
    {
        if(other.GetType() == typeof(Move))
        {
            return Value.CompareTo(((Move)other).Value);
        }
        else
        {
            throw new InvalidCastException($"Type Move cannot be compared to type {other.GetType().Name}.");
        }
    }
}
