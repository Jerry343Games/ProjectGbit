using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGetPartTrigger : MonoBehaviour
{
    private PlayerBot _playerBot;

    public GameObject mainCanvas;

    public UIPartBubble getPartBubble;

    private void Awake()
    {
        _playerBot = transform.parent.GetComponent<PlayerBot>();
    }
    private void OnTriggerEnter(Collider other)
    {


        if (_playerBot.currentPart!=PartType.Empty)
        {
            return;
        }
        if (other.gameObject.tag == "Part")
        {
            _playerBot.GetPart(other.gameObject.transform.parent.GetComponent<Part>());

            CreateBubble(other.gameObject.transform.parent.GetComponent<Part>().partType);

            //Ïú»ÙÁã¼þ
            Destroy(other.gameObject);

        }

    }

    public void CreateBubble(PartType type)
    {
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
        _playerBot.currentPart = PartType.Empty;
        getPartBubble.DestoryBubble();
        Instantiate(Resources.Load<GameObject>("Prefab/Effect/GivePartGreen"), transform.position, Quaternion.identity);
    }
}
