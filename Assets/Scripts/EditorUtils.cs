using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EditorUtils : MonoBehaviour
{
    //private Vector3 lastSnapPos = new Vector3(-1, -1, -1);
    public static readonly float gridUnit = 1;
    private static HashSet<GameObject> allChessmen = new HashSet<GameObject>();
    private Chessman chessman;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //add self to hashset
        allChessmen.Add(gameObject);

        //get pointer to the chessman component
        chessman = gameObject.GetComponent<Chessman>();
        
        //snap to grid and select sprite
        SnapToGrid(chessman);
        chessman.SelectSprite();
        chessman.name = chessman.Colour.ToString() + " " + chessman.Type.ToString() + " " + chessman.BoardCoords.ToString();
    }
    

    private void editorCalibrateWorldPos()
    {
        Chessman cm = gameObject.GetComponent<Chessman>();
        cm.CalibrateWorldPos();
    }

    //Snaps chessmen to the grid, which should be aligned with the board. 
    //Updates the transform and backing coordinates of the chessman as well.
    private void SnapToGrid(Chessman chessman)
    {
        Vector3 snapPos;
        Vector2Int newBoardPos;
        if (chessman != null)
        {
            //this arithmetic will snap gameobject transform to grid
            snapPos.x = Mathf.Clamp((Mathf.Floor(gameObject.transform.position.x / gridUnit) * gridUnit), -4.0f, 3.0f);
            snapPos.y = Mathf.Clamp((Mathf.Floor(gameObject.transform.position.y / gridUnit) * gridUnit), -4.0f, 3.0f);

            //store RAW grid position, floored, to the temporary vector2
            newBoardPos = new Vector2Int((int)snapPos.x, (int)snapPos.y);
            //centre the sprite on grid
            snapPos = new Vector3(newBoardPos.x + 0.5f, newBoardPos.y + 0.5f, -1.0f);

            //set the gameobject's transform and write the correct board coords to the chessman's backend coordinates
            gameObject.transform.position = new Vector3(snapPos.x, snapPos.y, -1.0f);
            newBoardPos += new Vector2Int(Game.boardOffset, Game.boardOffset);
            chessman.BoardCoords = newBoardPos;
            
            /*
            //...but only if there isnt anything there already
            if (Game.BoardMatrix[newBoardPos.x, newBoardPos.y] == null)
            {

                chessman.BoardCoords = newBoardPos;
                //set the gameobject's transform
                gameObject.transform.position = new Vector3(snapPos.x, snapPos.y, -1.0f);
                //lastSnapPos = snapPos;
            }
            else
            {
                //snapPos = lastSnapPos;
                gameObject.transform.position = new Vector3(snapPos.x, snapPos.y, -1.0f);
            }
            */
        }
        
    }
}
