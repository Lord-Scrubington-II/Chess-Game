using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessman : MonoBehaviour
{

    public GameObject controller;//might use static
    public GameObject movePlate;

    //position
    private Vector2Int boardPos;
    private static readonly Vector3 defaultScale = new Vector3(4.4f, 4.4f, 4.4f);
    public static readonly float masterGridOffset = -3.5f;

    private bool blackTurn;

    //sprite references
    [SerializeField] private Sprite blackKing, blackQueen, blackBishop, blackKnight, blackRook, blackPawn;
    [SerializeField] private Sprite whiteKing, whiteQueen, whiteBishop, whiteKnight, whiteRook, whitePawn;


    //who needs inheritance when you can just  t w o - d i m e n s i o n a l  e n u m
    public enum Colours
    {
        White,
        Black
    }
    public enum Types
    {
        King,
        Queen,
        Bishop,
        Knight,
        Rook,
        Pawn
    }

    [SerializeField] private Colours colour;
    [SerializeField] private Types type;

    
    public int XBoard { get => boardPos.x; set => boardPos.x = value; }
    public int YBoard { get => boardPos.y; set => boardPos.y = value; }
    public Colours Colour { get => colour; set => colour = value; }
    public Types Type { get => type; set => type = value; }
    public Vector2Int BoardCoords { get => boardPos; set => boardPos = value; }


    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.localScale = defaultScale;
        InitBoardPos();

        Game.IndexChessman(gameObject);
        Activate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {
        SetWorldCoords();
        SelectSprite();
    }

    //Selects the sprite of the piece based on a 2-D enumeration
    public void SelectSprite()
    {
        if (this.Colour == Colours.Black)
        {
            //this is incredibly dumb architecturally but it does means that I don't have to use as many scripts.
            switch (this.Type)
            {
                case Types.King:
                    gameObject.GetComponent<SpriteRenderer>().sprite = blackKing;
                    break;
                case Types.Queen:
                    this.GetComponent<SpriteRenderer>().sprite = blackQueen;
                    break;
                case Types.Bishop:
                    this.GetComponent<SpriteRenderer>().sprite = blackBishop;
                    break;
                case Types.Knight:
                    this.GetComponent<SpriteRenderer>().sprite = blackKnight;
                    break;
                case Types.Rook:
                    this.GetComponent<SpriteRenderer>().sprite = blackRook;
                    break;
                case Types.Pawn:
                    this.GetComponent<SpriteRenderer>().sprite = blackPawn;
                    break;
            }
        }
        else
        {
            switch (this.Type)
            {
                case Types.King:
                    this.GetComponent<SpriteRenderer>().sprite = whiteKing;
                    break;
                case Types.Queen:
                    this.GetComponent<SpriteRenderer>().sprite = whiteQueen;
                    break;
                case Types.Bishop:
                    this.GetComponent<SpriteRenderer>().sprite = whiteBishop;
                    break;
                case Types.Knight:
                    this.GetComponent<SpriteRenderer>().sprite = whiteKnight;
                    break;
                case Types.Rook:
                    this.GetComponent<SpriteRenderer>().sprite = whiteRook;
                    break;
                case Types.Pawn:
                    this.GetComponent<SpriteRenderer>().sprite = whitePawn;
                    break;
            }
        }
    }

    //for changing the world transform of a piece
    private void SetWorldCoords()
    {
        float x = XBoard;
        float y = YBoard;

        x += masterGridOffset;
        y += masterGridOffset;

        gameObject.transform.position = new Vector3(x, y, -1.0f);
    }

    //for changing the backing coords of a piece
    public bool SetBoardPos(Vector2Int coords)
    {
        if(coords.x > Game.BoardXYMax || coords.x > Game.BoardXYMin 
            || coords.y > Game.BoardXYMax || coords.y > Game.BoardXYMin)
        {
            return false;
        }

        boardPos.x = coords.x;
        boardPos.y = coords.y;

        Game.IndexChessman(gameObject);

        return true;
    }

    //initializes board position by the approx position of a piece on the board at compile time
    private void InitBoardPos()
    {
        Vector2Int rawPos = new Vector2Int((int)(gameObject.transform.position.x + Game.boardOffset), 
            (int)(gameObject.transform.position.y + Game.boardOffset));

        BoardCoords = rawPos;
    }
}
