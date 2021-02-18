using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameWrapper : MonoBehaviour
{
    [SerializeField] GameObject chessmanPrefab;

    // Start is called before the first frame update
    void Awake()
    {
        Chess.ResetGame();
    }

    // Update is called once per frame
    void Update()
    {
        //this should really be listening for an event that gets broadcasted by a chessman,
        //better yet, dont even do this at all.
        if(Chess.GameOver && Input.GetMouseButtonDown(0))
        {
            //SceneManager.LoadScene("Actual Chess"); //TODO implement level loading
            Chess.MainMenuBack();
        }
    }
}
