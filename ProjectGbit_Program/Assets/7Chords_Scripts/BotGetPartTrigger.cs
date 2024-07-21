using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotGetPartTrigger : MonoBehaviour
{
    private AIBot _aiBot;

    private void Awake()
    {
        _aiBot = transform.parent.GetComponent<AIBot>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(_aiBot.CurrentPart != PartType.Empty)
        {
            return;
        }
        if(other.gameObject.tag=="Part")
        {
            
            //Botִ�л��������� �����ȴ�
            _aiBot.GetPart(other.gameObject.transform.parent.GetComponent<Part>().partType);
            //�������
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "SubmissionPoint")
        {
            Debug.Log(11);
            //Botִ�л��������� �����ȴ�
            _aiBot.CurrentPart = PartType.Empty;


        }
    }

}
