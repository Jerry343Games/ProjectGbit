using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIRepairIcon : MonoBehaviour
{
    private GameObject factoryPlayer;
    private Vector3 pos;
    public Image myImg;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void Init(PartType type)
    {
        factoryPlayer = GameObject.FindWithTag("Factory");
        pos = Camera.main.WorldToScreenPoint(factoryPlayer.transform.position);
        switch (type)
        {
            case PartType.type1:
                myImg.sprite = Resources.Load<Sprite>("Prefab/Texture/type1");
                break;
            case PartType.type2:
                myImg.sprite = Resources.Load<Sprite>("Prefab/Texture/type2");
                break;
            case PartType.type3:
                myImg.sprite = Resources.Load<Sprite>("Prefab/Texture/type3");
                break;
        }
        transform.DOShakePosition(0.5f, 10f);
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(0.6f);
        sequence.Append(GetComponent<RectTransform>().DOMove(pos,0.5f)).OnComplete(() =>
        {
            factoryPlayer.GetComponent<PlayerFactory>().InitHealEffect();
            Destroy(gameObject);
        });
    }
}
