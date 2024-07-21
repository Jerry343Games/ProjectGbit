using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BotGetPartTrigger : MonoBehaviour
{
    private AIBot _aiBot;

    public GameObject mainCanvas;

    public UIPartBubble getPartBubble;

    private bool isBeingSubmit;

    private float submitTimer;

    private float submitDuration;

    private void Awake()
    {
        _aiBot = transform.parent.GetComponent<AIBot>();
    }

    private void Start()
    {
        submitDuration = _aiBot.GetComponent<BotProperty>().detectionTimeThreshold;
        submitTimer = 0;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="SubmissionPoint" && _aiBot.CurrentPart != PartType.Empty)
        {
            CreatBubble(transform.parent.gameObject);
        }

        if (other.gameObject.tag=="Part" && _aiBot.CurrentPart == PartType.Empty)
        {
            Instantiate(Resources.Load<GameObject>("Prefab/Effect/PickupTaskitem"), transform.position, Quaternion.identity);

            GameObject bubble = Instantiate(Resources.Load<GameObject>("Prefab/UI/UIPartBubble"), mainCanvas.transform);
            getPartBubble = bubble.GetComponent<UIPartBubble>();
            bubble.GetComponent<RectTransform>().localScale = Vector3.zero;
            bubble.GetComponent<RectTransform>().DOScale(1, 0.4f);
            PartType type = other.gameObject.transform.parent.GetComponent<Part>().partType;
            getPartBubble.SetInner(type);
            getPartBubble.myBot = _aiBot.gameObject;
            
            _aiBot.GetPart(type);
            //销毁零件
            Destroy(other.gameObject);
        }
    }

    /// <summary>
    /// 创建并初始化倒计时气泡
    /// </summary>
    /// <param name="bot"></param>
    private void CreatBubble(GameObject bot)
    {
        BotProperty botProperty = bot.GetComponent<BotProperty>();
        GameObject bubble = Instantiate(Resources.Load<GameObject>("Prefab/UI/UICountdownBubble"), mainCanvas.transform);
        
        bubble.GetComponent<RectTransform>().localScale = Vector3.zero;
        bubble.GetComponent<RectTransform>().DOScale(1, 0.4f);

        UICountdownBubble uiCountdownBubble = bubble.GetComponent<UICountdownBubble>();
        uiCountdownBubble.myBot = bot;
        botProperty.muBubble = bubble;

        uiCountdownBubble.duration = botProperty.detectionTimeThreshold;
        uiCountdownBubble.ExcuteFillBar();
    }

    public void SetEmpty()
    {
        if (getPartBubble == null) return;
        _aiBot.CurrentPart = PartType.Empty;
        submitTimer = 0;
        getPartBubble.DestoryBubble();
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag=="SubmissionPoint" && _aiBot.CurrentPart!=PartType.Empty)
        {
            submitTimer += Time.deltaTime;

            if(submitTimer>submitDuration)
            {
                SetEmpty();
                DestoryBubble(transform.parent.gameObject);
                Instantiate(Resources.Load<GameObject>("Prefab/Effect/GivePartGreen"), transform.position, Quaternion.identity);
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "SubmissionPoint" && _aiBot.CurrentPart != PartType.Empty)
        {
            submitTimer = 0;

        }else if(other.gameObject.tag == "SubmissionPoint" && _aiBot.CurrentPart == PartType.Empty)
        {
            DestoryBubble(transform.parent.gameObject);
        }
    }

    /// <summary>
    /// 销毁气泡
    /// </summary>
    /// <param name="bot"></param>
    private void DestoryBubble(GameObject bot)
    {
        Debug.Log("destroy1");
        BotProperty botProperty = bot.GetComponent<BotProperty>();
        if (botProperty == null) return;

        if (botProperty != null && botProperty.muBubble != null)
        {
            Debug.Log("destroy2");
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
