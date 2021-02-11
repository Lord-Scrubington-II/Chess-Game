using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessman : MonoBehaviour
{
    //ref to the moveplate prefab
    public GameObject movePlatePrefab;

    //transform information
    private Vector2Int boardPos;
    private static readonly Vector3 defaultScale = new Vector3(4.4f, 4.4f, 4.4f);
    public static readonly float chessmanZ = -1.0f;
    public static readonly float masterGridOffset = -3.5f;
    private bool hasMoved = false;

    //declarations of sprite refs.
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

    public Vector2Int BoardCoords { get => boardPos; set => boardPos = value; }
    public int XBoard { get => boardPos.x; set => boardPos.x = value; }
    public int YBoard { get => boardPos.y; set => boardPos.y = value; }
    public Colours Colour { get => colour; set => colour = value; }
    public Types Type { get => type; set => type = value; }
    public bool HasMoved { get => hasMoved; set => hasMoved = value; }


    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.localScale = defaultScale;
        CalibrateWorldPos();
        InitBoardPosFromWorldCoordinates();
        Game.IndexChessman(gameObject);
        Game.AddPieceToMatrix(gameObject);
        Render();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Visually activates the chessman.
    /// </summary>
    public void Render()
    {
        SetWorldCoords();
        SelectSprite();
    }

    /// <summary>
    /// Selects the sprite of the piece based on a 2-D enumeration.
    /// </summary>
    public void SelectSprite()
    {
        if (this.Colour == Colours.Black)
        {
            //this is incredibly dumb architecturally but it does mean that I don't have to use as many scripts.
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
    /// <summary>
    /// Changes the world transform of a piece.
    /// This method is called in <c>SetBoardPos()</c>, so it is unlikely that this method should be called by itself.
    /// </summary>
    private void SetWorldCoords()
    {
        float x = XBoard;
        float y = YBoard;

        x += masterGridOffset;
        y += masterGridOffset;

        gameObject.transform.position = new Vector3(x, y, chessmanZ);
    }

    /// <summary>
    /// <para>Func: SetBoardPos</para>
    /// Changes the backing coordinates of the chessman.
    /// *This method will call IndexChessman and also update the chessman's world coordinates.*
    /// </summary>
    /// <param name="coords"></param>
    /// <returns></returns>
    public bool SetBoardPos(Vector2Int coords)
    {
        if(!Game.PositionIsValid(coords.x, coords.y)
            || Game.BoardMatrix[coords.x, coords.y] != null)
        {
            throw new IndexOutOfRangeException("Game attempted to insert chessman " + getName() + " at board coordinates " + coords.ToString());
            //return false;
        }

        boardPos.x = coords.x;
        boardPos.y = coords.y;

        Game.IndexChessman(gameObject);
        SetWorldCoords();

        return true;
    }

    /// <summary>
    /// Initializes board position by the approimatei position of a piece on the board at compile time.
    /// </summary>
    private void InitBoardPosFromWorldCoordinates()
    {
        Vector2Int rawPos = new Vector2Int((int)(gameObject.transform.position.x + Game.boardOffset), 
            (int)(gameObject.transform.position.y + Game.boardOffset));

        BoardCoords = rawPos;
    }

    /// <summary>
    /// To be called in case pieces are slightly misaligned.
    /// </summary>
    internal void CalibrateWorldPos()
    {
        Vector3 worldTransform = gameObject.transform.position;
        worldTransform.x = Mathf.Floor(worldTransform.x) + 0.5f;
        worldTransform.y = Mathf.Floor(worldTransform.y) + 0.5f;
        worldTransform.z = -1;

        gameObject.transform.position = worldTransform;
    }

    private void OnMouseUp()
    {
        //when a chess piece is clicked, kill all existing moveplates
        //and spawn new ones corresponding to this chess piece
        if(Game.PlayerTurn == this.Colour)
        {
            DestroyMovePlates();
            InitializeMovePlates();
        }

        //broadcast event: piece clicked
    }

    /// <summary>
    /// This deletes all existing moveplates on the board.
    /// </summary>
    private void DestroyMovePlates()
    {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for(int i = 0; i < movePlates.Length; i++)
        {
            Destroy(movePlates[i]);
        }
    }

    /// <summary>
    /// <para>Func: InitializeMovePlates</para>
    /// This will instantiate moveplates appropriate for the type of the chessman.
    /// </summary>
    private void InitializeMovePlates()
    {
        switch (this.Type)
        {

            case (Types.Knight):
                KnightMovePlates();
                break;
            case (Types.Pawn):
                if(this.Colour == Colours.Black)
                {
                    PawnMovePlate(XBoard, YBoard - 1);
                    break;
                }
                else
                {
                    PawnMovePlate(XBoard, YBoard + 1);
                    break;
                }
            case (Types.Bishop):
                BishopMovePlates();
                break;
            case (Types.Rook):
                RookMovePlates();
                break;
            case (Types.Queen):
                RookMovePlates();
                BishopMovePlates();
                break;
            case (Types.King):
                CircleMovePlates();
                break;
        }
    }

    private void RookMovePlates()
    {
        LineMovePlates(0, 1);
        LineMovePlates(0, -1);
        LineMovePlates(1, 0);
        LineMovePlates(-1, 0);
    }

    private void BishopMovePlates()
    {
        int xIncr = 1;
        int yIncr = 1;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                LineMovePlates(xIncr, yIncr);
                xIncr *= -1;
            }
            yIncr *= -1;
        }
        /*
        LineMovePlates(1, 1);
        LineMovePlates(1, -1);
        LineMovePlates(-1, 1);
        LineMovePlates(-1, -1);
        */
    }

    /// <summary>
    /// The moveplate pattern instantiation function for bishops, queens, and rooks.
    /// </summary>
    private void LineMovePlates(int xIncr, int yIncr)
    {
        //start a square away
        int x = XBoard + xIncr;
        int y = YBoard + yIncr;

        //generate move plates in the specified direction until there's a piece
        //or we reach end of board
        while(Game.PositionIsValid(x, y) && Game.PieceAtPosition(x, y) == null)
        {
            PlaceMovePlate(x, y);
            x += xIncr;
            y += yIncr;
        }

        //at the end of the attack path, if the position is valid there must be a piece.
        //hence, if the piece is an enemy, lay down an attack square
        if(Game.PositionIsValid(x, y) && (Game.PieceAtPosition(x, y).GetComponent<Chessman>().Colour != this.Colour))
        {
            PlaceAttackPlate(x, y);
        }
    }

    /// <summary>
    /// The moveplate pattern instantiation function for pawns.
    /// </summary>
    private void PawnMovePlate(int x, int y)
    {
        //for handling first move case.
        int startExtraMove = Colour == Colours.Black ? -1 : 1;

        //pawns only move forward relative to boardside,
        //so this function expects that the correct arguments are given.
        if(Game.PositionIsValid(x, y))
        {
            //empty forward means move
            if(Game.PieceAtPosition(x, y) == null)
            {
                PlaceMovePlate(x, y);
            }

            //handle first move case
            int bonusY = y + startExtraMove;
            if (Game.PositionIsValid(x, bonusY) 
                && Game.PieceAtPosition(x, bonusY) == null 
                && Game.PieceAtPosition(x, y) == null 
                && !HasMoved)
            {
                PlaceMovePlate(x, bonusY);
            }

            //capture case

            int directionMod = 1;

            //check forward squares twice; once for right and once for left
            for(int i = 0; i < 2; i++)
            {
                int realX = x + directionMod;
                if (Game.PositionIsValid(realX, y)){
                    GameObject target = Game.PieceAtPosition(realX, y);
                    //Colours targetColour = target.GetComponent<Chessman>().Colour;
                    
                    //if the spot is valid, has a piece, and contains an enemy, place attack plate
                    if ((target != null) && (target.GetComponent<Chessman>().Colour != Colour))
                    {
                        PlaceAttackPlate(realX, y);
                    }
                }
                //switch to left
                directionMod = directionMod * -1;
            }
        }
    }

    /// <summary>
    /// The moveplate pattern instantiation function for kings.
    /// </summary>
    private void CircleMovePlates()
    {
        int plateX;
        int plateY;
        //for all squares around the king...
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                //except for the king himself...
                if (i != 0 || j != 0) 
                {
                    //place the correct plate type if possible.
                    plateX = XBoard + i;
                    plateY = YBoard + j;
                    PointMovePlate(plateX, plateY);
                }
            }
        }
    }

    /// <summary>
    /// The moveplate pattern instantiation function for knights.
    /// </summary>
    private void KnightMovePlates()
    {
        int xOffset = 2;
        int yOffset = 1;
        int plateX;
        int plateY;

        //I stand by my assertion that this is better than copy pasting the method call 8 times.
        
        //for all of the correct squares around the knight...
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                for (int k = 0; k < 2; k++)
                {
                    //place the correct plate type if possible.
                    plateX = XBoard + xOffset;
                    plateY = YBoard + yOffset;
                    PointMovePlate(plateX, plateY);
                    yOffset *= -1;
                }
                xOffset *= -1;
            }
            BitSwapInts(ref xOffset, ref yOffset);
        }
    }

    /// <summary>
    /// This function wraps validity check and attack type checks for moveplate generation in one function.
    /// Unfortunately, as it is, it only works properly for the king and knight.
    /// </summary>
    /// <param name="x">The board x coord.</param>
    /// <param name="y">The board y coord.</param>
    public void PointMovePlate(int x, int y)
    {
        if (Game.PositionIsValid(x, y))
        {
            GameObject pieceInSquare = Game.PieceAtPosition(x, y);

            if (pieceInSquare == null)
            {
                PlaceMovePlate(x, y);
            }
            else if (pieceInSquare.GetComponent<Chessman>().Colour != this.Colour)
            {
                PlaceAttackPlate(x, y);
            }
        }
    }

    /// <summary>
    /// This function passes two signed ints by reference and swaps their values.
    /// </summary>
    /// <param name="i">The first integer.</param>
    /// <param name="j">The second integer.</param>
    private void BitSwapInts(ref int i, ref int j)
    {
        i ^= j;
        j ^= i;
        i ^= j;
    }

    /// <summary>
    /// Places an move-type plate at the specified board location.
    /// </summary>
    /// <param name="x">The board x coord.</param>
    /// <param name="y">The board y coord.</param>
    private MovePlate PlaceMovePlate(int x, int y)
    {
        Vector2Int platePos = new Vector2Int(x, y);

        //set world transform based on board cooardinates
        float worldX = x;
        float worldY = y;
        worldX += masterGridOffset;
        worldY += masterGridOffset;
        Vector3 plateWorldPos = new Vector3(worldX, worldY, MovePlate.movePlateZ);

        //instantiation of move plate, childed to this piece
        GameObject nMovePlate = Instantiate(movePlatePrefab, plateWorldPos, Quaternion.identity);
        nMovePlate.transform.parent = gameObject.transform;

        //set the board coordinates of the actual moveplate
        MovePlate movePlate = nMovePlate.GetComponent<MovePlate>();
        movePlate.ParentPiece = gameObject;
        movePlate.SetCoords(platePos);

        return movePlate;
    }

    /// <summary>
    /// Places an attack-type plate at the specified board location.
    /// </summary>
    /// <param name="x">The board x coord.</param>
    /// <param name="y">The board y coord.</param>
    private void PlaceAttackPlate(int x, int y)
    {
        MovePlate movePlate = PlaceMovePlate(x, y);
        movePlate.attackSquare = true;
    }

    public string getName()
    {
        return Colour.ToString() + " " + Type.ToString();
    }
}
