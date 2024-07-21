using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOrderPanel : MonoBehaviour
{

    public Image headImg;
    public Image dialoguePanel;
    public TMP_Text dialogueText;
    public int countDownMax = 3;
    public int countDownVale;

    private float _startFontSize=30;
    private float _numberFountSize=55;
    // Start is called before the first frame update

    
    // Update is called once per frame
    

    /// <summary>
    /// 显示对话框
    /// </summary>
    public void ShowPanel()
    {
        countDownVale = countDownMax;
        dialogueText.text = countDownMax.ToString();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(headImg.rectTransform.DOScale(1, 0.2f));
        sequence.Append(dialoguePanel.rectTransform.DOScale(1, 0.3f));
        sequence.AppendInterval(0.5f).OnComplete(() =>
        {
            SayWords();
        });
    }

    /// <summary>
    /// 收起对话框
    /// </summary>
    public void ClosePanel()
    {
        countDownVale = countDownMax;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(dialoguePanel.rectTransform.DOScale(0, 0.3f));
        sequence.Append(headImg.rectTransform.DOScale(0, 0.2f));
        dialogueText.fontSize = _startFontSize;
        
    }

    public void SayWords()
    {
        StartCoroutine(CountdownCoroutine());
    }
    
    IEnumerator CountdownCoroutine()
    {
        while (countDownVale >= 0)
        {
            dialogueText.fontSize = _numberFountSize;
            dialogueText.text = countDownVale.ToString();
            yield return new WaitForSeconds(1); // 等待1秒
            countDownVale--;
        }
        
        // 倒计时结束后，可以在这里执行其他逻辑
        ClosePanel();
    }
}
