using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameWrapper : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Game.ResetGame();
    }

    // Update is called once per frame
    void Update()
    {
        
        //this should really be listening for an event that gets broadcasted by a chessman.
        //better yet, dont even do this at all.
        if(Game.GameOver && Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("Actual Chess"); // temp
        }
    }
}
