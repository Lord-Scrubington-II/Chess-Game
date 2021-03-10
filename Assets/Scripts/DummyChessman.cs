using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Dummy Chessman are stored in a high-performance copy of the 2-D Board array. 
/// This will permit memory-efficient move generation for implementing chess-playing algorithms.
/// </summary>
public class DummyChessman : IComputableChessman
{
    private Chessman.Colours colour;
    private Chessman.Types type;
    private Vector2Int boardPos;
    private bool hasMoved;
    public int File { get => boardPos.x; set => boardPos.x = value; }
    public int Rank { get => boardPos.y; set => boardPos.y = value; }
    public bool HasMoved { get => hasMoved; set => hasMoved = value; }


    public DummyChessman(Chessman.Colours theColour, Chessman.Types theType, Vector2Int coords, bool moved)
    {
        colour = theColour;
        type = theType;
        boardPos = coords;
        hasMoved = moved;
    }

    public Chessman.Colours Colour { 
        get => colour; 
        private set => colour = value; 
    }

    public Chessman.Types Type { 
        get => type; 
        private set => type = value; 
    }

    public Vector2Int BoardCoords { 
        get => boardPos; 
        set => boardPos = value; 
    }

    /// <summary>
    /// This method generates a list of this piece's currently available moves.
    /// </summary>
    /// <param name="boardMatrix">A dummy copy of the board matrix.</param>
    /// <returns>A list of my available moves.</returns>
    public List<Move> GenerateMoves(IComputableChessman[,] boardMatrix)
    {
        List<Move> moves = new List<Move>();

        switch (this.Type)
        {
            case (Chessman.Types.Knight):
                moves = KnightMoves(boardMatrix);
                break;
            case (Chessman.Types.Pawn):
                if (this.Colour == Chessman.Colours.Black)
                {
                    moves = PawnMoves(File, Rank - 1, boardMatrix);
                    break;
                }
                else
                {
                    moves = PawnMoves(File, Rank + 1, boardMatrix);
                    break;
                }
            case (Chessman.Types.Bishop):
                moves = BishopMoves(boardMatrix);
                break;
            case (Chessman.Types.Rook):
                moves = RookMoves(boardMatrix);
                break;
            case (Chessman.Types.Queen):
                moves.AddRange(RookMoves(boardMatrix));
                moves.AddRange(BishopMoves(boardMatrix));
                break;
            case (Chessman.Types.King):
                moves = KingMoves(boardMatrix);
                break;
        }
        return moves;
    }

    private List<Move> KingMoves(in IComputableChessman[,] boardMatrix)
    {
        int targetX;
        int targetY;
        List<Move> myMoves = new List<Move>();

        //for all squares around the king...
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                //except for the king himself...
                if (i != 0 || j != 0)
                {
                    //generate the corresponding move.
                    targetX = File + i;
                    targetY = Rank + j;
                    IndexMove(targetX, targetY, ref myMoves, boardMatrix, false);
                }
            }
        }

        KingsCastleMoves(ref myMoves, boardMatrix);

        return myMoves;
    }

    private void KingsCastleMoves(ref List<Move> myMoves, in IComputableChessman[,] boardMatrix)
    {
        int kingsFile = this.File;
        int kingsRank = this.Rank;
        int x;
        int y;

        //if the king has not moved...
        if (!this.hasMoved)
        {
            //for each of the two directions along the king's rank...
            int step = 1;
            for (int i = 0; i < 2; i++)
            {
                x = kingsFile + step;
                y = kingsRank;

                //search for another piece.
                while (Chess.PositionIsValid(x, y) && boardMatrix[x, y] == null)
                {
                    x += step;
                }

                //if the piece exists, has an empty spot to move to... 
                if (Chess.PositionIsValid(x, y)
                    && boardMatrix[x, y] != null
                    && Chess.PositionIsValid(x - step, y)
                    && boardMatrix[x - step, y] == null)
                {
                    //and is a rook of the same colour that has also not moved...
                    if (boardMatrix[x, y].Type == Chessman.Types.Rook
                        && boardMatrix[x, y].Colour == Colour
                        && !boardMatrix[x, y].HasMoved)
                    {
                        //then permit the king to castle.
                        IndexMove(x, y, ref myMoves, boardMatrix, true);
                    }
                }
                step *= -1;
            }
        }
    }

    private List<Move> RookMoves(in IComputableChessman[,] boardMatrix)
    {
        List<Move> myMoves = new List<Move>();

        myMoves.AddRange(LineMoves(0, 1, boardMatrix));
        myMoves.AddRange(LineMoves(0, -1, boardMatrix));
        myMoves.AddRange(LineMoves(1, 0, boardMatrix));
        myMoves.AddRange(LineMoves(-1, 0, boardMatrix));

        return myMoves;
    }

    private List<Move> BishopMoves(in IComputableChessman[,] boardMatrix)
    {
        List<Move> myMoves = new List<Move>();

        myMoves.AddRange(LineMoves(1, 1, boardMatrix));
        myMoves.AddRange(LineMoves(1, -1, boardMatrix));
        myMoves.AddRange(LineMoves(-1, 1, boardMatrix));
        myMoves.AddRange(LineMoves(-1, -1, boardMatrix));

        return myMoves;
    }

    private List<Move> LineMoves(int xIncr, int yIncr, in IComputableChessman[,] boardMatrix)
    {
        //start a square away
        int x = File + xIncr;
        int y = Rank + yIncr;
        List<Move> myMoves = new List<Move>();

        //generate moves in the specified direction until there's a piece
        //or we reach end of board
        while (Chess.PositionIsValid(x, y) && boardMatrix[x, y] == null)
        {
            IndexMove(x, y, ref myMoves, boardMatrix, false);
            x += xIncr;
            y += yIncr;
        }

        //at the end of the attack path, if the position is valid there must be a piece.
        if (Chess.PositionIsValid(x, y) && (boardMatrix[x, y].Colour != this.Colour))
        {
            IndexMove(x, y, ref myMoves, boardMatrix, false);
        }

        return myMoves;
    }

    private List<Move> PawnMoves(int x, int y, in IComputableChessman[,] boardMatrix)
    {
        //for handling first move case.
        int startExtraMove = Colour == Chessman.Colours.Black ? -1 : 1;
        List<Move> myMoves = new List<Move>();

        //pawns only move forward relative to boardside,
        //so this function expects that the correct arguments are given.
        if (Chess.PositionIsValid(x, y))
        {
            //empty forward means move
            if (boardMatrix[x, y] == null)
            {
                IndexMove(x, y, ref myMoves, boardMatrix, false);
            }

            //handle double push case
            int bonusY = y + startExtraMove;
            if (Chess.PositionIsValid(x, bonusY)
                && boardMatrix[x, bonusY] == null
                && boardMatrix[x, y] == null
                && !HasMoved)
            {
                IndexMove(x, bonusY, ref myMoves, boardMatrix, false);
            }

            //capture case:

            int directionMod = 1;

            //check forward squares twice; once for right and once for left
            for (int i = 0; i < 2; i++)
            {
                int realX = x + directionMod;
                if (Chess.PositionIsValid(realX, y))
                {
                    IComputableChessman target = boardMatrix[realX, y];

                    //if the spot is valid, has a piece, and contains an enemy, place attack plate
                    if ((target != null) && (target.Colour != Colour))
                    {
                        IndexMove(x, y, ref myMoves, boardMatrix, false);
                    }
                }
                //switch to left
                directionMod = directionMod * -1;
            }
        }

        return myMoves;
    }

    private List<Move> KnightMoves(in IComputableChessman[,] boardMatrix)
    {
        int xOffset = 2;
        int yOffset = 1;
        int targetX;
        int targetY;
        List<Move> myMoves = new List<Move>();

        //I stand by my assertion that this is better than copy pasting the method call 8 times.

        for (int i = 0; i < 2; i++) //for all of the correct squares
        {
            for (int j = 0; j < 2; j++) //forming an L-profile
            {
                for (int k = 0; k < 2; k++) //around the knight...
                {
                    //generate the corresponding move.
                    targetX = File + xOffset;
                    targetY = Rank + yOffset;
                    IndexMove(targetX, targetY, ref myMoves, boardMatrix, false);
                    yOffset *= -1;
                }
                xOffset *= -1;
            }
            Chess.BitSwapInts(ref xOffset, ref yOffset);
        }

        return myMoves;
    }

    private void IndexMove(int targetX, int targetY, ref List<Move> myMoves, in IComputableChessman[,] board, bool isCastle)
    {
        if (Chess.PositionIsValid(targetX, targetY))
        {
            //Capture Case
            if (board[targetX, targetY] != null)
            {
                //only index a capture if the target is an opponent.
                if ( board[targetX, targetY].Colour != this.Colour)
                {
                    myMoves.Add(new Move(this, new Vector2Int(targetX, targetY), Chess.ReducedBoardMatrix, isCastle));
                }
            } 
            else
            {
                //this covers normal moves
                myMoves.Add(new Move(this, new Vector2Int(targetX, targetY), Chess.ReducedBoardMatrix, isCastle));
            }
        }
    }

    public string GetName()
    {
        return Colour.ToString() + " " + Type.ToString();
    }

    /// <summary>
    /// ToString() override for Dummy Chessmen.
    /// </summary>
    /// <returns>The Dummy as a string.</returns>
    public override string ToString()
    {
        string sRep = $"{GetName()} at {boardPos}\n";
        return sRep;
    }
}

/// <summary>
/// Computable Chessmen should possess, at minimum:
/// <list type="bullet"> 
///     <item>
///     A Colour
///     </item>
///     <item>
///     A Type
///     </item>
///     <item>
///     A Position
///     </item>
///     <item>
///     A Name
///     </item>
///     <item>
///     Move Generation Capabilities
///     </item>
/// </list>
/// </summary>
public interface IComputableChessman
{
    Chessman.Colours Colour { get; }
    Chessman.Types Type { get; }
    Vector2Int BoardCoords { get; }
    int File { get; }
    int Rank { get; }
    bool HasMoved { get; }

    List<Move> GenerateMoves(IComputableChessman[,] boardMatrix);
    string GetName();

}