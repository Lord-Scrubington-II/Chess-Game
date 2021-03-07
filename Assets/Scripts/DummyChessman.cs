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


    public DummyChessman(Chessman.Colours theColour, Chessman.Types theType, Vector2Int coords)
    {
        colour = theColour;
        type = theType;
        boardPos = coords;
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

    public List<Move> GenerateMoves(DummyChessman[,] boardMatrix)
    {
        throw new NotImplementedException();
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
    public int File { get; }
    public int Rank { get; }
    public bool HasMoved { get; }

    List<Move> GenerateMoves(DummyChessman[,] boardMatrix);
    string GetName();

}