using System;
using System.Collections;
using System.Collections.Generic;
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
        DEstoryBubble(other.gameObject);
    }

    /// <summary>
    /// 倒计时气泡
    /// </summary>
    /// <param name="bot"></param>
    private void CreatBubble(GameObject bot)
    {
        if (bot.name=="PlayerBot1")
        {
            GameObject bubble=Instantiate(Resources.Load<GameObject>("Prefab/UI/UICountdownBubble"),mainCanvas.transform);
            bubble.GetComponent<UICountdownBubble>().myBot = bot;
            bubble.GetComponent<UICountdownBubble>().ExcuteFillBar();
        }
    }

    private void DEstoryBubble(GameObject bot)
    {
        
    }
}
