using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public float beltSpeed = 2.0f; // ���ʹ����ٶ�
    //public float playerResistanceFactor = 0.5f; // ����ƶ�ʱ����������
    public bool isAwake = true;
    public bool isReverse = false;
    private int reverseNum = 1;
    private void OnTriggerStay(Collider other)
    {
        if (!isAwake) return;

        Rigidbody rb = other.attachedRigidbody;

        if (rb != null)
        {
            if (isReverse) reverseNum = -1; else reverseNum = 1;
            Vector3 conveyorMovement = transform.forward * beltSpeed * Time.deltaTime * reverseNum;

            BotProperty botProperty = other.GetComponent<BotProperty>();
            rb.velocity += conveyorMovement;
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        BotProperty botProperty = other.GetComponent<BotProperty>();
        Rigidbody rb = other.attachedRigidbody;
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
        //�����ĵ����ҷ�ת���ʹ�
    }

}

