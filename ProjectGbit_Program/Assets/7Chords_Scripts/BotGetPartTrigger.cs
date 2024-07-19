using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotGetPartTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Part")
        {
            //�������
            Destroy(other.gameObject);
            //Botִ�л��������� �����ȴ�
            transform.parent.GetComponent<AIBot>().GetPart();

        }
    }

}
