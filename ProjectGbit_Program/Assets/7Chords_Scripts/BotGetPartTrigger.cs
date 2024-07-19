using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotGetPartTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Part")
        {
            //销毁零件
            Destroy(other.gameObject);
            //Bot执行获得零件方法 结束等待
            transform.parent.GetComponent<AIBot>().GetPart();

        }
    }

}
