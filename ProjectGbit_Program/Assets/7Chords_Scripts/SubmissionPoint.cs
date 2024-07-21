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
            else
            {
                if (bot.GetComponent<AIBot>().CurrentPart == PartType.Empty) return;
                Debug.Log("1121");
            }
            GameObject bubble=Instantiate(Resources.Load<GameObject>("Prefab/UI/UICountdownBubble"),mainCanvas.transform);
            Debug.Log(bubble);
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
        
        BotProperty botProperty = bot.GetComponent<BotProperty>();
        if (botProperty == null) return;
        if (botProperty.isAIBot)
        {
            if (botProperty != null && botProperty.muBubble != null)
            {

                //BotProperty playerBot = bot.GetComponent<BotProperty>();
                //销毁并清空索引
                botProperty.muBubble.GetComponent<RectTransform>().DOScale(0, 0.4f).OnComplete(() =>
                {
                    
                    Destroy(botProperty.muBubble.gameObject);
                    botProperty.muBubble = null;
                });
            }
        }
        
    }
}
