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
            //销毁零件
            Destroy(other.gameObject);
            //Bot执行获得零件方法 结束等待
            _aiBot.GetPart();

        }
    }

}
