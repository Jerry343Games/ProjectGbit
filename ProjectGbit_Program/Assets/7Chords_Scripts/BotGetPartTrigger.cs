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
        if(_aiBot.CurrentPart)
        {
            return;
        }
        if(other.gameObject.tag=="Part")
        {
            //�������
            Destroy(other.gameObject);
            //Botִ�л��������� �����ȴ�
            _aiBot.GetPart();

        }
    }

}
