using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CameraShake : MonoBehaviour
{
    private Camera _camera;

    void Start()
    {
        _camera = Camera.main;
        if (_camera != null && _camera != this.GetComponent<Camera>())
        {
            Debug.LogWarning("CameraShake script should be attached to the main camera.");
        }
    }

    public void ShakeCamera(float duration, float strength, int vibrato = 10, float randomness = 90f, bool fadeOut = true)
    {
        if (_camera != null)
        {
            _camera.DOShakePosition(duration, strength, vibrato, randomness, fadeOut);
        }
    }
}
