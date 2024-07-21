using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public float beltSpeed = 2.0f; // 传送带的速度
    //public float playerResistanceFactor = 0.5f; // 玩家移动时的阻力因子
    private bool isAwake = true;
    private bool isReverse = false;
    private int reverseNum = 1;
    public BeltSurfaceSet bletSurfaceSet;
    public bool awakeReverse = false;
    public bool awakeStart = false;
    public float exitSpeedNum = 0.5f;
    private void Awake()
    {
        bletSurfaceSet = GetComponent<BeltSurfaceSet>();
        if (awakeReverse)
        {
            ChangeReverse();
        }
        if (awakeStart)
        {
            ChangeOnOff();
        }
    }
    private void OnTriggerStay(Collider other)
    {
       

        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            if (isReverse) reverseNum = -1; else reverseNum = 1;
            Vector3 conveyorMovement = -transform.right * beltSpeed * Time.deltaTime * reverseNum;

            PlayerBot playerBot = other.gameObject.GetComponent<PlayerBot>();
            if (isAwake)
            {
                if (playerBot == null)
                {
                    rb.velocity += conveyorMovement;
                }
                else
                {
                    playerBot.isOnConveyBelt = true;
                    playerBot.conveyorVelocity = conveyorMovement;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerBot playerBot = other.gameObject.GetComponent<PlayerBot>();
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        if (playerBot == null && rb != null)
        {
            rb.velocity = rb.velocity * exitSpeedNum;
            rb.angularVelocity = Vector3.zero;
        }
        else if (playerBot != null)
        {
            playerBot.isOnConveyBelt = false;
        }
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + -transform.right * 2*reverseNum);
    }*/

    public void ChangeOnOff()
    {
        isAwake = !isAwake;
        bletSurfaceSet.OnOffMove(isAwake);

    }
    
    public void ChangeReverse()
    {
        isReverse = !isReverse;
        //按中心点左右翻转传送带
        bletSurfaceSet.SwitchDir();
    }
    
}

