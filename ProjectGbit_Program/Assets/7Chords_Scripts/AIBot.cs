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
    //private Coroutine _currentCoroutine; // ��ǰ����ִ�е�Э��

    private Rigidbody _rb;

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

        //���������Ϊ����
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

        //���õ�ǰ��Ϊ
        CurrentStateIndex = 0;
        CurrentState = StateList[CurrentStateIndex];

        //��ȡ�������е�����ȴ���
        WaitPoints = FindObjectsOfType<BotWaitPoint>();

        PrecomputeRandomDirection();
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
    /// AIBot��״̬�л�
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
    /// ������֮�����Ӧ
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
        float QTETime = Random.Range(0, QTEMaxTime); // ���ѡȡһ��0�������Ӧʱ���ʱ��� ��ӦQTE
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
