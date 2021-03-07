using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Chessman component is to be attached to GameObjects that represent chessmen.
/// </summary>
public class Chessman : MonoBehaviour, IComputableChessman
{
    //ref to the moveplate prefab
    [SerializeField] private GameObject movePlatePrefab;

    //transform information
    private Vector2Int boardPos;
    private static readonly Vector3 DefaultScale = new Vector3(4.4f, 4.4f, 4.4f);
    public static readonly float chessmanZ = -1.0f;
    public static readonly float masterGridOffset = -3.5f;
    public static readonly float pieceMoveSpeed = 16.0f; //this should be a setting
    private bool hasMoved = false;
    private delegate void IndexPossibleMoves(Chessman piece, GameObject[,] boardMatrix);
    private static bool controlsFrozen = false;

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
        Rook,
        Bishop,
        Knight,
        Pawn
    }

    [SerializeField] private Colours colour;
    [SerializeField] private Types type;
    

    public Vector2Int BoardCoords { get => boardPos; set => boardPos = value; }
    public int File { get => boardPos.x; set => boardPos.x = value; }
    public int Rank { get => boardPos.y; set => boardPos.y = value; }
    public Colours Colour { get => colour; set => colour = value; }
    public Types Type { get => type; set => type = value; }
    public bool HasMoved { get => hasMoved; set => hasMoved = value; }
    public static bool ControlsFrozen { get => controlsFrozen; set => controlsFrozen = value; }


    // Start is called before the first frame update
    void Start()
    {
        readyChessman();
    }

    private void readyChessman()
    {
        gameObject.transform.localScale = DefaultScale;
        CalibrateWorldPos();
        InitBoardPosFromWorldCoordinates();
        Chess.IndexChessman(gameObject);
        Chess.AddPieceToMatrix(gameObject);
        ActivateSprite();
        DisableEditorUtilsModule();
    }

    private void DisableEditorUtilsModule()
    {
        EditorUtils editorUtils = gameObject.GetComponent<EditorUtils>();
        if (editorUtils != null)
        {
            editorUtils.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Visually activates the chessman.
    /// </summary>
    public void ActivateSprite()
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
                    gameObject.GetComponent<SpriteRenderer>().sprite = blackQueen;
                    break;
                case Types.Bishop:
                    gameObject.GetComponent<SpriteRenderer>().sprite = blackBishop;
                    break;
                case Types.Knight:
                    gameObject.GetComponent<SpriteRenderer>().sprite = blackKnight;
                    break;
                case Types.Rook:
                    gameObject.GetComponent<SpriteRenderer>().sprite = blackRook;
                    break;
                case Types.Pawn:
                    gameObject.GetComponent<SpriteRenderer>().sprite = blackPawn;
                    break;
            }
        }
        else
        {
            switch (this.Type)
            {
                case Types.King:
                    gameObject.GetComponent<SpriteRenderer>().sprite = whiteKing;
                    break;
                case Types.Queen:
                    gameObject.GetComponent<SpriteRenderer>().sprite = whiteQueen;
                    break;
                case Types.Bishop:
                    gameObject.GetComponent<SpriteRenderer>().sprite = whiteBishop;
                    break;
                case Types.Knight:
                    gameObject.GetComponent<SpriteRenderer>().sprite = whiteKnight;
                    break;
                case Types.Rook:
                    gameObject.GetComponent<SpriteRenderer>().sprite = whiteRook;
                    break;
                case Types.Pawn:
                    gameObject.GetComponent<SpriteRenderer>().sprite = whitePawn;
                    break;
            }
        }
    }


    /// <summary>
    /// Changes the world transform of a piece.
    /// This method is called in <c>SetBoardPos()</c>, so it is unlikely that this method should be called by itself.
    /// </summary>
    private void SetWorldCoords()
    {
        float x = File;
        float y = Rank;

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
        if(!Chess.PositionIsValid(coords.x, coords.y)
            || Chess.PieceAtPosition(coords.x, coords.y) != null)
        {
            throw new IndexOutOfRangeException("Game attempted to insert chessman " + GetName() 
                + " at board coordinates " + coords.ToString()
                + " which contains " + Chess.PieceAtPosition(coords.x, coords.y));
            //return false;
        }

        boardPos.x = coords.x;
        boardPos.y = coords.y;

        Chess.IndexChessman(gameObject);
        SetWorldCoords();

        return true;
    }

    /// <summary>
    /// Initializes board position by the approximate position of a piece on the board at compile time.
    /// </summary>
    private void InitBoardPosFromWorldCoordinates()
    {
        Vector2Int rawPos = new Vector2Int((int)(gameObject.transform.position.x + Chess.boardOffset), 
            (int)(gameObject.transform.position.y + Chess.boardOffset));

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
        if (!ControlsFrozen)
        {
            if (!Chess.GameOver) ShowPossibleMoves();
        }
        //broadcast event: piece clicked
    }

    public IEnumerator TransformPieceTo(Vector2Int targetSquare)
    {
        ControlsFrozen = true;
        Vector3 pos = gameObject.transform.position;
        float targetX = (float)targetSquare.x + masterGridOffset;
        float targetY = (float)targetSquare.y + masterGridOffset;
        Vector3 target = new Vector3(targetX, targetY, pos.z);

        while (Vector3.Distance(pos, target) > float.Epsilon)
        {
            pos.x = Mathf.MoveTowards(pos.x, Time.deltaTime * pieceMoveSpeed, float.Epsilon);
            pos.y = Mathf.MoveTowards(pos.y, Time.deltaTime * pieceMoveSpeed, float.Epsilon);
            yield return null;
        }
        ControlsFrozen = false;
    }

    public IEnumerator TransformPieceTo(Vector3 targetPos)
    {
        ControlsFrozen = true;
        Vector3 pos = gameObject.transform.position;

        while (Vector3.Distance(pos, targetPos) > float.Epsilon)
        {
            pos.x = Mathf.MoveTowards(pos.x, Time.deltaTime * pieceMoveSpeed, float.Epsilon);
            pos.y = Mathf.MoveTowards(pos.y, Time.deltaTime * pieceMoveSpeed, float.Epsilon);
            yield return null;
        }
        ControlsFrozen = false;
    }

    private void ShowPossibleMoves()
    {
        //when a chess piece is clicked, kill all existing moveplates
        //and spawn new ones corresponding to this chess piece
        if (Chess.PlayerTurn == this.Colour)
        {
            Chess.DestroyMovePlates();
            InitializeMovePlates();
        }
    }

    /// <summary>
    /// This deletes all existing moveplates on the board.
    /// </summary>
    public static void DestroyMovePlates()
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
                    PawnMovePlate(File, Rank - 1);
                    break;
                }
                else
                {
                    PawnMovePlate(File, Rank + 1);
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
                KingMovePlates();
                break;
        }
    }

    /// <summary>
    /// Rooks lay LineMovePlates() orthogonally.
    /// </summary>
    private void RookMovePlates()
    {
        LineMovePlates(0, 1);
        LineMovePlates(0, -1);
        LineMovePlates(1, 0);
        LineMovePlates(-1, 0);
    }

    /// <summary>
    /// Bishops lay LineMovePlates() diagonally.
    /// </summary>
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
        int x = File + xIncr;
        int y = Rank + yIncr;

        //generate move plates in the specified direction until there's a piece
        //or we reach end of board
        while(Chess.PositionIsValid(x, y) && Chess.PieceAtPosition(x, y) == null)
        {
            PlaceMovePlate(x, y);
            x += xIncr;
            y += yIncr;
        }

        //at the end of the attack path, if the position is valid there must be a piece.
        //hence, if the piece is an enemy, lay down an attack square
        if(Chess.PositionIsValid(x, y) && (Chess.PieceAtPosition(x, y).GetComponent<Chessman>().Colour != this.Colour))
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
        if(Chess.PositionIsValid(x, y))
        {
            //empty forward means move
            if(Chess.PieceAtPosition(x, y) == null)
            {
                PlaceMovePlate(x, y);
            }

            //handle first move case
            int bonusY = y + startExtraMove;
            if (Chess.PositionIsValid(x, bonusY) 
                && Chess.PieceAtPosition(x, bonusY) == null 
                && Chess.PieceAtPosition(x, y) == null 
                && !HasMoved)
            {
                PlaceMovePlate(x, bonusY);
            }

            //capture case:

            int directionMod = 1;

            //check forward squares twice; once for right and once for left
            for(int i = 0; i < 2; i++)
            {
                int realX = x + directionMod;
                if (Chess.PositionIsValid(realX, y)){
                    GameObject target = Chess.PieceAtPosition(realX, y);
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
    private void KingMovePlates()
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
                    plateX = File + i;
                    plateY = Rank + j;
                    PointMovePlate(plateX, plateY);
                }
            }
        }

        //but also, suppose the king can castle...
        KingCastleMovePlates();
    }

    /// <summary>
    /// The moveplate instantiation function for king's castle.
    /// </summary>
    private void KingCastleMovePlates()
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
                while (Chess.PositionIsValid(x, y) && Chess.PieceAtPosition(x, y) == null)
                {
                    x += step;
                }

                //if the piece exists, has an empty spot to move to... 
                if (Chess.PositionIsValid(x, y) 
                    && Chess.BoardMatrix[x, y] != null
                    && Chess.PositionIsValid(x - step, y)
                    && Chess.BoardMatrix[x - step, y] == null)
                {
                    //and is a rook of the same colour that has also not moved...
                    if (Chess.BoardMatrix[x, y].GetComponent<Chessman>().Type == Types.Rook 
                        && Chess.BoardMatrix[x, y].GetComponent<Chessman>().Colour == Colour
                        && !Chess.BoardMatrix[x, y].GetComponent<Chessman>().HasMoved)
                    {
                        //then permit the king to castle.
                        PlaceCastlePlate(x - step, y);
                    }
                }
                step *= -1;
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
        
        
        for (int i = 0; i < 2; i++) //for all of the correct squares
        {
            for (int j = 0; j < 2; j++) //forming an L-profile
            {
                for (int k = 0; k < 2; k++) //around the knight...
                {
                    //place the correct plate type if possible.
                    plateX = File + xOffset;
                    plateY = Rank + yOffset;
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
        if (Chess.PositionIsValid(x, y))
        {
            GameObject pieceInSquare = Chess.PieceAtPosition(x, y);

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

        //set world transform based on board coordinates
        float worldX = x;
        float worldY = y;
        worldX += masterGridOffset;
        worldY += masterGridOffset;
        Vector3 plateWorldPos = new Vector3(worldX, worldY, MovePlate.movePlateZ);

        //instantiation of move plate, childed to this piece
        GameObject nMovePlate = Instantiate(movePlatePrefab, plateWorldPos, Quaternion.identity);
        //nMovePlate.transform.parent = gameObject.transform;

        //set the board coordinates of the actual moveplate
        MovePlate movePlate = nMovePlate.GetComponent<MovePlate>();
        movePlate.ParentPiece = gameObject;
        movePlate.SetCoords(platePos);

        //attach a Move object to the new move plate
        movePlate.MoveData = new Move(this, platePos, Chess.ReducedBoardMatrix, false);

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
        movePlate.Type = MovePlate.PlateTypes.Attack;
    }

    /// <summary>
    /// Places an castle-type plate at the specified board location.
    /// </summary>
    /// <param name="x">The board x coord.</param>
    /// <param name="y">The board y coord.</param>
    private void PlaceCastlePlate(int x, int y)
    {
        MovePlate movePlate = PlaceMovePlate(x, y);
        movePlate.Type = MovePlate.PlateTypes.Castle;
        movePlate.MoveData = new Move(this, movePlate.BoardPos, Chess.ReducedBoardMatrix, true);
    }

    /// <summary>
    /// Retrieve the name of the piece, as represented by its colour and type.
    /// </summary>
    /// <returns>The name of the piece.</returns>
    public string GetName()
    {
        return Colour.ToString() + " " + Type.ToString();
    }

    /// <summary>
    /// ToString() override for Chessmen.
    /// </summary>
    /// <returns>The Chessman as a string.</returns>
    public override string ToString()
    {
        string sRep = $"{GetName()} at {Chess.BoardXAlias[this.BoardCoords.x]}{this.BoardCoords.y + 1}\n";
        return sRep;
    }

    public List<Move> GenerateMoves(DummyChessman[,] boardMatrix)
    {
        List<Move> moves = new List<Move>();

        switch (this.Type)
        {

            case (Types.Knight):
                //KnightMovePlates();
                break;
            case (Types.Pawn):
                if (this.Colour == Colours.Black)
                {
                    //PawnMovePlate(File, Rank - 1);
                    break;
                }
                else
                {
                    //PawnMovePlate(File, Rank + 1);
                    break;
                }
            case (Types.Bishop):
                //BishopMovePlates();
                break;
            case (Types.Rook):
                //RookMovePlates();
                break;
            case (Types.Queen):
                //RookMovePlates();
                //BishopMovePlates();
                break;
            case (Types.King):
                //KingMovePlates();
                break;
        }
        return moves;
    }

    internal class AIModule
    {

    }
    
}