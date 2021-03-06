﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private static readonly int titleIndex = 0;
    private static readonly int menuIndex = 1;
    //private static int chessIndex;

    public static int TitleIndex => titleIndex;
    public static int MenuIndex => menuIndex;
    //public static int ChessIndex { get => chessIndex; private set => chessIndex = value; }

    public void SelectLevel(string levelName)
    {
        Chess.ResetGame();
        SceneManager.LoadScene(levelName);
    }

    public void ReturnToTitle()
    {
        //Chess.ResetGame();
        SceneManager.LoadScene(titleIndex);
    }

    public void ReturnToMenu()
    {
        Chess.ResetGame();
        SceneManager.LoadScene(menuIndex);
    }

    public void PlayActualChess()
    {
        Chess.ResetGame();
        SceneManager.LoadScene("Actual Chess");
    }

    // Start is called before the first frame update
    void Start()
    {
        //ChessIndex = SceneManager.sceneCount - 1; 
        //DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
