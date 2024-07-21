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

    private Animator _anim;

    [Header("普通移动参数")]
    public float MoveSpeed; // 移动速度

    [Header("获取道具参数")]

    public PartType CurrentPart;



    [Header("巡逻相关")]
    public Transform[] PatolPoints;

    private Vector3 _guardPos;//初始站岗点

    public int _wayPointIndex = 0;//巡逻点标记，用以循环

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

        qTEUI = FindFirstObjectByType<QTEUI>();
    }

    private void Start()
    {

        SceneManager.Instance.RegisterAIBot(this);

        _patrolStayTimer = PatrolStayDuration;


        targetPoint = PatolPoints[0];
    }

    private void Patrol()//巡逻方法
    {
        if (Vector3.Distance(transform.position, targetPoint.position) > 1.3f)
        {
            _agent.speed = MoveSpeed;
            _agent.SetDestination(targetPoint.position);
            //播放移动
            _anim.SetBool("isRun", true);
        }
        else
        {
            _patrolStayTimer -= Time.deltaTime;
            //播放待机
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
            // 获取当前速度的方向
            Vector3 direction = _agent.velocity.normalized;

            // 计算朝向运动方向的旋转角度
            Quaternion newRotation = Quaternion.LookRotation(direction);

            // 应用突变的旋转
            transform.rotation = newRotation;
        }

    }


    public void ExecuteQTE()
    {
        float QTEMaxTime = GetComponent<BotProperty>().QTEMaxTime;
        float QTETime = Random.Range(QTEMaxTime/2, QTEMaxTime); // 随机选取一个0到最大响应时间的时间点 响应QTE
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

        Destroy(gameObject);
    }



}
