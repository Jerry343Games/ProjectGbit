using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerGetPartTrigger : MonoBehaviour
{
    private PlayerBot _playerBot;

    public GameObject mainCanvas;

    public UIPartBubble getPartBubble;

    private float submitTimer;

    private float submitDuration;

    private void Awake()
    {
        _playerBot = transform.parent.GetComponent<PlayerBot>();
    }

    private void Start()
    {
        submitDuration = transform.parent.GetComponent<BotProperty>().detectionTimeThreshold;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "SubmissionPoint" && _playerBot.currentPart!=PartType.Empty)
        {
            BotProperty botProperty = _playerBot.GetComponent<BotProperty>();
            GameObject bubble = Instantiate(Resources.Load<GameObject>("Prefab/UI/UICountdownBubble"), mainCanvas.transform);
            Debug.Log(bubble);
            bubble.GetComponent<RectTransform>().localScale = Vector3.zero;
            bubble.GetComponent<RectTransform>().DOScale(1, 0.4f);

            UICountdownBubble uiCountdownBubble = bubble.GetComponent<UICountdownBubble>();
            uiCountdownBubble.myBot = _playerBot.gameObject;
            botProperty.muBubble = bubble;

            uiCountdownBubble.duration = botProperty.detectionTimeThreshold;
            uiCountdownBubble.ExcuteFillBar();
        }

        if (_playerBot.currentPart!=PartType.Empty)
        {
            return;
        }
        if (other.gameObject.tag == "Part")
        {
            _playerBot.GetPart(other.gameObject.transform.parent.GetComponent<Part>());

            CreateBubble(other.gameObject.transform.parent.GetComponent<Part>().partType);

            //销毁零件
            Destroy(other.gameObject);
        }


    }


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "SubmissionPoint" && _playerBot.currentPart != PartType.Empty)
        {
            submitTimer += Time.deltaTime;

            if (submitTimer > submitDuration)
            {
                SetEmpty();
                Instantiate(Resources.Load<GameObject>("Prefab/Effect/GivePartGreen"), transform.position, Quaternion.identity);
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "SubmissionPoint")
        {
            DestoryBubble(transform.parent.gameObject);

            submitTimer = 0;
        }
    }

    public void CreateBubble(PartType type)
    {
        if (getPartBubble != null)
        {
            getPartBubble.DestoryBubble();
        }
        Instantiate(Resources.Load<GameObject>("Prefab/Effect/PickupTaskitem"), transform.position, Quaternion.identity);
        GameObject bubble = Instantiate(Resources.Load<GameObject>("Prefab/UI/UIPartBubble"), mainCanvas.transform);
        getPartBubble = bubble.GetComponent<UIPartBubble>();
        bubble.GetComponent<RectTransform>().localScale = Vector3.zero;
        bubble.GetComponent<RectTransform>().DOScale(1, 0.4f);
        getPartBubble.SetInner(type);
        getPartBubble.myBot = transform.parent.gameObject;
    }
    public void SetEmpty()
    {
        if (getPartBubble == null) return;
        submitTimer = 0;
        getPartBubble.DestoryBubble();
    }

    /// <summary>
    /// 销毁气泡
    /// </summary>
    /// <param name="bot"></param>
    private void DestoryBubble(GameObject bot)
    {

        BotProperty botProperty = bot.GetComponent<BotProperty>();
        if (botProperty == null) return;

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
