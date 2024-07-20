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

    [Header("攻击参数")]
    public float AttackCD;

    private float _attackTimer;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player._isPlayerSetup)
        {
            Movement(inputSetting.inputDir);
        }
        if (inputSetting.isPressConfirm)
        {
            CallQTE();
        }
        if(inputSetting.isPressScan)
        {
            StartScanPlayerBot();
        }
        if(inputSetting.isPressSwitch)
        {
            Attack();
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
        Vector3 movement = new Vector3(inputDir.x, 0, inputDir.y).normalized * moveSpeed;
        // 移动玩家
        _rigidbody.velocity = new Vector3(movement.x, _rigidbody.velocity.y, movement.z);
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
        if(_attackTimer>0)
        {
            _attackTimer -= Time.deltaTime;
        }
    }


    /// <summary>
    /// 使用QTE技能
    /// </summary>
    private void CallQTE()
    {
        if(_callQTECDTimer > 0)
        {
            return;
        }
        Debug.Log("警察开始qte");

        foreach(var aiBot in SceneManager.Instance.AIBotList)
        {
            aiBot.GetComponent<AIBot>().ExecuteQTE();
        }
        foreach (var playerBot in SceneManager.Instance.PlayerBotList)
        {
            playerBot.GetComponent<PlayerBot>().BeginQTE();
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
            return;
        }


        Collider[] colliders = Physics.OverlapSphere(transform.position, CheckPlayerBotDistance);
        foreach (Collider collider in colliders)
        {

            if (collider.CompareTag("Player"))
            {
                Vector3 directionToPlayer = collider.transform.position - transform.position;
                float angle = Vector3.Angle(transform.forward, directionToPlayer);

                if (angle < CheckPlayerBotAngle / 2f)
                {
                    Debug.Log("扫描出玩家了");
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
