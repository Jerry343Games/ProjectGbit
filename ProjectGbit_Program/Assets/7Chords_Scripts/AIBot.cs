using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

// AI机器人状态
public enum BotState
{
    Move, // 普通移动
    TakeParts, // 拿零件（移动到固定点面前，等待零件过来拿起来）
}

/// <summary>
/// AI机器人脚本
/// </summary>
public class AIBot : MonoBehaviour
{
    private NavMeshAgent _agent;
    //private Coroutine _currentCoroutine; // 当前正在执行的协程

    private Rigidbody _rb;

    [Header("行为参数")]
    public List<BotState> StateList;
    public int CurrentStateIndex;
    public BotState CurrentState;

    [Header("普通移动参数")]
    public float MoveSpeed; // 移动速度
    public float SingleMoveDuration; // 单次移动时间
    public float SingleStopDuration; // 单次移动后停止时间
    private Vector3 _randomDir; // 缓存的随机方向

    [Header("获取道具参数")]
    public BotWaitPoint[] WaitPoints;

    public Part CurrentPart;

    public BotWaitPoint TargetPoint;

    public float StopAtSubmissionDuration;

    [Header("QTE参数")]
    public bool IsBeingQTE;

    [Header("地图边界")]
    public Vector3 MinBounds; // 地图最小边界
    public Vector3 MaxBounds; // 地图最大边界

    bool hasMoveOver = false;

    bool hasSetDest = false;

    bool hasSetPartDest = false;

    float moveTimer = 0;

    float waitTimer = 0;

    float QTEtimer = 0;

    bool hasQTEed = false;

    private void Awake()
    {
        _agent = gameObject.GetComponent<NavMeshAgent>();

        _rb = gameObject.GetComponent<Rigidbody>();
    }

    private void Start()
    {

        SceneManager.Instance.RegisterAIBot(this);

        //生成随机行为序列
        StateList.Clear();

        for (int i = 0; i < 5; i++)
        {
            int r = Random.Range(0, 2);
            if (r == 0)
            {
                StateList.Add(BotState.Move);
            }
            else
            {
                StateList.Add(BotState.TakeParts);
            }
        }

        //设置当前行为
        CurrentStateIndex = 0;
        CurrentState = StateList[CurrentStateIndex];

        //获取场上所有的零件等待点
        WaitPoints = FindObjectsOfType<BotWaitPoint>();

        PrecomputeRandomDirection();
    }


    /// <summary>
    /// 获取一个随机移动方向，并确保不会超出边界和定位在障碍物里面
    /// </summary>
    private void PrecomputeRandomDirection()
    {
        Vector3 potentialDirection;
        Vector3 newPosition;

        do
        {
            potentialDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            newPosition = transform.position + potentialDirection * MoveSpeed * SingleMoveDuration;
        }
        while (!IsWithinBounds(newPosition) && !HasObstacleBetweenTwoPos(potentialDirection, Vector3.Distance(newPosition, transform.position)));

        _randomDir = potentialDirection;
    }

    /// <summary>
    /// 检查新位置是否在边界内
    /// </summary>
    private bool IsWithinBounds(Vector3 position)
    {
        return position.x > MinBounds.x && position.x < MaxBounds.x &&
               position.z > MinBounds.z && position.z < MaxBounds.z;
    }

    /// <summary>
    /// 检查新位置和当前位置之间是否有障碍物
    /// </summary>
    private bool HasObstacleBetweenTwoPos(Vector3 dir, float distance)
    {
        return Physics.Raycast(transform.position, dir, distance, LayerMask.GetMask("Obstacle"));
    }



    private void Update()
    {
        if(!GameManager.Instance.GameStarted || GameManager.Instance.GameFinished)
        {
            return;
        }
        if(!IsBeingQTE)
        {
            switch (CurrentState)
            {
                case BotState.Move:
                    Move();
                    break;
                case BotState.TakeParts:
                    TakePart();
                    //_currentCoroutine = StartCoroutine(TakePartRoutine());
                    break;
                default:
                    break;
            }
        }
    }




    private void Move()
    {
 

        if(!hasMoveOver)
        {
            if (moveTimer < SingleMoveDuration)
            {
                if(!hasSetDest)
                {
                    hasSetDest = true;
                    Vector3 targetPosition = transform.position + _randomDir * MoveSpeed * SingleMoveDuration;
                    _agent.isStopped = false;
                    _agent.SetDestination(targetPosition);
                }
                moveTimer += Time.deltaTime;

            }
            else
            {
                hasMoveOver = true;
            }
        }
        else
        {
            if(waitTimer<SingleStopDuration)
            {
                waitTimer += Time.deltaTime;
                _agent.isStopped = true;
            }
            else
            {
                SwitchState();
            }
        }


    }


    private void TakePart()
    {

        if(!hasSetPartDest)
        {
            hasSetPartDest = true;
            do
            {
                int randomPartIndex = Random.Range(0, WaitPoints.Length);

                TargetPoint = WaitPoints[randomPartIndex];

            } while (TargetPoint.HasBotExit);

            TargetPoint.HasBotExit = true;

            _agent.isStopped = false;

            _agent.SetDestination(TargetPoint.transform.position);
        }

    }

    /// <summary>
    /// AIBot的状态切换
    /// </summary>

    private void SwitchState()
    {
        CurrentStateIndex = (CurrentStateIndex + 1) % StateList.Count;

        CurrentState = StateList[CurrentStateIndex];


        _agent.isStopped = true;

        hasMoveOver = false;

        hasSetDest = false;

        hasSetPartDest = false;

        moveTimer = 0;

        waitTimer = 0;

        QTEtimer = 0;

        hasQTEed = false;

        PrecomputeRandomDirection();
    }


    /// <summary>
    /// 获得零件之后的响应
    /// </summary>
    public void GetPart(Part part)
    {

        CurrentPart = part;

        TargetPoint.HasBotExit = false;

        _agent.isStopped = false;

        Vector3 endCenterPoint = FindObjectOfType<SubmissionPoint>().transform.position;

        Vector3 targetPosition = new Vector3(endCenterPoint.x + Random.Range(-2, 2), transform.position.y, endCenterPoint.z + Random.Range(-2, 2));

        _agent.SetDestination(targetPosition);

        StartCoroutine(test());

    }

    IEnumerator test()
    {
        while(Vector3.Distance(_agent.pathEndPosition,transform.position) > 0.1f)
        {
            yield return null;
        }

        CurrentPart = null;

        SwitchState();

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
    }



}
