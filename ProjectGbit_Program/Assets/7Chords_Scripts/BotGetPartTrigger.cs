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


    private void Awake()
    {
        _aiBot = transform.parent.GetComponent<AIBot>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "SubmissionPoint")
        {
            Invoke("SetEmpty", 3f);
        }
        if (_aiBot.CurrentPart != PartType.Empty)
        {
            return;
        }
        if(other.gameObject.tag=="Part")
        {
            Instantiate(Resources.Load<GameObject>("Prefab/Effect/PickupTaskitem"), transform.position, Quaternion.identity);

            GameObject bubble = Instantiate(Resources.Load<GameObject>("Prefab/UI/UIPartBubble"), mainCanvas.transform);
            getPartBubble = bubble.GetComponent<UIPartBubble>();
            bubble.GetComponent<RectTransform>().localScale = Vector3.zero;
            bubble.GetComponent<RectTransform>().DOScale(1, 0.4f);
            PartType type = other.gameObject.transform.parent.GetComponent<Part>().partType;
            getPartBubble.SetInner(type);
            getPartBubble.myBot = _aiBot.gameObject;
            //Bot执行获得零件方法 结束等待
            _aiBot.GetPart(type);
            //销毁零件
            Destroy(other.gameObject);
        }
    }
    private void SetEmpty()
    {
        _aiBot.CurrentPart = PartType.Empty;
        getPartBubble.DestoryBubble();
        Instantiate(Resources.Load<GameObject>("Prefab/Effect/GivePartGreen"), transform.position, Quaternion.identity);
    }

}
