using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// AI�����˽ű�
/// </summary>
public class AIBot : MonoBehaviour
{
    private NavMeshAgent _agent;

    private Rigidbody _rb;

    //[Header("��Ϊ����")]
    //public List<BotState> StateList = new List<BotState>();
    //public int CurrentStateIndex;
    //public BotState CurrentState;

    [Header("��ͨ�ƶ�����")]
    public float MoveSpeed; // �ƶ��ٶ�
    public float SingleMoveDuration; // �����ƶ�ʱ��
    public float SingleStopDuration; // �����ƶ���ֹͣʱ��
    private Vector3 _randomDir; // ������������

    [Header("��ȡ���߲���")]
    public BotWaitPoint[] WaitPoints;

    public PartType CurrentPart;

    public BotWaitPoint TargetPoint;

    public float StopAtSubmissionDuration;



    [Header("Ѳ�����")]
    public Transform[] PatolPoints;
    private Vector3 _guardPos;//��ʼվ�ڵ�
    public int _wayPointIndex = 0;//Ѳ�ߵ��ǣ�����ѭ��

    public float _patrolStayTimer;

    public float PatrolStayDuration;


    public BotProperty botProperty;


    private void Patrol()//Ѳ�߷���
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
    ///// ��ȡһ������ƶ����򣬲�ȷ�����ᳬ���߽�Ͷ�λ���ϰ�������
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
    ///// �����λ�ú͵�ǰλ��֮���Ƿ����ϰ���
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
            // ��ȡ��ǰ�ٶȵķ���
            Vector3 direction = _agent.velocity.normalized;

            // ���㳯���˶��������ת�Ƕ�
            Quaternion newRotation = Quaternion.LookRotation(direction);

            // Ӧ��ͻ�����ת
            transform.rotation = newRotation;
        }

    }





    /// <summary>
    /// ÿ���ƶ������һ����һ���ƶ���ֹͣ�ĳ���ʱ�� ��������
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
        Destroy(gameObject);
    }



}
