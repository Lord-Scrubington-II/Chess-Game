using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    GameObject parentPiece = null; // the chesspiece that created it.
    Vector2Int boardPos; //board coords
    internal bool attackSquare = false;
    private SpriteRenderer renderer;

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

    public void OnMouseUp()
    {
        if (attackSquare)
        {
            GameObject targetPiece = Game.PieceAtPosition(boardPos.x, boardPos.y);

            if(targetPiece != null) Destroy(targetPiece);
        }
        Chessman parentChessman = ParentPiece.GetComponent<Chessman>();
        Game.SetSquareEmpty(parentChessman.XBoard, parentChessman.YBoard);
        parentChessman.SetBoardPos(boardPos);
        parentChessman.HasMoved = true;

        Game.AddPieceToMatrix(ParentPiece);

        //parentChessman.DestroyMovePlate();
    }

    public void SetCoords(Vector2Int coords)
    {
        boardPos = coords;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
