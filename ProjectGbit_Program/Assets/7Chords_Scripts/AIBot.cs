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

    private Animator _anim;

    [Header("��ͨ�ƶ�����")]
    public float MoveSpeed; // �ƶ��ٶ�

    [Header("��ȡ���߲���")]

    public PartType CurrentPart;



    [Header("Ѳ�����")]
    public Transform[] PatolPoints;

    private Vector3 _guardPos;//��ʼվ�ڵ�

    public int _wayPointIndex = 0;//Ѳ�ߵ��ǣ�����ѭ��

    public float _patrolStayTimer;

    public float PatrolStayDuration;

    public BotProperty botProperty;

    public Transform targetPoint;

    public QTEUI qTEUI;

    private void Awake()
    {
        _agent = gameObject.GetComponent<NavMeshAgent>();

        _rb = gameObject.GetComponent<Rigidbody>();

        botProperty = GetComponent<BotProperty>();

        _anim = transform.GetChild(0).GetComponent<Animator>();

        qTEUI = GetComponentInChildren<QTEUI>();
    }

    private void Start()
    {

        SceneManager.Instance.RegisterAIBot(this);

        _patrolStayTimer = PatrolStayDuration;


        targetPoint = PatolPoints[0];
    }

    private void Patrol()//Ѳ�߷���
    {
        if (Vector3.Distance(transform.position, targetPoint.position) > 1.5f)
        {
            _agent.speed = MoveSpeed;
            _agent.SetDestination(targetPoint.position);
            //�����ƶ�
            _anim.SetBool("isRun", true);
        }
        else
        {
            _patrolStayTimer -= Time.deltaTime;
            //���Ŵ���
            _anim.SetBool("isRun", false);

            if (_patrolStayTimer < 0)
            {
                if (CurrentPart != PartType.Empty)
                {
                    targetPoint = FindNearestSubmissonPoint();
                }
                else
                {
                    _wayPointIndex = (_wayPointIndex + 1) % PatolPoints.Length;
                    targetPoint = PatolPoints[_wayPointIndex];
                }

                PatrolStayDuration += Random.Range(-1f, 1f);
                PatrolStayDuration = Mathf.Clamp(PatrolStayDuration, 1, 5);
                _patrolStayTimer = PatrolStayDuration;


            }
        }
    }

    public void GetPart(PartType type)
    {
        CurrentPart = type;
    }
    


    private Transform FindNearestSubmissonPoint()
    {
        GameObject[] submissionPoints = GameObject.FindGameObjectsWithTag("SubmissionPoint");

        if (submissionPoints.Length == 0) return null;

        Transform nearestTran = submissionPoints[0].transform;

        float nearestDis = Vector3.Distance(nearestTran.position, transform.position);



        foreach (var point in submissionPoints)
        {
            float curDis = Vector3.Distance(point.transform.position, transform.position);
            if (curDis < nearestDis)
            {
                nearestTran = point.transform;
                nearestDis = curDis;
            }
        }

        return nearestTran;
    }


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


    public void ExecuteQTE()
    {
        float QTEMaxTime = GetComponent<BotProperty>().QTEMaxTime;
        float QTETime = Random.Range(QTEMaxTime/2, QTEMaxTime); // ���ѡȡһ��0�������Ӧʱ���ʱ��� ��ӦQTE
        Invoke("QTE", QTETime);
    }

    public void QTE()
    {
        qTEUI.CreatBubble(gameObject);
    }

    public void Dead()
    {
        SceneManager.Instance.RemoveAIBot(this);

        Instantiate(Resources.Load<GameObject>("Prefab/Effect/BotDeadEffect"),transform.position,Quaternion.identity);
        GetComponentInChildren<BotGetPartTrigger>().SetEmpty();
        if (botProperty.muBubble != null)
            botProperty.muBubble.GetComponent<UICountdownBubble>().StopAndDeleteBubble();
        Destroy(botProperty.qteBubble);

        Destroy(gameObject);
    }



}
