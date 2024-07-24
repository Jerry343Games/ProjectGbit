using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Composites;

[Serializable]
public class PartTask//零件收集任务
{
    public PartType type;
    public int totalAmount;
    public int currentAmount;
    public GameObject taskUI;
    public bool hasFinshed;
}

public class GameManager : Singleton<GameManager>
{
    [Header("游戏基本参数")]
    public bool GameStarted;//游戏是否开始

    public bool GameFinished;//游戏是否结束

    public int GameDuration;//游戏持续时间


    
    public GameObject botWinObj;

    public GameObject botLoseObj;
    public event Action GameStartedAction;

    public event Action<bool> GameFinishedAction;

    [Header("零件收集相关")]
    public List<PartTask> Tasks = new List<PartTask>();

    public int MaxNum;

    public int MinNum;

    private void Start()
    {
        GameStarted = false;

        GameFinished = false;

       // Invoke("StartGame", 2f);
    }
    private void OnEnable()
    {
        GameStartedAction += RandomTask;
    }

    private void OnDisable()
    {
        GameStartedAction -= RandomTask;
    }
    public void StartGame()
    {
        GameStarted = true;

        MusicManager.Instance.PlayBackMusic("bot_crazy");


        FindObjectOfType<GameCountdownTimer>().startMinutes = GameDuration;

        FindObjectOfType<GameCountdownTimer>().ExcuteCountdown();



        GameStartedAction?.Invoke();

    }

    private void Update()
    {


    }


    public void GameOver(bool isBotWin)
    {
        if (GameFinished) return;

        Debug.Log("游戏结束了,结果是机器人" + isBotWin);

        GameFinished = true;

        GameFinishedAction?.Invoke(isBotWin);

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Publish_Scene1")
        {

            StartCoroutine(Next(isBotWin));
        }
    }

    IEnumerator Next(bool isBotWin)
    {
        if (isBotWin)
        {
            botWinObj.SetActive(true);
        }
        else
        {
            botLoseObj.SetActive(true);
        }

        yield return new WaitForSeconds(3f);
        SceneLoader.Instance.LoadScene("AlarmclockScene 1");
        
    }

    public void RandomTask()
    {
        for(int i =0;i<3;i++)
        {
            int randomNum = UnityEngine.Random.Range(MinNum, MaxNum + 1);

            Tasks[i].totalAmount = randomNum;

            Tasks[i].taskUI.GetComponent<UITaskPanel>().Init(randomNum);

        }
    }

    public void AddPartToTask(PartType type)
    {
        PartTask task = Tasks.Find(x => x.type == type);
        task.currentAmount++;

        task.taskUI.GetComponent<UITaskPanel>().AddOne();

        if(task.currentAmount == task.totalAmount)
        {
            task.hasFinshed = true;
        }

        if (CheckIfBotWin())
        {
            GameOver(true);
        }

    }

    private bool CheckIfBotWin()
    {
        foreach(var task in Tasks)
        {
            if(!task.hasFinshed)
            {
                return false;

            }
        }
        return true;
    }


}
