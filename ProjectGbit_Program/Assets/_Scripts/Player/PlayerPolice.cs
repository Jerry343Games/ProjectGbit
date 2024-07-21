using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPolice : MonoBehaviour
{
    public float moveSpeed;

    private Rigidbody _rigidbody;

    public InputSetting inputSetting;

    public Player player;

    [Header("下QTE命令参数")]
    public float CallQTECD;//下QTE命令的cd

    private float _callQTECDTimer;

    [Header("扫描参数")]
    public float ScanBotDuration;//扫描持续时间

    private float _scanBotTimer;

    public float ScanBotCD;//扫描CD

    private float _scanBotCDTimer;

    public float CheckPlayerBotDistance;//扇形扫描区域弦长

    public float CheckPlayerBotAngle;//扇形扫描区域角度

    private bool _isBeingScan;

    private bool _canStartScan;

    public UIOrderPanel uiOrderPanel;
    [Header("攻击参数")]
    public float AttackCD;

    private float _attackTimer;
    public PlayerPoliceAttack policeAttackArea;

    public Animator myAnimator;
    [Header("传送带参数")]
    public bool isOnConveyBelt = false;
    public Vector3 conveyorVelocity;
    //用于攻击动画罚站
    private bool _isAttack;


    [Header("传送带参数")]

    public Material ScanRangeMaterial;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        policeAttackArea = GetComponentInChildren<PlayerPoliceAttack>();
        //Invoke("CallQTE", 3f);
    }

    // Update is called once per frame
    // 新增一个布尔变量，初始值为false
    private bool hasPressed = false;
    private float pressResetTime = 0.5f; // 0.5秒的重置时间
    private float pressTimer = 0f;
    void Update()
    {
        if (player._isPlayerSetup)
        {
            Movement(inputSetting.inputDir);
        }



        SwitchDirection();
        // 检查是否需要重置hasPressed变量
        ConfirmDirection();
        if (inputSetting.isPressScan)
        {
            StartScanPlayerBot();
        }
        
        SkillCDUpdate();
        ScanPlayerBot();
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
            //转向
            Vector3 direction = new Vector3(inputDir.x, 0, inputDir.y);
            Vector3 targetPosition = transform.position +direction;
            transform.LookAt(new Vector3(targetPosition.x, transform.position.y, targetPosition.z));
        }
        else
        {
            myAnimator.SetBool("isRun",false);
        }

        if (!_isAttack)
        {
            Vector3 movement = new Vector3(inputDir.x, 0, inputDir.y).normalized * moveSpeed;
            // 移动玩家
            // 移动玩家
            if (!isOnConveyBelt)
            {
                _rigidbody.velocity = new Vector3(movement.x, _rigidbody.velocity.y, movement.z);
            }
            else
            {
                // 应用玩家输入方向和传送带速度
                Vector3 totalVelocity = new Vector3(movement.x, _rigidbody.velocity.y, movement.z) + conveyorVelocity * 10;
                _rigidbody.velocity = totalVelocity;
            }
        }
        else
        {
            _rigidbody.velocity = Vector3.zero;
        }
    }

    /// <summary>
    /// 技能CD计时器
    /// </summary>
    private void SkillCDUpdate()
    {
        //QTECD计时
        if(_callQTECDTimer>0)
        {
            _callQTECDTimer -= Time.deltaTime;
        }
        //扫描时间计时
        if (_scanBotTimer>0)
        {
            _scanBotTimer -= Time.deltaTime;
        }
        else
        {
            _isBeingScan = false;
        }
        //扫描CD计时
        if (_scanBotCDTimer > 0)
        {
            _scanBotCDTimer -= Time.deltaTime;
        }
        else
        {
            _canStartScan = true;
        }
        //攻击CD计时
        if(_attackTimer > 0)
        {
            _attackTimer -= Time.deltaTime;
        }
    }


    /// <summary>
    /// 使用QTE技能
    /// </summary>
    private void CallQTE()
    {
        Debug.Log("callQTE");
        if(_callQTECDTimer > 0)
        {
            return;
        }
        Camera.main.GetComponent<CameraShake>().ShakeCamera(0.3f,1f);
        uiOrderPanel.ShowPanel();
        Invoke(nameof(CallAIQte), uiOrderPanel.countDownMax);
        
        hasPressed = false;
    }
    public void CallAIQte()
    {
        foreach (var aiBot in SceneManager.Instance.AIBotList)
        {
            aiBot.GetComponent<AIBot>().ExecuteQTE();
        }
    }
    private float _pressConfirmTimer = 2;
    private void ConfirmDirection()
    {
        
        if (!GameManager.Instance.GameStarted) return;
        _pressConfirmTimer -= Time.deltaTime; // 更新计时器
        if (_pressConfirmTimer <= 0f) // 检查计时器是否超过间隔
        {

            if (inputSetting.isPressConfirm)
            {
                
                _pressConfirmTimer = CallQTECD; // 重置计时器
                Debug.Log("confirm");
                CallQTE();
                
            }
            
        }
    }
    private float _pressSwitchTimer = 0;
    private void SwitchDirection()
    {
        _pressSwitchTimer -= Time.deltaTime; 
        if (_pressSwitchTimer <= 0f) 
        {
            if (inputSetting.isPressSwitch)
            {

                Attack();
                _pressSwitchTimer = AttackCD; 
            }

        }
    }
    /// <summary>
    /// 开始扫描机器人
    /// </summary>
    private void StartScanPlayerBot()
    {
        if(_canStartScan)
        {
            _isBeingScan = true;

            _canStartScan = false;

            _scanBotTimer = ScanBotDuration;

            _scanBotCDTimer = ScanBotCD;
        }
    }    

    /// <summary>
    /// 扫描机器人技能
    /// </summary>
    private void ScanPlayerBot()
    {
        if(!_isBeingScan)
        {
            ScanRangeMaterial.SetFloat("_Alpha", 0);
            return;
        }
        ScanRangeMaterial.SetFloat("_Alpha", 0.05f);
        Collider[] colliders = Physics.OverlapSphere(transform.position, CheckPlayerBotDistance);
        ScanRangeMaterial.SetColor("_Color", Color.white);

        foreach (Collider collider in colliders)
        {

            if (collider.CompareTag("Player"))
            {
                Vector3 directionToPlayer = collider.transform.position - transform.position;
                float angle = Vector3.Angle(transform.forward, directionToPlayer);

                if (angle < CheckPlayerBotAngle / 2f)
                {
                    ScanRangeMaterial.SetColor("_Color", Color.red);

                }
            }

        }
    }

    /// <summary>
    /// 警察攻击
    /// </summary>
    private void Attack()
    {
        if(_attackTimer>0)
        {
            return;
        }
        Debug.Log("玩家发动了攻击");
        myAnimator.SetTrigger("Attack");
        _isAttack = true;
        StartCoroutine(WaitToEndAttack(0.7f));
        policeAttackArea.AttackPlayer();
    }

    IEnumerator WaitToEndAttack(float duration)
    {
        yield return new WaitForSeconds(duration);
        _isAttack = false;
    }

    /// <summary>
    /// 辅助线绘制
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, CheckPlayerBotDistance);

        Vector3 viewAngleA = Quaternion.AngleAxis(CheckPlayerBotAngle / 2, transform.up) * transform.forward;//扇形视野射线初始角度
        Vector3 viewAngleB = Quaternion.AngleAxis(-CheckPlayerBotAngle / 2, transform.up) * transform.forward;//扇形视野射线终止角度


        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * CheckPlayerBotDistance);//起点，方向*长度
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * CheckPlayerBotDistance);//同上
    }

}
