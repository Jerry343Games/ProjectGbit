using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

// AI������״̬
public enum BotState
{
    Move, // ��ͨ�ƶ�
    TakeParts, // ��������ƶ����̶�����ǰ���ȴ����������������
}

/// <summary>
/// AI�����˽ű�
/// </summary>
public class AIBot : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Coroutine _currentCoroutine; // ��ǰ����ִ�е�Э��

    [Header("��Ϊ����")]
    public List<BotState> StateList;
    public int CurrentStateIndex;
    public BotState CurrentState;

    [Header("��ͨ�ƶ�����")]
    public float MoveSpeed; // �ƶ��ٶ�
    public float SingleMoveDuration; // �����ƶ�ʱ��
    public float SingleStopDuration; // �����ƶ���ֹͣʱ��
    private Vector3 _randomDir; // ������������

    [Header("��ȡ���߲���")]
    public BotWaitPoint[] WaitPoints;

    [Header("QTE����")]
    public bool IsBeingQTE;

    [Header("��ͼ�߽�")]
    public Vector3 MinBounds; // ��ͼ��С�߽�
    public Vector3 MaxBounds; // ��ͼ���߽�

    private void Awake()
    {
        _agent = gameObject.GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        GameManager.Instance.GameStartedAction += AIBotAction;

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
    /// ��ȡһ������ƶ����򣬲�ȷ�����ᳬ���߽�Ͷ�λ���ϰ�������
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
    /// �����λ���Ƿ��ڱ߽���
    /// </summary>
    private bool IsWithinBounds(Vector3 position)
    {
        return position.x > MinBounds.x && position.x < MaxBounds.x &&
               position.z > MinBounds.z && position.z < MaxBounds.z;
    }

    /// <summary>
    /// �����λ�ú͵�ǰλ��֮���Ƿ����ϰ���
    /// </summary>
    private bool HasObstacleBetweenTwoPos(Vector3 dir, float distance)
    {
        return Physics.Raycast(transform.position, dir, distance, LayerMask.GetMask("Obstacle"));
    }

    /// <summary>
    /// AIBot��Ϊ
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
    /// AIBot��״̬�л�
    /// </summary>

    private void SwitchState()
    {
        CurrentStateIndex = (CurrentStateIndex + 1) % StateList.Count;
        CurrentState = StateList[CurrentStateIndex];
        AIBotAction();
    }


    /// <summary>
    /// ����ƶ�Я��
    /// </summary>
    /// <returns></returns>
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
        PrecomputeRandomDirection(); // �ƶ����������¼����������
        SwitchState();
    }

    /// <summary>
    /// ȥ�����Я��
    /// </summary>
    /// <returns></returns>
    private IEnumerator TakePartRoutine()
    {
        int randomPartIndex = Random.Range(0, WaitPoints.Length);
        Vector3 targetPosition = new Vector3(WaitPoints[randomPartIndex].transform.position.x,transform.position.y, WaitPoints[randomPartIndex].transform.position.z);

        float distance = Vector3.Distance(transform.position, targetPosition);
        float travelTime = distance / MoveSpeed;

        float moveTimer = 0f;
        while (moveTimer < travelTime)
        {
            _agent.SetDestination(targetPosition);
            moveTimer += Time.deltaTime;
            yield return null;
        }

        _agent.isStopped = true;

        print("����λ�ã��ȴ����");
    }


    /// <summary>
    /// ������֮�����Ӧ
    /// </summary>
    public void GetPart()
    {
        StartCoroutine(GetPartRoutine());
    }

    IEnumerator GetPartRoutine()
    {
        Vector3 targetPosition = FindObjectOfType<SubmissionPoint>().transform.GetChild(0).transform.position;

        Vector3 offsetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);

        float distance = Vector3.Distance(transform.position, offsetPosition);
        float travelTime = distance / MoveSpeed;

        float moveTimer = 0f;
        while (moveTimer < travelTime)
        {
            _agent.SetDestination(offsetPosition);
            moveTimer += Time.deltaTime;
            yield return null;
        }

        AIBotAction();
    }




    public void ExecuteQTE()
    {
        IsBeingQTE = true;
        _agent.isStopped = true;


        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }

        _currentCoroutine = StartCoroutine(QTERoutine());
    }

    private IEnumerator QTERoutine()
    {
        float QTETimer = 0;
        float QTEMaxTime = GetComponent<BotProperty>().QTEMaxTime;
        float QTETime = Random.Range(0, QTEMaxTime); // ���ѡȡһ��0�������Ӧʱ���ʱ��� ��ӦQTE

        while (QTETimer < QTETime)
        {
            QTETimer += Time.deltaTime;
            yield return null;
        }
        //ִ��QTEЧ��
        while (QTETimer < QTEMaxTime)
        {
            QTETimer += Time.deltaTime;
            yield return null;
        }

        // QTE�¼����
        IsBeingQTE = false;
        _agent.isStopped = false;
        // ����QTE�Ϳ�ʼ��
        AIBotAction();
    }
}
