using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBot : MonoBehaviour
{
    public float moveSpeed;

    private Rigidbody _rigidbody;

    public InputSetting inputSetting;

    public Player player;

    public bool isOnConveyBelt = false;

    public Vector3 conveyorVelocity;

    public Part currentPart = null;

    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        SceneManager.Instance.RegisterPlayerBot(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (player._isPlayerSetup)
        {
            
            Movement(inputSetting.inputDir);
            
        }
        
    }
    
    /// <summary>
    /// 玩家移动方法
    /// </summary>
    /// <param name="inputDir"></param>
    private void Movement(Vector2 inputDir)
    {
        Vector3 movement = new Vector3(inputDir.x, 0, inputDir.y).normalized * moveSpeed ;
        // 移动玩家
        if (!isOnConveyBelt)
        {
            _rigidbody.velocity = new Vector3(movement.x, _rigidbody.velocity.y, movement.z);
        }
        else
        {
            // 应用玩家输入方向和传送带速度
            Vector3 totalVelocity = new Vector3(movement.x, _rigidbody.velocity.y, movement.z) + conveyorVelocity;
            _rigidbody.velocity = totalVelocity;
        }
        
    }
    public void SubmitPart()
    {
        if (currentPart == null) return;
        //在这里写检测根据part的不同种类提交

        currentPart = null;
    }
    /// <summary>
    /// 开始QTE
    /// </summary>
    public void BeginQTE()
    {
        StartCoroutine(QTERoutine());
    }

    IEnumerator QTERoutine()
    {
        float QTETimer = 0;
        float QTEMaxTime = GetComponent<BotProperty>().QTEMaxTime;

        while (QTETimer < QTEMaxTime)
        {
            //输入按键后结束
            if (inputSetting.isPressConfirm)
            {
                Debug.Log("qte成功");
                yield break;
            }
            QTETimer += Time.deltaTime;
            yield return null;
        }
        //未完成输入，暴露
        Debug.Log("Qte失败");

    }
    /// <summary>
    /// 获得零件之后的响应
    /// </summary>
    public void GetPart(Part part)
    {
        currentPart = part;
        Debug.Log(part);
    }
}
