using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// AI������״̬
public enum BotState
{
    Move, // ��ͨ�ƶ�
    TakeParts, // ��������ƶ����̶�����ǰ���ȴ����������������
    ExecuteCommand, // ִ�о��������
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

    [Header("QTE����")]
    public bool IsBeingQTE;

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

    //��ȡһ������ƶ�����
    private void PrecomputeRandomDirection()
    {
        _randomDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
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

        while (moveTimer < SingleMoveDuration)
        {
            _agent.Move(_randomDir * MoveSpeed * Time.deltaTime);
            moveTimer += Time.deltaTime;
            yield return null;
        }

        _agent.isStopped = true;
        yield return new WaitForSeconds(SingleStopDuration);
        _agent.isStopped = false;
        PrecomputeRandomDirection(); // �ƶ����������¼����������
        SwitchState();
    }

    private IEnumerator TakePartRoutine()
    {
        // ѡȡ���ϴ�����������λ��
        print("��ȡ���");
        // if�жϵ�ǰλ�ú����λ�õľ��� �ӽ�֮�����ȡGetPart
        // �ص����Ӻ�
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
        float QTETime = Random.Range(0, 5f); // ���ѡȡһ��0�������Ӧʱ���ʱ��� ��ӦQTE

        while (QTETimer < QTETime)
        {
            QTETimer += Time.deltaTime;
            yield return null;
        }

        // QTE�¼����
        IsBeingQTE = false;

        // ����QTE�Ϳ�ʼ��
        AIBotAction();
    }
}
