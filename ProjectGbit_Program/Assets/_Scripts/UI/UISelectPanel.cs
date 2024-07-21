using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class UISelectPanel : MonoBehaviour
{
    /// <summary>
    /// 选人物体列表
    /// </summary>
    public GameObject[] btnList;
    
    /// <summary>
    /// 当前的玩家数量
    /// </summary>
    private int _currentPlayerNum;

    public Image selectMask;
    
    public GameObject warningPanel;
    
    private void Awake()
    {
        foreach (GameObject btnObj in btnList)
        {
            UIConfirmButtom uiConfirmButtom = btnObj.GetComponent<UIConfirmButtom>();
            uiConfirmButtom.OnPlayerConfirmed+= RegisterPlayer;
            uiConfirmButtom.OnCantConfirm += ShowWarning;
        }
    }

    private void OnDestroy()
    {
        foreach (GameObject btnObj in btnList)
        {
            UIConfirmButtom uiConfirmButtom = btnObj.GetComponent<UIConfirmButtom>();
            uiConfirmButtom.OnPlayerConfirmed-= RegisterPlayer;
            uiConfirmButtom.OnCantConfirm -= ShowWarning;       
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 每次确认玩家时触发
    /// </summary>
    private void RegisterPlayer()
    {
        _currentPlayerNum++;

        MusicManager.Instance.PlaySound("选择角色");
        
        //全部确认时
        if (_currentPlayerNum==PlayerManager.Instance.MaxPlayerNumber())
        {
            Debug.Log("AllPlayerOnline");
            Sequence sequence = DOTween.Sequence();
            //等待一秒
            sequence.AppendInterval(1f).OnComplete(() =>
            {
                //消失菜单
                gameObject.GetComponent<RectTransform>().DOScale(0, 0.5f);
                gameObject.GetComponent<CanvasGroup>().DOFade(0, 0.2f);
                selectMask.DOFade(0, 0.2f);

                GameManager.Instance.StartGame();
                
            });
        }
    }

    private void ShowWarning()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(warningPanel.GetComponent<CanvasGroup>().DOFade(1, 0.3f));
        sequence.AppendInterval(1f);
        sequence.Append(warningPanel.GetComponent<CanvasGroup>().DOFade(0, 0.3f));
    }
}
