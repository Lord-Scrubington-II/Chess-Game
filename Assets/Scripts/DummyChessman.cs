﻿using System;
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
    private Vector2Int boardCoords;

    public DummyChessman(Chessman.Colours theColour, Chessman.Types theType, Vector2Int coords)
    {
        colour = theColour;
        type = theType;
        boardCoords = coords;
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
        get => boardCoords; 
        set => boardCoords = value; 
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
        string sRep = $"{GetName()} at {boardCoords}\n";
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
    //public Vector2Int BoardCoords { get; }

    List<Move> GenerateMoves(DummyChessman[,] boardMatrix);
    string GetName();

}