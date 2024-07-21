using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// AI机器人脚本
/// </summary>
public class AIBot : MonoBehaviour
{
    private NavMeshAgent _agent;

    private Rigidbody _rb;

    //[Header("行为参数")]
    //public List<BotState> StateList = new List<BotState>();
    //public int CurrentStateIndex;
    //public BotState CurrentState;

    [Header("普通移动参数")]
    public float MoveSpeed; // 移动速度
    public float SingleMoveDuration; // 单次移动时间
    public float SingleStopDuration; // 单次移动后停止时间
    private Vector3 _randomDir; // 缓存的随机方向

    [Header("获取道具参数")]
    public BotWaitPoint[] WaitPoints;

    public PartType CurrentPart;

    public BotWaitPoint TargetPoint;

    public float StopAtSubmissionDuration;



    [Header("巡逻相关")]
    public Transform[] PatolPoints;
    private Vector3 _guardPos;//初始站岗点
    public int _wayPointIndex = 0;//巡逻点标记，用以循环

    public float _patrolStayTimer;

    public float PatrolStayDuration;


    public BotProperty botProperty;


    private void Patrol()//巡逻方法
    {
        if (Vector3.Distance(transform.position, PatolPoints[_wayPointIndex].position) > 0.5f)
        {
            _agent.speed = MoveSpeed;
            _agent.SetDestination(PatolPoints[_wayPointIndex].position);
        }
        else
        {
            _patrolStayTimer -= Time.deltaTime;
            if (_patrolStayTimer < 0)
            {
                _wayPointIndex = (_wayPointIndex + 1) % PatolPoints.Length;
                _patrolStayTimer = PatrolStayDuration;
            }
        }
    }






    private void Awake()
    {
        _agent = gameObject.GetComponent<NavMeshAgent>();

        _rb = gameObject.GetComponent<Rigidbody>();
        botProperty = GetComponent<BotProperty>();
    }

    private void Start()
    {

        SceneManager.Instance.RegisterAIBot(this);

        _patrolStayTimer = PatrolStayDuration;

        //PrecomputeRandomDirection();

        StopAtSubmissionDuration = botProperty.detectionTimeThreshold;
    }


    ///// <summary>
    ///// 获取一个随机移动方向，并确保不会超出边界和定位在障碍物里面
    ///// </summary>
    //private void PrecomputeRandomDirection()
    //{

    //    Vector3 potentialDirection;
    //    Vector3 newPosition;

    //    do
    //    {
    //        potentialDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    //        newPosition = transform.position + potentialDirection * MoveSpeed * SingleMoveDuration;
    //    }
    //    while (!IsWithinBounds(newPosition) && !HasObstacleBetweenTwoPos(potentialDirection, Vector3.Distance(newPosition, transform.position)));

    //    _randomDir = potentialDirection;
    //}

    ///// <summary>
    ///// 检查新位置和当前位置之间是否有障碍物
    ///// </summary>
    //private bool HasObstacleBetweenTwoPos(Vector3 dir, float distance)
    //{
    //    return Physics.Raycast(transform.position, dir, distance, LayerMask.GetMask("Obstacle"));
    //}



    private void Update()
    {
        if (!GameManager.Instance.GameStarted || GameManager.Instance.GameFinished)
        {
            return;
        }
        Patrol();

        if (_agent.velocity != Vector3.zero)
        {
            // 获取当前速度的方向
            Vector3 direction = _agent.velocity.normalized;

            // 计算朝向运动方向的旋转角度
            Quaternion newRotation = Quaternion.LookRotation(direction);

            // 应用突变的旋转
            transform.rotation = newRotation;
        }

    }





    /// <summary>
    /// 每次移动后都随机一下下一次移动和停止的持续时间 更加拟人
    /// </summary>
    private void RandomMoveAndStopDuration()
    {
        SingleMoveDuration += Random.Range(-2, 2);
        SingleMoveDuration = Mathf.Clamp(SingleMoveDuration, 1, 7);

        SingleStopDuration += Random.Range(-2, 2);
        SingleStopDuration = Mathf.Clamp(SingleStopDuration, 1, 7);

    }


    public void ExecuteQTE()
    {
        float QTEMaxTime = GetComponent<BotProperty>().QTEMaxTime;
        float QTETime = Random.Range(0, QTEMaxTime); // 随机选取一个0到最大响应时间的时间点 响应QTE
        Invoke("QTE", QTETime);
    }

    public void QTE()
    {
        Debug.Log("AIQTE!!");
    }

    public void Dead()
    {
        SceneManager.Instance.RemoveAIBot(this);
        Destroy(gameObject);
    }



}
