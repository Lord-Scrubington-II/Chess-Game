using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    GameObject parentPiece = null; // the chesspiece that created it.
    Vector2Int boardPos; //board coords
    internal bool attackSquare = false;
    private SpriteRenderer renderer;
    public static readonly float movePlateZ = -2.0f; //render these above pieces

    /// <summary>
    /// Assign or retrieve a pointer to the chess piece that created this moveplate.
    /// </summary>
    public GameObject ParentPiece 
    { 
        get => parentPiece; 
        set => parentPiece = value; 
    }

    // Start is called before the first frame update
    void Start()
    {
        if (attackSquare)
        {
            //change colour to red
            renderer = gameObject.GetComponent<SpriteRenderer>();
            renderer.color = new Color(255, 0, 0, 255);
        }
    }

    //When a moveplate is clicked, initiate the movement and attack sequences.
    public void OnMouseUp()
    {
        if (attackSquare)
        {
            GameObject targetPiece = Game.PieceAtPosition(boardPos.x, boardPos.y);

            if(targetPiece != null)
            {
                Game.UnIndexChessman(targetPiece);
                Game.SetSquareEmpty(boardPos.x, boardPos.y);
                Destroy(targetPiece);
            }
            else
            {
                throw new System.ObjectDisposedException("A chessman existed in backend when it should not.");
            }
           
        }
        Chessman parentChessman = ParentPiece.GetComponent<Chessman>();
        Game.SetSquareEmpty(parentChessman.XBoard, parentChessman.YBoard);
        parentChessman.SetBoardPos(boardPos);
        parentChessman.HasMoved = true;
        Game.AddPieceToMatrix(ParentPiece);

        //TODO: broadcast event: piece moved/piece taken

        DestroyMovePlates();
    }

    /// <summary>
    /// Sets the board coordinates of the moveplate.
    /// </summary>
    /// <param name="coords">The coordinates</param>
    public void SetCoords(Vector2Int coords)
    {
        boardPos = coords;
    }

    /// <summary>
    /// Removes all moveplates in the scene.
    /// </summary>
    private void DestroyMovePlates()
    {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i = 0; i < movePlates.Length; i++)
        {
            Destroy(movePlates[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
