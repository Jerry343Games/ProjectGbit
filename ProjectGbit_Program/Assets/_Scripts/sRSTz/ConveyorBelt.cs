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
    public float changeTime = 12f;
    
    private void Awake()
    {
        bletSurfaceSet = GetComponent<BeltSurfaceSet>();
        if (awakeReverse)
        {
            ChangeReverseOnce();
        }
        if (awakeStart)
        {
            ChangeOnOffOnce();
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
            PlayerPolice playerPolice = other.gameObject.GetComponent<PlayerPolice>();
            if (isAwake)
            {
                if (playerBot == null && playerPolice == null)
                {
                    rb.velocity += conveyorMovement;
                }
                else if (playerBot != null)
                {
                    playerBot.isOnConveyBelt = true;
                    playerBot.conveyorVelocity = conveyorMovement;
                }else if(playerPolice != null)
                {
                    playerPolice.isOnConveyBelt = true;
                    playerPolice.conveyorVelocity = conveyorMovement;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerBot playerBot = other.gameObject.GetComponent<PlayerBot>();
        PlayerPolice playerPolice = other.gameObject.GetComponent<PlayerPolice>();
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        if (playerBot == null && rb != null&&playerPolice==null)
        {
            rb.velocity = rb.velocity * exitSpeedNum;
            rb.angularVelocity = Vector3.zero;
        }
        else if (playerBot != null)
        {
            playerBot.isOnConveyBelt = false;
        }else if (playerPolice != null)
        {
            playerPolice.isOnConveyBelt = false;
        }
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + -transform.right * 2*reverseNum);
    }*/

    public void ChangeOnOff()
    {
        CancelInvoke(nameof(ChangeOnOffOnce));
        isAwake = !isAwake;
        bletSurfaceSet.OnOffMove(isAwake);
        Invoke(nameof(ChangeOnOffOnce), changeTime);
    }
    public void ChangeOnOffOnce()
    {
        isAwake = !isAwake;
        bletSurfaceSet.OnOffMove(isAwake);
    }
    public void ChangeReverse()
    {
        CancelInvoke(nameof(ChangeReverseOnce));
        isReverse = !isReverse;
        //按中心点左右翻转传送带
        bletSurfaceSet.SwitchDir();
        Invoke(nameof(ChangeReverseOnce), changeTime);
    }
    public void ChangeReverseOnce()
    {
        isReverse = !isReverse;
        //按中心点左右翻转传送带
        bletSurfaceSet.SwitchDir();
    }
}

