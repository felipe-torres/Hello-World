using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    SplashScreen, MainMenu, GamePlay, GameOver, Pausa, Tutorial
}

public delegate void OnStateChangeHandler();

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public event OnStateChangeHandler OnStateChange;
    public GameState gameState { get; private set; }

    protected GameManager()
    {
        
    }

    public static GameManager Instance
    {
        get
        {
            if (GameManager.instance == null)
            {
                GameManager.instance = new GameManager();
                DontDestroyOnLoad(GameManager.instance);
            }
            return GameManager.instance;
        }

    }

    public void SetGameState(GameState state)
    {
        this.gameState = state;
        OnStateChange();
    }

    public void OnApplicationQuit()
    {
        GameManager.instance = null;
    }
}
