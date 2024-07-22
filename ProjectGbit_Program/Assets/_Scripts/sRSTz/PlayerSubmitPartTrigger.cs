using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSubmitPartTrigger : MonoBehaviour
{
    private PlayerBot _playerBot;
    public float detectionTimeThreshold = 3f; // 检测时间阈值
    private float detectionTime = 0f;
    private bool isDetecting = false;
    private void Awake()
    {
        _playerBot = transform.parent.GetComponent<PlayerBot>();
        detectionTimeThreshold = _playerBot.GetComponent<BotProperty>().detectionTimeThreshold;
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
                isDetecting = false;
                detectionTime = 0f;
            }
        }
    }

    public void StartDetection()
    {
        isDetecting = true;
        detectionTime = 0;
    }

    public void StopDetection()
    {
        isDetecting = false;
        detectionTime = 0f;
    }
}
