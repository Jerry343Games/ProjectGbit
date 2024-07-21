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

    public PartType currentPart = PartType.Empty;

    public Animator myAnimator;

    public QTEUI qteUI;
    public BotProperty botProperty;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        SceneManager.Instance.RegisterPlayerBot(this);
        qteUI = GetComponentInChildren<QTEUI>();
        botProperty = GetComponent<BotProperty>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player._isPlayerSetup)
        {
            
            Movement(inputSetting.inputDir);
            
        }
        ConfirmDirection();
    }
    private float _pressConfirmTimer = 0;
    private bool previousConfirmState = false; // 记录上一次的布尔值状态
    private void ConfirmDirection()
    {
        //_pressConfirmTimer -= Time.deltaTime; // 更新计时器
        if (!previousConfirmState && inputSetting.isPressConfirm)
        {
            
            //if (_pressConfirmTimer <= 0) // 检查计时器是否超过间隔
           //{
               // if (inputSetting.isPressSwitch)
                {
                    //_rigidbody.AddForce(Vector3.up * 5, ForceMode.Impulse);
                    qteUI.CreatBubble(gameObject);
                   // _pressConfirmTimer = 0.5f; // 重置计时器
                }

            //}
        }
        previousConfirmState = inputSetting.isPressConfirm;
    }
   
    /// <summary>
    /// 玩家移动方法
    /// </summary>
    /// <param name="inputDir"></param>
    private void Movement(Vector2 inputDir)
    {
        //动画控制
        if (inputDir!=Vector2.zero)
        {
           myAnimator.SetBool("isRun",true);
            Vector3 direction = new Vector3(inputDir.x, 0, inputDir.y);
            Vector3 targetPosition = transform.position -direction;
            transform.LookAt(new Vector3(targetPosition.x, transform.position.y, targetPosition.z));
        }
        else
        {
            myAnimator.SetBool("isRun",false);
        }
        
        Vector3 movement = new Vector3(inputDir.x, 0, inputDir.y).normalized * moveSpeed ;
        // 移动玩家
        if (!isOnConveyBelt)
        {
            _rigidbody.velocity = new Vector3(movement.x, _rigidbody.velocity.y, movement.z);
        }
        else
        {
            // 应用玩家输入方向和传送带速度
            Vector3 totalVelocity = new Vector3(movement.x, _rigidbody.velocity.y, movement.z) +conveyorVelocity*10;
            _rigidbody.velocity = totalVelocity;
        }
        
    }
    public void SubmitPart()
    {
        if (currentPart==PartType.Empty) return;
        //在这里写检测根据part的不同种类提交
        GameManager.Instance.AddPartToTask(currentPart);
        currentPart = PartType.Empty;
        FindObjectOfType<PlayerFactory>()?.InitHealEffect();
        Instantiate(Resources.Load<GameObject>("Prefab/Effect/GivePartGreen"), transform.position, Quaternion.identity);



    }

    /// <summary>
    /// 获得零件之后的响应
    /// </summary>
    public void GetPart(Part part)
    {
        currentPart = part.partType;
        Debug.Log(part);
    }

    public void Dead()
    {
        SceneManager.Instance.RemovePlayerBot(this);
        Instantiate(Resources.Load<GameObject>("Prefab/Effect/PlayerDeadEffect"), transform.position, Quaternion.identity);
        GetComponentInChildren<PlayerGetPartTrigger>().SetEmpty();//
        Debug.Log(gameObject.name + "dead");
        if(botProperty.muBubble!=null)
        botProperty.muBubble.GetComponent<UICountdownBubble>().StopAndDeleteBubble();//删掉交付气泡

        //Destroy(GetComponent<UICountdownBubble>())
        //Destroy(botProperty.);
        Destroy(gameObject);
    }
}
