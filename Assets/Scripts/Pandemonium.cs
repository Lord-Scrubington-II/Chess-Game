using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pandemonium : MonoBehaviour
{
    private static float PieceProbability = 0.5f;
    [SerializeField] GameObject chessmanPrefab;

    // Start is called before the first frame update
    void Start()
    {
        float rand;
        Chessman.Types cmType;
        Chessman.Colours cmColour;
        Vector2Int newCoords;

        //I'm aN AgEnt oF CHaOs
        for (int file = 0; file < Chess.BoardMatrix.GetLength(0); file++)
        {
            for (int rank = 0; rank < Chess.BoardMatrix.GetLength(1); rank++)
            {
                rand = Random.value;
                if(rand >= PieceProbability)
                {
                    cmType = (Chessman.Types)Random.Range(0, 8);
                    cmColour = (Chessman.Colours)Random.Range(0, 2);
                    newCoords = new Vector2Int(file, rank);
                    CreateChessman(cmType, cmColour, newCoords);
                    //Chess.AddPieceToMatrix();
                }
            }
        }
    }

    private void CreateChessman(Chessman.Types cmType, Chessman.Colours cmColour, Vector2Int newCoords)
    {
        GameObject newPiece = Instantiate(chessmanPrefab, Vector3.zero, Quaternion.identity);
        newPiece.transform.position = new Vector3(0, 0, Chessman.chessmanZ);
        Chessman newChessman = newPiece.GetComponent<Chessman>();
        newChessman.Type = cmType;
        newChessman.Colour = cmColour;
        newChessman.SetBoardPos(newCoords);
        Chess.AddPieceToMatrix(newPiece);
        newChessman.Render();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
