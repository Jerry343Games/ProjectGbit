using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSubmitPartTrigger : MonoBehaviour
{
    private PlayerBot _playerBot;
    public float detectionTimeThreshold = 3f; // ���ʱ����ֵ
    private float detectionTime = 0f;
    private bool isDetecting = false;
    private void Awake()
    {
        _playerBot = transform.parent.GetComponent<PlayerBot>();
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SubmissionPoint"))
        {
            StartDetection();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("SubmissionPoint"))
        {
            StopDetection();
        }
    }
    private void Update()
    {
        if (isDetecting)
        {
            detectionTime += Time.deltaTime;
            if (detectionTime >= detectionTimeThreshold)
            {
                Submit();
                isDetecting = false;
                detectionTime = 0f;
            }
        }
    }

    public void StartDetection()
    {
        isDetecting = true;
    }

    public void StopDetection()
    {
        isDetecting = false;
        detectionTime = 0f;
    }

    private void Submit()
    {
        _playerBot.SubmitPart();
    }
}