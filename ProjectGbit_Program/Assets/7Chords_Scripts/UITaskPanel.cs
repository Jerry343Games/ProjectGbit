using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITaskPanel : MonoBehaviour
{

    private int _targetNum;

    private int _currentNum;

    public List<Sprite> NumSpriteList;

    private Image _firstNumImage;

    private Image _secondNumImage;


    private void Awake()
    {
        _firstNumImage = transform.GetChild(0).GetComponent<Image>();

        _secondNumImage = transform.GetChild(2).GetComponent<Image>();
    }

    public void Init(int targetNum)
    {
        _currentNum = 0;

        _targetNum = targetNum;

        _firstNumImage.sprite = NumSpriteList[_currentNum];

        _secondNumImage.sprite = NumSpriteList[_targetNum];

    }

    public void AddOne()
    {
        _currentNum++;

        _firstNumImage.sprite = NumSpriteList[_currentNum];

        _secondNumImage.sprite = NumSpriteList[_targetNum];
    }




}
