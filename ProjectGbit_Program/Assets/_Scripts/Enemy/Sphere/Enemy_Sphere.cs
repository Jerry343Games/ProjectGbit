using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//利用枚举列出所有敌人状态:站岗，巡逻，追击，死亡
public enum EnemyStatus 
{ 
    Guard, 
    Patrol, 
    Chase, 
    Dead 
};




public class Enemy_Sphere : MonoBehaviour
{
    //多个敌人保持间距(间距不同要在特定的敌人脚本中额外写） UI制作与显示

    private Rigidbody _rb;
    private Animator _anim;
    private Collider _coll;



    private EnemyStatus _enemyStatus;

    public GameObject Target;//玩家目标

    private NavMeshAgent _agent;

    [Header("状态相关")]
    public int MaxHealth;
    private int _currentHealth;



    [Header("视野相关")]
    public float CheckPlayerDistance;//扇形视野弦长
    public float CheckPlayerAngle;//扇形视野角度


    [Header("巡逻相关")]
    public Transform[] PatolPoints;
    private Vector3 _guardPos;//初始站岗点
    private int _wayPointIndex = 0;//巡逻点标记，用以循环
    public float PatrolSpeed;//巡逻速度


    public float PatrolStayDuration;//巡逻驻足观察的时间
    private float _patrolStayTimer;//巡逻驻足观察的计时器


    [Header("追击相关")]
    public float WaitPlayerExitDuration;//丢失玩家后原地停留的时间
    private float _waitPlayerExitTimer;//丢失玩家后原地停留时间的计时器
    public float ChaseSpeed;//追击速度
    public float ChaseStopDistance;
    [Header("攻击相关")]
    public float AttackCoolDown;
    private float _attackCoolDownTimer;


    [Header("状态相关")]

    //声明状态
    [SerializeField] bool isGuard;//是否是站岗型敌人
    [SerializeField] bool isFindPlayer;//是否发现玩家
    [SerializeField] bool isWalk;//是否行走
    [SerializeField] bool isDead;
    [SerializeField] bool isChase;//
    [SerializeField] bool isFollow;//是否追逐，在动画控制器中做追击和战力攻击的切换
    [SerializeField] bool isCritical;//是否暴击
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _coll = GetComponent<Collider>();
        _anim = GetComponent<Animator>();

        _agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        _guardPos = transform.position;
        _waitPlayerExitTimer = PatrolStayDuration;
        _attackCoolDownTimer = AttackCoolDown;
        if (!isGuard)//初始化判断站岗型还是巡逻型敌人
        {
            _enemyStatus = EnemyStatus.Patrol;
            _patrolStayTimer = PatrolStayDuration;
        }
        else
        {
            _enemyStatus = EnemyStatus.Guard;
        }


        _currentHealth = MaxHealth;
    }

    private void Update()
    {
        if (_currentHealth == 0)
        {
            isDead = true;
            _enemyStatus = EnemyStatus.Dead;
        }
        if (!isDead)
        {
            FindPlayer();
            SwitchStatus();
            //SwitchAnimation();
        }
        switch (_enemyStatus)
        {
            case EnemyStatus.Guard:
                Guard();
                break;
            case EnemyStatus.Patrol:
                Patrol();
                break;
            case EnemyStatus.Chase:
                Chase();
                break;
            case EnemyStatus.Dead:
                Dead();
                break;

        }

        if (_attackCoolDownTimer > -1f)//设置个范围，防止一直计算性能过度消耗
        {
            _attackCoolDownTimer -= Time.deltaTime;
        }
    }
    private void SwitchStatus()//敌人状态转变的方法
    {


        if (isFindPlayer)
        {
            if (Vector2.Distance(transform.position, Target.transform.position) > ChaseStopDistance)//需要判断敌人不在攻击范围内才设置isFollow为真
            {
                if (!isGuard)//加上这个判断防止出现站岗敌人原地鬼畜？
                {
                    isFollow = true;
                }
            }
            _waitPlayerExitTimer = WaitPlayerExitDuration;
            _enemyStatus = EnemyStatus.Chase;
        }
        else
        {
            isFollow = false;
            if (_waitPlayerExitTimer < 0)
            {
                if (isGuard)
                {
                    _enemyStatus = EnemyStatus.Guard;
                }
                else
                {
                    _enemyStatus = EnemyStatus.Patrol;
                }
            }
            else
            {
                if (isGuard)
                {
                    _enemyStatus = EnemyStatus.Guard;//不用担心马上回去站岗状态,因为在巡逻时会先判断是否等待时间结束
                }
                if (!isGuard)
                {
                    _enemyStatus = EnemyStatus.Patrol;//不用担心马上回去巡逻状态,因为在巡逻时会先判断是否等待时间结束
                }
                _waitPlayerExitTimer -= Time.deltaTime;
            }

        }



    }
    private void SwitchAnimation()//切换动画的方法
    {
        _anim.SetBool("walk", isWalk);
        _anim.SetBool("chase", isChase);
        _anim.SetBool("follow", isFollow);

    }
    private void FindPlayer()//发现玩家的方法
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, CheckPlayerDistance);
        Transform closestPlayer = null;
        float closestDistanceSqr = Mathf.Infinity;
        foreach (Collider collider in colliders)
        {
            
            if (collider.CompareTag("Player"))
            {
                Vector3 directionToPlayer = collider.transform.position - transform.position;
                float angle = Vector3.Angle(transform.forward, directionToPlayer);

                if (angle < CheckPlayerAngle / 2f)
                {
                    float dSqrToPlayer = directionToPlayer.sqrMagnitude;
                    if (dSqrToPlayer < closestDistanceSqr)
                    {
                        closestDistanceSqr = dSqrToPlayer;
                        closestPlayer = collider.transform;
                    }
                }
            }

        }
        if(closestPlayer)
        {
            Target = closestPlayer.gameObject;
            isFindPlayer = true;
        }
        else
        {
            Target = null;
            isFindPlayer = false;

        }
    }


    private void Guard()//守卫的方法,主要是丢失敌人后回守卫点
    {

        if (_waitPlayerExitTimer < 0)
        {
            isChase = false;
            Vector2 direction = _guardPos - transform.position;//保证回站岗点的时候也能跟着翻转

            transform.position = Vector3.MoveTowards(transform.position, _guardPos, PatrolSpeed * Time.deltaTime);
            transform.LookAt(_guardPos);

            if (Vector2.Distance(transform.position, _guardPos) > 0.1f)
            {
                isWalk = true;
            }
            else
            {
                isWalk = false;
            }
        }

    }
    private void Patrol()//巡逻方法
    {
        if (_waitPlayerExitTimer < 0)
        {
            isChase = false;
            if (Vector3.Distance(transform.position, PatolPoints[_wayPointIndex].position) > 0.5f)
            {
                isWalk = true;
                _agent.speed = PatrolSpeed;
                _agent.stoppingDistance = ChaseStopDistance;
                _agent.SetDestination(PatolPoints[_wayPointIndex].position);
            }
            else
            {
                isWalk = false;
                _patrolStayTimer -= Time.deltaTime;
                if (_patrolStayTimer < 0)
                {
                    _wayPointIndex = (_wayPointIndex + 1) % PatolPoints.Length;
                    _patrolStayTimer = PatrolStayDuration;
                }
            }
        }

    }
    private void Chase()//追击方法
    {

        if (!isGuard)
        {
            if (Vector2.Distance(transform.position, Target.transform.position) >= ChaseStopDistance)//大于技能攻击范围才靠近,除非特殊类型，不要设置敌人自己往玩家身上去的傻瓜式敌人
            {
                isWalk = false;
                isChase = true;
                _agent.speed = ChaseSpeed;
                _agent.stoppingDistance = ChaseStopDistance;
                _agent.SetDestination(Target.transform.position);
                //transform.LookAt(Target.transform.position);
            }
            else
            {
                isChase = true;
                isFollow = false;
                Attack();
            }
        }
        else
        {
            isWalk = false;
            isChase = true;
            Attack();
        }

    }
    private void Dead()//敌人死亡方法
    {
        _coll.enabled = false;//关闭碰撞体
        _anim.SetBool("death", true);
    }
    private void DestoryEnemy()//Animation Event 死亡动画最后一帧销毁敌人
    {
        Destroy(gameObject);
    }
    private void Attack()//“设置”攻击的方法
    {
        if (_attackCoolDownTimer < 0)
        {
            _anim.SetTrigger("attack");
            _attackCoolDownTimer = AttackCoolDown;
            //if (TargetInAttackRange())
            //{
            //    _anim.SetTrigger("attack");
            //    _attackCoolDownTimer = AttackCoolDown;
            //}
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, CheckPlayerDistance);

        Vector3 viewAngleA = Quaternion.AngleAxis(CheckPlayerAngle / 2, transform.up) * transform.forward;//扇形视野射线初始角度
        Vector3 viewAngleB = Quaternion.AngleAxis(-CheckPlayerAngle / 2, transform.up) * transform.forward;//扇形视野射线终止角度

        
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * CheckPlayerDistance);//起点，方向*长度
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * CheckPlayerDistance);//同上
    }

}
