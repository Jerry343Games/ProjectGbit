using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public bool GameStarted;//游戏是否开始

    public bool GameFinished;//游戏是否结束

    public event Action GameStartedAction;

    public event Action GameFinishedAction;
    private void Start()
    {
        GameStarted = false;

        GameFinished = false;

        StartGame();
    }
    public void StartGame()
    {
        GameStarted = true;

        GameStartedAction.Invoke();
    }


    public void GameOver(bool isBotWin)
    {
        GameFinished = true;

        GameFinishedAction.Invoke();
    }
}
