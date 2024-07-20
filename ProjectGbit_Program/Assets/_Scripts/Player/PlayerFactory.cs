using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PlayerFactory : MonoBehaviour
{
    private InputSetting _inputSetting;
    public GameObject myPlayer;
    public float cooldownTime = 20f;
    private float _nextUseTime;
    private float _ambientIndensity=1;

    public Light mainLight;
    
    private void Awake()
    {
        _inputSetting = myPlayer.GetComponent<InputSetting>();
    }

    private void Update()
    {
        RenderSettings.ambientLight=Color.white*_ambientIndensity;
        
        
        if (_inputSetting.isPressScan)
        {
            UseSkill();
        }
    }
    /// <summary>
    /// 应用停电
    /// </summary>
    private void UseSkill()
    {
        if (Time.time >= _nextUseTime)
        {
            // 执行技能
            Blackout(0.2f,1.5f);
            // 设置下次可以使用技能的时间
            _nextUseTime = Time.time + cooldownTime;
        }
        else
        {
            // 冷却中，无法使用技能
            Debug.Log("停电冷却");
        }
    }
    
    private void Blackout(float time,float duration)
    {
        DOTween.To(() => _ambientIndensity, x => _ambientIndensity = x, 0, time)
            .SetEase(Ease.InOutQuad);
        DOTween.To(() => mainLight.intensity, x => mainLight.intensity = x, 0, time)
            .SetEase(Ease.InOutQuad);
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(duration).OnComplete(() =>
        {
            DOTween.To(() => _ambientIndensity, x => _ambientIndensity = x, 1, time)
                .SetEase(Ease.InOutQuad);
            DOTween.To(() => mainLight.intensity, x => mainLight.intensity = x, 1, time)
                .SetEase(Ease.InOutQuad);
        });
    }
}
