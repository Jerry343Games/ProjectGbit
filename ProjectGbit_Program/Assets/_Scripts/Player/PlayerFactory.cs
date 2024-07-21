using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PlayerFactory : MonoBehaviour
{
    private InputSetting _inputSetting;
    public GameObject myPlayer;
    public float cooldownTime = 10f;
    private float _nextUseTime;
    private float _ambientIndensity=1;

    public Light[] mainLights;
    private Color _originalAmbientLight;
    private Color _currentAmbirntLight;
    private void Awake()
    {
        _inputSetting = myPlayer.GetComponent<InputSetting>();
        _originalAmbientLight = RenderSettings.ambientLight;
        _currentAmbirntLight = _originalAmbientLight;
    }

    private void Update()
    {
        RenderSettings.ambientLight = _currentAmbirntLight;
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
            Blackout(0.2f,2f);
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
        
            DOTween.To(() => _currentAmbirntLight, x => _currentAmbirntLight = x, Color.black, time)
                .SetEase(Ease.InOutQuad);
            foreach (var mainLight in mainLights)
            {
                DOTween.To(() => mainLight.intensity, x => mainLight.intensity = x, 0, time)
                    .SetEase(Ease.InOutQuad);
            }
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(duration).OnComplete(() =>
            {
                DOTween.To(() => _currentAmbirntLight, x => _currentAmbirntLight = x, _originalAmbientLight, time)
                    .SetEase(Ease.InOutQuad);
                foreach (var mainLight in mainLights)
                {
                    if (mainLight.type==LightType.Directional)
                    {
                        DOTween.To(() => mainLight.intensity, x => mainLight.intensity = x, 0.24f, time)
                            .SetEase(Ease.InOutQuad);
                    }
                    else
                    {
                        DOTween.To(() => mainLight.intensity, x => mainLight.intensity = x, 28, time)
                            .SetEase(Ease.InOutQuad);
                    }
                }
            });
    }
}
