using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    private GameObject parentPiece = null; // the chesspiece that created it.
    private Vector2Int boardPos; //board coords

    private SpriteRenderer sRenderer;
    public static readonly float movePlateZ = -2.0f; //render these above pieces

    private Move moveData; //the Move associated with this plate
    private PlateTypes type = PlateTypes.Move;

    /// <summary>
    /// Assign or retrieve a pointer to the chess piece that created this moveplate.
    /// </summary>
    public GameObject ParentPiece 
    { 
        get => parentPiece; 
        set => parentPiece = value; 
    }

    /// <summary>
    /// Assign or retrieve a pointer to the Move that this plate represents.
    /// </summary>
    public Move MoveData 
    { 
        get => moveData; 
        set => moveData = value; 
    }

    public PlateTypes Type { 
        get => type; 
        set => type = value; 
    }

    public Vector2Int BoardPos { 
        get => boardPos; 
        private set => boardPos = value; 
    }

    public enum PlateTypes
    {
        Move,
        Attack,
        Castle
    }

    // Start is called before the first frame update
    void Start()
    {
        ChooseColour();
    }

    /// <summary>
    /// Sets the colour of the Moveplate to red if it's an attack plate and green if it's a castle plate, yellow otherwise.
    /// </summary>
    private void ChooseColour()
    {
        sRenderer = gameObject.GetComponent<SpriteRenderer>();

        if (Type == PlateTypes.Attack)
        {
            //change colour to red
            sRenderer.color = new Color(255, 0, 0, 255);
        }
        else if (Type == PlateTypes.Castle)
        {
            //change colour to green
            sRenderer.color = new Color(0, 255, 0, 255);
        } 
        else
        {
            //change colour to yellow
            sRenderer.color = new Color(255, 255, 0, 255);
        }
    }

    //When a moveplate is clicked, initiate the movement and attack sequences.
    public void OnMouseUp()
    {
        //PlayMovePlate();

        //this is dumb, but it works.
        //TODO: broadcast event: piece moved/piece taken, have the static class handle these
        Game.Play(this.moveData);
    }

    /// <summary>
    /// Sets the board coordinates of the moveplate.
    /// </summary>
    /// <param name="coords">The coordinates</param>
    public void SetCoords(Vector2Int coords)
    {
        BoardPos = coords;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PlayMovePlate()
    {
        //TODO: this is very architectually prohibitive. Not to mention that it obfuscates moves from the backend.
        //This entire function needs to be refactored

        if (Type == PlateTypes.Castle)
        {
            //move king to correct spot
            MoveParentToMyLocation();

            //move rook to correct spot

            //find the step direction to the empty square next to the king.
            //recall that this spot is guaranteed to exist if the king can castle
            int stepToCastleTarget = Game.PieceAtPosition(BoardPos.x - 1, BoardPos.y) == null ? 1 : -1;
            int stepToNewRookPos = stepToCastleTarget * -1;

            //find the rook to castle with. The nonempty square adjacent to the king contains the rook.
            GameObject castleTarget = Game.PieceAtPosition(BoardPos.x + stepToCastleTarget, BoardPos.y);
            //castleTarget = castleTarget == null ? Game.PieceAtPosition(boardPos.x + stepToCastleTarget, boardPos.y) : castleTarget;

            //move the rook to the other side of the king
            Vector2Int castleBoardPos = new Vector2Int(BoardPos.x + stepToNewRookPos, BoardPos.y);
            Chessman castleChessman = castleTarget.GetComponent<Chessman>();

            Game.SetSquareEmpty(castleChessman.File, castleChessman.Rank);
            castleChessman.SetBoardPos(castleBoardPos);
            castleChessman.HasMoved = true;
            Game.AddPieceToMatrix(castleTarget);

        }

        else
        {

            if (Type == PlateTypes.Attack)
            {
                GameObject targetPiece = Game.PieceAtPosition(BoardPos.x, BoardPos.y);
                Chessman targetChessman = targetPiece.GetComponent<Chessman>();

                if (targetPiece != null)
                {
                    Game.UnIndexChessman(targetPiece);
                    Game.SetSquareEmpty(BoardPos.x, BoardPos.y);

                    if (targetChessman.Type == Chessman.Types.King)
                    {
                        Chessman.Colours winner = targetChessman.Colour == Chessman.Colours.White ? Chessman.Colours.Black : Chessman.Colours.White;
                        Game.WonGame(winner);
                    }

                    Destroy(targetPiece);
                }
                else
                {
                    throw new System.ObjectDisposedException("A chessman does not exist in the backend when it should.");
                }

            }

            //change the coordinates of the chessman in the backend
            MoveParentToMyLocation();

        }


        Game.NextTurn();
        Game.DestroyMovePlates();
    }

    private void MoveParentToMyLocation()
    {
        Chessman parentChessman = ParentPiece.GetComponent<Chessman>();
        Game.SetSquareEmpty(parentChessman.File, parentChessman.Rank);
        parentChessman.SetBoardPos(BoardPos);
        parentChessman.HasMoved = true;
        Game.AddPieceToMatrix(ParentPiece);
    }

}
