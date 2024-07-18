using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
public class UIConfirmButtom : MonoBehaviour, ISelectHandler,IDeselectHandler
{
    public GameObject player;

    private Button _selectBtn;

    private MultiplayerEventSystem _multiplayerEventSystem;

    public Image[] selectedIcons;

    public Color selectColor=Color.gray;
    public Color confirmColor=Color.cyan;

    private bool _isConfirmed;

    private int _currentSelectedNum;

    private void OnEnable()
    {
        _selectBtn = GetComponent<Button>();
        _selectBtn.onClick.AddListener(OnClickSelectBtn);
    }

    private void OnDisable()
    {
        _selectBtn.onClick.RemoveListener(OnClickSelectBtn);
    }

    /// <summary>
    /// 选中事件
    /// </summary>
    /// <param name="eventData"></param>
    public void OnSelect(BaseEventData eventData)
    {
        switch (MultiplayerEventSystem.current.gameObject.GetComponent<PlayerInput>().playerIndex)
        {
            case 0:
                selectedIcons[0].gameObject.SetActive(true);
                break;
            case 1:
                selectedIcons[1].gameObject.SetActive(true);
                break;
            case 2:
                selectedIcons[2].gameObject.SetActive(true);
                break;
            case 3:
                selectedIcons[3].gameObject.SetActive(true);
                break;
        }
        
        if (!_isConfirmed)
        {
            GetComponent<Image>().color = selectColor;
        }

        _currentSelectedNum++;
    }

    /// <summary>
    /// 选中离开事件
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDeselect(BaseEventData eventData)
    {
        switch (MultiplayerEventSystem.current.gameObject.GetComponent<PlayerInput>().playerIndex)
        {
            case 0:
                selectedIcons[0].gameObject.SetActive(false);
                break;
            case 1:
                selectedIcons[1].gameObject.SetActive(false);
                break;
            case 2:
                selectedIcons[2].gameObject.SetActive(false);
                break;
            case 3:
                selectedIcons[3].gameObject.SetActive(false);
                break;
        }

        _currentSelectedNum--;
        if (_currentSelectedNum==0)
        {
            GetComponent<Image>().color = Color.white;
        }
    }

    private void OnClickSelectBtn()
    {
        
    }
}
