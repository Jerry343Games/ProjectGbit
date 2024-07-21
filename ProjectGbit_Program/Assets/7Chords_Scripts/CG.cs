using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CG : MonoBehaviour
{
    public List<Sprite> WinCGs;

    public List<Sprite> LoseCGs;

    private Image cgImg;

    private void Awake()
    {
        cgImg =transform.GetChild(0).GetComponent<Image>();
    }
    private void Start()
    {
        GameManager.Instance.GameFinishedAction += OnGameFininshed;
    }

    private void  OnGameFininshed(bool isBotWin)
    {
        if (isBotWin)
        {
            PlayWinCG();
        }
        else
        {
            PlayLoseCG();

        }
    }
    public void PlayWinCG()
    {
        Sequence s = DOTween.Sequence();

        s.Append(cgImg.DOFade(1, 0.5f).OnStart(() =>
        {
            cgImg.sprite = WinCGs[0];
        }));
        s.Append(cgImg.DOFade(0, 0.3f).OnComplete(() =>
        {
            cgImg.sprite = WinCGs[1];
        }));

        s.Append(cgImg.DOFade(1, 0.5f).OnStart(() =>
        {
            cgImg.sprite = WinCGs[1];
        }));
        s.Append(cgImg.DOFade(0, 0.3f).OnComplete(() =>
        {
            cgImg.sprite = WinCGs[2];
        }));
        s.Append(cgImg.DOFade(1, 0.5f).OnStart(() =>
        {
            cgImg.sprite = WinCGs[2];
        }));
        s.Append(cgImg.DOFade(0, 0.3f).OnComplete(() =>
        {
            cgImg.sprite = WinCGs[3];
        }));
        s.Append(cgImg.DOFade(0, 0.3f));

    }

    public void PlayLoseCG()
    {
        Sequence s = DOTween.Sequence();

        s.Append(cgImg.DOFade(1, 0.5f).OnStart(() =>
        {
            cgImg.sprite = LoseCGs[0];
        }));
        s.Append(cgImg.DOFade(0, 0.3f).OnComplete(() =>
        {
            cgImg.sprite = LoseCGs[1];
        }));

        s.Append(cgImg.DOFade(1, 0.5f).OnStart(() =>
        {
            cgImg.sprite = LoseCGs[1];
        }));
        s.Append(cgImg.DOFade(0, 0.3f));
 
    }
}
