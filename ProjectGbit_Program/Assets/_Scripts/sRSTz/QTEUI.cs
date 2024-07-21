using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTEUI : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("SetMainUICanvas")]
    public GameObject mainCanvas;
    public float qteTime = 1f;//持续时间
    public bool isCreating = false;
    //public GameObject currentBubble;
    private void Start()
    {
        mainCanvas = GameObject.Find("Canvas");
    }
    /// <summary>
    /// 创建并初始化倒计时气泡
    /// </summary>
    /// <param name="bot"></param>
    public void CreatBubble(GameObject bot)
    {

        BotProperty botProperty = bot.gameObject.GetComponent<BotProperty>();
        if (botProperty != null&&!isCreating)
        {
            isCreating = true;
            GameObject bubble = Instantiate(Resources.Load<GameObject>("Prefab/UI/UIQTEBubble"), mainCanvas.transform);
            Debug.Log(bubble);
            bubble.GetComponent<RectTransform>().localScale = Vector3.zero;
            bubble.GetComponent<RectTransform>().DOScale(1, 0.2f);

            UIQTEbubble uiCountdownBubble = bubble.GetComponent<UIQTEbubble>();
            uiCountdownBubble.myBot = bot;
            botProperty.qteBubble = bubble;
            
            StartCoroutine(DestoryBubble(bot, 1f));
            //uiCountdownBubble.duration = botProperty.detectionTimeThreshold;
            //uiCountdownBubble.ExcuteFillBar();
        }
    }
    /// <summary>
    /// 销毁气泡
    /// </summary>
    /// <param name="bot"></param>
    public IEnumerator DestoryBubble(GameObject bot,float delay)
    {
        yield return new WaitForSeconds(delay);
        //Debug.Log("111");
        BotProperty botProperty = bot.GetComponent<BotProperty>();
        if (botProperty == null) yield break ;
        
            if (botProperty != null && botProperty.qteBubble != null)
            {

            
                //BotProperty playerBot = bot.GetComponent<BotProperty>();
                //销毁并清空索引
                botProperty.qteBubble.GetComponent<RectTransform>().DOScale(0, 0.2f).OnComplete(() =>
                {
                    isCreating = false;
                    Destroy(botProperty.qteBubble.gameObject);
                    botProperty.qteBubble = null;
                });
            }
        

    }
}
