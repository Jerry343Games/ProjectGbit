using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// AI机器人状态
public enum BotState
{
    Move, // 普通移动
    TakeParts, // 拿零件（移动到固定点面前，等待零件过来拿起来）
    ExecuteCommand, // 执行警察的命令
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

    [Header("QTE参数")]
    public bool IsBeingQTE;

    [Header("地图边界")]
    public Vector3 MinBounds; // 地图最小边界
    public Vector3 MaxBounds; // 地图最大边界

    private void OnEnable()
    {
        GameManager.Instance.GameStartedAction += AIBotAction;
    }

    private void Awake()
    {
        _agent = gameObject.GetComponent<NavMeshAgent>();
        PrecomputeRandomDirection();
    }

    private void Start()
    {
        CurrentStateIndex = 0;
        CurrentState = StateList[CurrentStateIndex];
    }

    // 获取一个随机移动方向，并确保不会超出边界
    private void PrecomputeRandomDirection()
    {
        Vector3 potentialDirection;
        Vector3 newPosition;

        do
        {
            potentialDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            newPosition = transform.position + potentialDirection * MoveSpeed * SingleMoveDuration;
        }
        while (!IsWithinBounds(newPosition));

        _randomDir = potentialDirection;
    }

    // 检查新位置是否在边界内
    private bool IsWithinBounds(Vector3 position)
    {
        return position.x > MinBounds.x && position.x < MaxBounds.x &&
               position.z > MinBounds.z && position.z < MaxBounds.z;
    }

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

    private void Update()
    {
        if (!GameManager.Instance.GameStarted)
        {
            return;
        }

        if (IsBeingQTE)
        {
            _agent.isStopped = true;
            return;
        }
    }

    private void SwitchState()
    {
        CurrentStateIndex = (CurrentStateIndex + 1) % StateList.Count;
        CurrentState = StateList[CurrentStateIndex];
        AIBotAction();
    }

    private IEnumerator MoveRoutine()
    {
        float moveTimer = 0f;
        Vector3 targetPosition = transform.position + _randomDir * MoveSpeed * SingleMoveDuration;

        while (moveTimer < SingleMoveDuration)
        {
            _agent.SetDestination(targetPosition);
            moveTimer += Time.deltaTime;
            yield return null;
        }

        _agent.isStopped = true;
        yield return new WaitForSeconds(SingleStopDuration);
        _agent.isStopped = false;
        PrecomputeRandomDirection(); // 移动结束后重新计算随机方向
        SwitchState();
    }

    private IEnumerator TakePartRoutine()
    {
        // 选取场上存在随机零件的位置
        print("拿取零件");
        // if判断当前位置和零件位置的距离 接近之后就拿取GetPart
        // 回到框子后
        SwitchState();
        yield return null;
    }

    public void ExecuteQTE()
    {
        IsBeingQTE = true;

        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }

        _currentCoroutine = StartCoroutine(QTERoutine());
    }

    private IEnumerator QTERoutine()
    {
        float QTETimer = 0;
        float QTETime = Random.Range(0, 5f); // 随机选取一个0到最大响应时间的时间点 响应QTE

        while (QTETimer < QTETime)
        {
            QTETimer += Time.deltaTime;
            yield return null;
        }

        // QTE事件完成
        IsBeingQTE = false;

        // 结束QTE就开始动
        AIBotAction();
    }
}
