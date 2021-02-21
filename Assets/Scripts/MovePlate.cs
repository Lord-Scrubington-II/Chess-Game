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

    public PlateTypes Type 
    { 
        get => type; 
        set => type = value; 
    }

    public Vector2Int BoardPos 
    { 
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
        if (!Chessman.ControlsFrozen)
        {
            if (Chess.UsingAnims) StartCoroutine(PlayMoveWIthAnim());
            else Chess.Play(MoveData);
        }
    }

    /// <summary>
    /// Function: PlayMoveWIthAnim() (Coroutine)
    /// <para>Moves the parent of this moveplate to the correct location over time. It thereafter invokes the Game.Play(Move m) method.</para>
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayMoveWIthAnim()
    {
        Chessman.ControlsFrozen = true;
        Vector3 piecePos = parentPiece.transform.position;
        Vector3 targetPos = gameObject.transform.position;
        targetPos.z = piecePos.z; //ignore z component for Vector3.Distance() calculations

        // to make knights move in a straight line
        float xSpeedMult = Mathf.Abs(Vector3.Normalize(targetPos - piecePos).x);
        float ySpeedMult = Mathf.Abs(Vector3.Normalize(targetPos - piecePos).y);

        GameObject[] sceneMovePlates = DisableOtherMovePlates();
        while (Vector3.Distance(piecePos, targetPos) > float.Epsilon)
        {
            piecePos.x = Mathf.MoveTowards(piecePos.x, targetPos.x, Time.deltaTime * xSpeedMult * Chessman.pieceMoveSpeed);
            piecePos.y = Mathf.MoveTowards(piecePos.y, targetPos.y, Time.deltaTime * ySpeedMult * Chessman.pieceMoveSpeed);

            parentPiece.transform.position = piecePos;

            yield return null;
        }
        Chessman.ControlsFrozen = false;
        EnableAllObjects(sceneMovePlates);
        Chess.Play(this.moveData);
    }

    /// <summary>
    /// Disable the moveplates in the scene.
    /// </summary>
    /// <returns>The moveplates disabled.</returns>
    public GameObject[] DisableOtherMovePlates()
    {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i = 0; i < movePlates.Length; i++)
        {
            GameObject currentEval = movePlates[i];
            if(currentEval != gameObject) movePlates[i].SetActive(false);
        }

        return movePlates;
    }

    /// <summary>
    /// Re-enable GameObjects to allow for "garbage collection". This particular method is for moveplates.
    /// </summary>
    /// <param name="movePlates">The array of game objects to enable.</param>
    public void EnableAllObjects(GameObject[] movePlates)
    {
        for (int i = 0; i < movePlates.Length; i++)
        {
            GameObject currentEval = movePlates[i];
            if (currentEval != gameObject) movePlates[i].SetActive(true);
        }
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

    /// <summary>
    /// Legacy method. Was used for playing moves before the backend was restructured.
    /// </summary>
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
            int stepToCastleTarget = Chess.PieceAtPosition(BoardPos.x - 1, BoardPos.y) == null ? 1 : -1;
            int stepToNewRookPos = stepToCastleTarget * -1;

            //find the rook to castle with. The nonempty square adjacent to the king contains the rook.
            GameObject castleTarget = Chess.PieceAtPosition(BoardPos.x + stepToCastleTarget, BoardPos.y);
            //castleTarget = castleTarget == null ? Game.PieceAtPosition(boardPos.x + stepToCastleTarget, boardPos.y) : castleTarget;

            //move the rook to the other side of the king
            Vector2Int castleBoardPos = new Vector2Int(BoardPos.x + stepToNewRookPos, BoardPos.y);
            Chessman castleChessman = castleTarget.GetComponent<Chessman>();

            Chess.SetSquareEmpty(castleChessman.File, castleChessman.Rank);
            castleChessman.SetBoardPos(castleBoardPos);
            castleChessman.HasMoved = true;
            Chess.AddPieceToMatrix(castleTarget);

        }

        else
        {

            if (Type == PlateTypes.Attack)
            {
                GameObject targetPiece = Chess.PieceAtPosition(BoardPos.x, BoardPos.y);
                Chessman targetChessman = targetPiece.GetComponent<Chessman>();

                if (targetPiece != null)
                {
                    Chess.UnIndexChessman(targetPiece);
                    Chess.SetSquareEmpty(BoardPos.x, BoardPos.y);

                    if (targetChessman.Type == Chessman.Types.King)
                    {
                        Chessman.Colours winner = targetChessman.Colour == Chessman.Colours.White ? Chessman.Colours.Black : Chessman.Colours.White;
                        Chess.WonGame(winner);
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


        Chess.NextTurn();
        Chess.DestroyMovePlates();
    }

    private void MoveParentToMyLocation()
    {
        Chessman parentChessman = ParentPiece.GetComponent<Chessman>();
        Chess.SetSquareEmpty(parentChessman.File, parentChessman.Rank);
        parentChessman.SetBoardPos(BoardPos);
        parentChessman.HasMoved = true;
        Chess.AddPieceToMatrix(ParentPiece);
    }

}
