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
    private Coroutine _currentCoroutine; // 当前正在执行的协程

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

    private void Awake()
    {
        _agent = gameObject.GetComponent<NavMeshAgent>();
    }

    private void Start()
    {


        GameManager.Instance.GameStartedAction += AIBotAction;

        SceneManager.Instance.RegisterAIBot(this);

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

        CurrentStateIndex = 0;
        CurrentState = StateList[CurrentStateIndex];
        WaitPoints = FindObjectsOfType<BotWaitPoint>();
        PrecomputeRandomDirection();
    }

    private void OnDisable()
    {
        GameManager.Instance.GameStartedAction -= AIBotAction;
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

    /// <summary>
    /// AIBot行为
    /// </summary>
    private void AIBotAction()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }

        switch (CurrentState)
        {
            case BotState.Move:
                _currentCoroutine = StartCoroutine(MoveRoutine());
                break;
            case BotState.TakeParts:
                _currentCoroutine = StartCoroutine(TakePartRoutine());
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// AIBot的状态切换
    /// </summary>

    private void SwitchState()
    {
        CurrentStateIndex = (CurrentStateIndex + 1) % StateList.Count;
        CurrentState = StateList[CurrentStateIndex];
        AIBotAction();
    }


    /// <summary>
    /// 随机移动携程
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveRoutine()
    {
        float moveTimer = 0f;
        Vector3 targetPosition = transform.position + _randomDir * MoveSpeed * SingleMoveDuration;
        _agent.SetDestination(targetPosition);
        while (moveTimer < SingleMoveDuration)
        {
            //_agent.SetDestination(targetPosition);
            moveTimer += Time.deltaTime;
            yield return null;
        }

        _agent.isStopped = true;
        yield return new WaitForSeconds(SingleStopDuration);
        _agent.isStopped = false;
        PrecomputeRandomDirection(); // 移动结束后重新计算随机方向
        SwitchState();
    }

    /// <summary>
    /// 去拿零件携程
    /// </summary>
    /// <returns></returns>
    private IEnumerator TakePartRoutine()
    {
        do
        {
            int randomPartIndex = Random.Range(0, WaitPoints.Length);

            TargetPoint = WaitPoints[randomPartIndex];

        } while (TargetPoint.HasBotExit);

        TargetPoint.HasBotExit = true;

        Vector3 targetPosition = new Vector3(TargetPoint.transform.position.x, transform.position.y, TargetPoint.transform.position.z);

        _agent.SetDestination(targetPosition);

        while (_agent.pathPending || _agent.remainingDistance > _agent.stoppingDistance)
        {
            yield return null;
        }

        _agent.isStopped = true;

        print("到达位置，等待零件");


    }

    /// <summary>
    /// 获得零件之后的响应
    /// </summary>
    public void GetPart(Part part)
    {
        CurrentPart = part;

        TargetPoint.HasBotExit = false;

        StartCoroutine(GetPartRoutine());
    }

    private IEnumerator GetPartRoutine()
    {
        _agent.isStopped = false;

        Vector3 endCenterPoint = FindObjectOfType<SubmissionPoint>().transform.position;

        Vector3 targetPosition = new Vector3(endCenterPoint.x + Random.Range(-2, 2), transform.position.y, endCenterPoint.z + Random.Range(-2, 2));

        _agent.SetDestination(targetPosition);

        while (_agent.pathPending || _agent.remainingDistance > _agent.stoppingDistance)
        {
            yield return null;
        }
        yield return new WaitForSeconds(StopAtSubmissionDuration);

        CurrentPart = null;


        SwitchState();
    }

    public void ExecuteQTE()
    {
        if (!IsBeingQTE) // 检查是否已经在进行QTE
        {
            IsBeingQTE = true;
            _agent.isStopped = true;

            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
            }

            _currentCoroutine = StartCoroutine(QTERoutine());
        }
    }

    private IEnumerator QTERoutine()
    {
        float QTETimer = 0;
        float QTEMaxTime = GetComponent<BotProperty>().QTEMaxTime;
        float QTETime = Random.Range(0, QTEMaxTime); // 随机选取一个0到最大响应时间的时间点 响应QTE

        while (QTETimer < QTETime)
        {
            QTETimer += Time.deltaTime;
            yield return null;
        }
        //执行QTE效果
        Debug.Log("QTE");
        while (QTETimer < QTEMaxTime)
        {
            QTETimer += Time.deltaTime;
            yield return null;
        }
        Debug.Log("QTEOver");

        // QTE事件完成
        IsBeingQTE = false;

        _agent.isStopped = false;
        // 结束QTE就开始动

        _currentCoroutine = null;

        AIBotAction();
    }

    public void Dead()
    {
        SceneManager.Instance.RemoveAIBot(this);
    }


    private Vector3 GetRandomIntermediatePoint(Vector3 targetPosition)
    {
        Vector3 midPoint = (transform.position + targetPosition) / 2;
        float randomOffset = Random.Range(-5f, 5f);
        Vector3 randomPoint = new Vector3(midPoint.x + randomOffset, midPoint.y, midPoint.z + randomOffset);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return targetPosition;
    }

}
