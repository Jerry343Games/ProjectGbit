using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPartBubble : MonoBehaviour
{
    public Image inner;
    public Image outer;
    public GameObject myBot;
    private RectTransform _rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(myBot)
        MoveBubble(myBot);
    }

    public void SetInner(PartType partType)
    {
        inner.sprite = Resources.Load<Sprite>("Prefab/Texture/" + partType.ToString());

    }
    private void MoveBubble(GameObject bot)
    {
        //改
        Vector3 screenPos = Camera.main.WorldToScreenPoint(bot.transform.position);
        _rectTransform.position = screenPos;
    }

    public void DestoryBubble()
    {
        Destroy(gameObject);
    }
}
