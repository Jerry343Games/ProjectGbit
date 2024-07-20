using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITaskPanel : MonoBehaviour
{
    private Text _amountText;

    private int _targetNum;

    private int _currentNum;

    private void Awake()
    {
        _amountText = transform.GetChild(1).GetComponent<Text>();
    }

    public void Init(int targetNum)
    {
        _currentNum = 0;

        _targetNum = targetNum;

        _amountText.text = _currentNum+  "/" + _targetNum;



    }

    public void AddOne()
    {
        _currentNum++;

        _amountText.text = _currentNum + "/" + _targetNum;

        if (_currentNum >= _targetNum)
        {
            //¼¤»î´ò¹´·ûºÅ
        }
    }




}
