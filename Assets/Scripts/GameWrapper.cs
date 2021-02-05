using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWrapper : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Game.RefreshBoard();
    }

    // Update is called once per frame
    void Update()
    {
        //this should listen for an event that gets broadcasted by a chessman.
        Game.RefreshBoard();
    }
}
