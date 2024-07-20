using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CloseEnviromentLight : MonoBehaviour
{
    private float _power=1;
    
    void Start()
    {

        DOTween.To(() => _power, x => _power = x, 0, 10)
            .SetEase(Ease.InOutQuad); // 设置缓动类型，这里使用的是InOutQuad
    }

    // Update is called once per frame
    void Update()
    {
        RenderSettings.ambientLight = Color.white * _power;
    }
}
