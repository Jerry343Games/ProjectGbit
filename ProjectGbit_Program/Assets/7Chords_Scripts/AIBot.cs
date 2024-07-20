using DG.Tweening;
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

    public Part CurrentPart;

    public BotWaitPoint TargetPoint;

    public float StopAtSubmissionDuration;

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
        PrecomputeRandomDirection(); // �ƶ����������¼����������
        SwitchState();
    }

    /// <summary>
    /// ȥ�����Я��
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

        print("����λ�ã��ȴ����");


    }

    /// <summary>
    /// ������֮�����Ӧ
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
        if (!IsBeingQTE) // ����Ƿ��Ѿ��ڽ���QTE
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
        float QTETime = Random.Range(0, QTEMaxTime); // ���ѡȡһ��0�������Ӧʱ���ʱ��� ��ӦQTE

        while (QTETimer < QTETime)
        {
            QTETimer += Time.deltaTime;
            yield return null;
        }
        //ִ��QTEЧ��
        Debug.Log("QTE");
        while (QTETimer < QTEMaxTime)
        {
            QTETimer += Time.deltaTime;
            yield return null;
        }
        Debug.Log("QTEOver");

        // QTE�¼����
        IsBeingQTE = false;

        _agent.isStopped = false;
        // ����QTE�Ϳ�ʼ��

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
