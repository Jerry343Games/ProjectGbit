using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class SubmissionPoint : MonoBehaviour
{
    [Header("SetMainUICanvas")]
    public GameObject mainCanvas;
    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        CreatBubble(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        DestoryBubble(other.gameObject);
    }

    /// <summary>
    /// 创建并初始化倒计时气泡
    /// </summary>
    /// <param name="bot"></param>
    private void CreatBubble(GameObject bot)
    {
        BotProperty botProperty = bot.gameObject.GetComponent<BotProperty>();
        if (botProperty!=null)
        {
            if (!botProperty.isAIBot)
            {
                if (bot.GetComponent<PlayerBot>().currentPart == PartType.Empty) return;
            }
            GameObject bubble=Instantiate(Resources.Load<GameObject>("Prefab/UI/UICountdownBubble"),mainCanvas.transform);
            
            bubble.GetComponent<RectTransform>().localScale=Vector3.zero;
            bubble.GetComponent<RectTransform>().DOScale(1, 0.4f);
            
            UICountdownBubble uiCountdownBubble = bubble.GetComponent<UICountdownBubble>();
            uiCountdownBubble.myBot = bot;
            botProperty.muBubble = bubble;
            
            uiCountdownBubble.duration = botProperty.detectionTimeThreshold;
            uiCountdownBubble.ExcuteFillBar();
        }
    }

    /// <summary>
    /// 销毁气泡
    /// </summary>
    /// <param name="bot"></param>
    private void DestoryBubble(GameObject bot)
    {
        if (bot.CompareTag("Player")&&bot.GetComponent<BotProperty>().muBubble)
        {
            Debug.Log("destory");
            BotProperty playerBot = bot.GetComponent<BotProperty>();
            //销毁并清空索引
            playerBot.muBubble.GetComponent<RectTransform>().DOScale(0, 0.4f).OnComplete(() =>
            {
                Destroy(playerBot.muBubble.gameObject);
                playerBot.muBubble = null;
            });
        }
    }
}
