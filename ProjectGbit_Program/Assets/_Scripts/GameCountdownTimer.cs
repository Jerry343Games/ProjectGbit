using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameCountdownTimer : MonoBehaviour
{
    public int startMinutes; // 起始时间的分钟数
    public TMP_Text countdownText; // 用于显示倒计时的UI文本

    private float timeRemaining;
    private bool timerIsRunning = false;

    void Start()
    {
        
    }

    public void ExcuteCountdown()
    {
        // 将时间转换为秒数
        timeRemaining = startMinutes * 60;
        timerIsRunning = true;
        StartCoroutine(UpdateTimer());
    }
    
    private IEnumerator UpdateTimer()
    {
        while (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                // 每秒减少时间
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                // 时间结束
                timeRemaining = 0;
                timerIsRunning = false;

                GameManager.Instance.GameOver(true);
            }

            yield return null;
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1; // 向上取整

        float minutes = Mathf.FloorToInt(timeToDisplay / 60); // 计算分钟数
        float seconds = Mathf.FloorToInt(timeToDisplay % 60); // 计算秒数

        // 设置倒计时文本为“00:00”格式
        countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
