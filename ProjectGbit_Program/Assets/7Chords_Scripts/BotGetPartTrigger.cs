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
            
            //Bot执行获得零件方法 结束等待
            _aiBot.GetPart(other.gameObject.transform.parent.GetComponent<Part>().partType);
            //销毁零件
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "SubmissionPoint")
        {
            Debug.Log(11);
            //Bot执行获得零件方法 结束等待
            _aiBot.CurrentPart = PartType.Empty;


        }
    }

}
