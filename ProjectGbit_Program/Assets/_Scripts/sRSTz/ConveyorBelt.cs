using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public float beltSpeed = 2.0f; // 传送带的速度
    //public float playerResistanceFactor = 0.5f; // 玩家移动时的阻力因子
    public bool isAwake = true;
    public bool isReverse = false;
    private int reverseNum = 1;
    private void OnCollisionStay(Collision collision)
    {
        if (!isAwake) return;

        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            if (isReverse) reverseNum = -1; else reverseNum = 1;
            Vector3 conveyorMovement = transform.forward * beltSpeed * Time.deltaTime * reverseNum;

            BotProperty botProperty = collision.gameObject.GetComponent<BotProperty>();
            rb.velocity += conveyorMovement;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        BotProperty botProperty = collision.gameObject.GetComponent<BotProperty>();
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (botProperty == null && rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2*reverseNum);
    }

    public void ChangeOnOff(bool isAwake)
    {
        this.isAwake = isAwake;
    }
    
    public void ChangeReverse(bool isReverse)
    {
        this.isReverse = isReverse;
        //按中心点左右翻转传送带
    }

}

