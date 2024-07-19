using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public bool GameStarted;//��Ϸ�Ƿ�ʼ

    public bool GameFinished;//��Ϸ�Ƿ����

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
