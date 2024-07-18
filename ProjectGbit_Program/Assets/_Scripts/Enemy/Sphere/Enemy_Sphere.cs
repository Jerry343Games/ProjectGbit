using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//����ö���г����е���״̬:վ�ڣ�Ѳ�ߣ�׷��������
public enum EnemyStatus 
{ 
    Guard, 
    Patol, 
    Chase, 
    Dead 
};




public class Enemy_Sphere : MonoBehaviour
{
    //������˱��ּ��(��಻ͬҪ���ض��ĵ��˽ű��ж���д�� UI��������ʾ

    private Rigidbody2D _rb;
    private Animator _anim;
    private Collider _coll;



    private EnemyStatus _enemyStatus;

    public GameObject Target;//���Ŀ��

    [Header("״̬���")]
    public int MaxHealth;
    private int _currentHealth;



    [Header("��Ұ���")]
    public float CheckPlayerDistance;//������Ұ�ҳ�
    public float CheckPlayerAngle;//������Ұ�Ƕ�


    [Header("Ѳ�����")]
    public Transform[] PatolPoints;
    private Vector3 _guardPos;//��ʼվ�ڵ�
    private int _wayPointIndex = 0;//Ѳ�ߵ��ǣ�����ѭ��
    public float PatolSpeed;//Ѳ���ٶ�


    public float PatolStayDuration;//Ѳ��פ��۲��ʱ��
    private float _patolStayTimer;//Ѳ��פ��۲�ļ�ʱ��


    [Header("׷�����")]
    public float WaitPlayerExitDuration;//��ʧ��Һ�ԭ��ͣ����ʱ��
    private float _waitPlayerExitTimer;//��ʧ��Һ�ԭ��ͣ��ʱ��ļ�ʱ��
    public float ChaseSpeed;//׷���ٶ�
    public float ChaseStopDistance;
    [Header("�������")]
    public float AttackCoolDown;
    private float _attackCoolDownTimer;


    [Header("״̬���")]

    //����״̬
    [SerializeField] bool isGuard;//�Ƿ���վ���͵���
    [SerializeField] bool isFindPlayer;//�Ƿ������
    [SerializeField] bool isWalk;//�Ƿ�����
    [SerializeField] bool isDead;
    [SerializeField] bool isChase;//
    [SerializeField] bool isFollow;//�Ƿ�׷���ڶ�������������׷����ս���������л�
    [SerializeField] bool isCritical;//�Ƿ񱩻�
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _coll = GetComponent<Collider>();
        _anim = GetComponent<Animator>();
    }
    private void Start()
    {
        _guardPos = transform.position;
        _waitPlayerExitTimer = PatolStayDuration;
        _attackCoolDownTimer = AttackCoolDown;
        if (!isGuard)//��ʼ���ж�վ���ͻ���Ѳ���͵���
        {
            _enemyStatus = EnemyStatus.Patol;
            _patolStayTimer = PatolStayDuration;
        }
        else
        {
            _enemyStatus = EnemyStatus.Guard;
        }


        _currentHealth = MaxHealth;
    }

    private void Update()
    {
        if (_currentHealth == 0)
        {
            isDead = true;
            _enemyStatus = EnemyStatus.Dead;
        }
        if (!isDead)
        {
            FindPlayer();
            SwitchStatus();
            //SwitchAnimation();
        }
        switch (_enemyStatus)
        {
            case EnemyStatus.Guard:
                Guard();
                break;
            case EnemyStatus.Patol:
                Patol();
                break;
            case EnemyStatus.Chase:
                Chase();
                break;
            case EnemyStatus.Dead:
                Dead();
                break;

        }

        if (_attackCoolDownTimer > -1f)//���ø���Χ����ֹһֱ�������ܹ�������
        {
            _attackCoolDownTimer -= Time.deltaTime;
        }
    }
    private void SwitchStatus()//����״̬ת��ķ���
    {


        if (isFindPlayer)
        {
            if (Vector2.Distance(transform.position, Target.transform.position) > ChaseStopDistance)//��Ҫ�жϵ��˲��ڹ�����Χ�ڲ�����isFollowΪ��
            {
                if (!isGuard)//��������жϷ�ֹ����վ�ڵ���ԭ�ع���
                {
                    isFollow = true;
                }
            }
            _waitPlayerExitTimer = WaitPlayerExitDuration;
            _enemyStatus = EnemyStatus.Chase;
        }
        else
        {
            isFollow = false;
            if (_waitPlayerExitTimer < 0)
            {
                if (isGuard)
                {
                    _enemyStatus = EnemyStatus.Guard;
                }
                else
                {
                    _enemyStatus = EnemyStatus.Patol;
                }
            }
            else
            {
                if (isGuard)
                {
                    _enemyStatus = EnemyStatus.Guard;//���õ������ϻ�ȥվ��״̬,��Ϊ��Ѳ��ʱ�����ж��Ƿ�ȴ�ʱ�����
                }
                if (!isGuard)
                {
                    _enemyStatus = EnemyStatus.Patol;//���õ������ϻ�ȥѲ��״̬,��Ϊ��Ѳ��ʱ�����ж��Ƿ�ȴ�ʱ�����
                }
                _waitPlayerExitTimer -= Time.deltaTime;
            }

        }



    }
    private void SwitchAnimation()//�л������ķ���
    {
        _anim.SetBool("walk", isWalk);
        _anim.SetBool("chase", isChase);
        _anim.SetBool("follow", isFollow);

    }
    private void FindPlayer()//������ҵķ���
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, CheckPlayerDistance);
        Transform closestPlayer = null;
        float closestDistanceSqr = Mathf.Infinity;
        foreach (Collider collider in colliders)
        {
            
            if (collider.CompareTag("Player"))
            {
                Vector3 directionToPlayer = collider.transform.position - transform.position;
                float angle = Vector3.Angle(transform.forward, directionToPlayer);

                if (angle < CheckPlayerAngle / 2f)
                {
                    float dSqrToPlayer = directionToPlayer.sqrMagnitude;
                    if (dSqrToPlayer < closestDistanceSqr)
                    {
                        closestDistanceSqr = dSqrToPlayer;
                        closestPlayer = collider.transform;
                    }
                }
            }

        }
        if(closestPlayer)
        {
            Target = closestPlayer.gameObject;
            isFindPlayer = true;
        }
        else
        {
            Target = null;
            isFindPlayer = false;

        }
    }


    private void Guard()//�����ķ���,��Ҫ�Ƕ�ʧ���˺��������
    {

        if (_waitPlayerExitTimer < 0)
        {
            isChase = false;
            Vector2 direction = _guardPos - transform.position;//��֤��վ�ڵ��ʱ��Ҳ�ܸ��ŷ�ת

            transform.position = Vector3.MoveTowards(transform.position, _guardPos, PatolSpeed * Time.deltaTime);
            transform.LookAt(_guardPos);

            if (Vector2.Distance(transform.position, _guardPos) > 0.1f)
            {
                isWalk = true;
            }
            else
            {
                isWalk = false;
            }
        }

    }
    private void Patol()//Ѳ�߷���
    {
        if (_waitPlayerExitTimer < 0)
        {
            isChase = false;
            if (Vector2.Distance(transform.position, PatolPoints[_wayPointIndex].position) > 0.1f)
            {
                isWalk = true;
                transform.position = Vector3.MoveTowards(transform.position, PatolPoints[_wayPointIndex].position, PatolSpeed * Time.deltaTime);
                transform.LookAt(PatolPoints[_wayPointIndex].position);

            }
            else
            {
                isWalk = false;
                _patolStayTimer -= Time.deltaTime;
                if (_patolStayTimer < 0)
                {
                    _wayPointIndex = (_wayPointIndex + 1) % PatolPoints.Length;
                    _patolStayTimer = PatolStayDuration;
                }
            }
        }

    }
    private void Chase()//׷������
    {

        if (!isGuard)
        {
            if (Vector2.Distance(transform.position, Target.transform.position) >= ChaseStopDistance)//���ڼ��ܹ�����Χ�ſ���,�����������ͣ���Ҫ���õ����Լ����������ȥ��ɵ��ʽ����
            {
                isWalk = false;
                isChase = true;
                transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, ChaseSpeed * Time.deltaTime);//�ƶ������λ�õķ��� 
                transform.LookAt(Target.transform.position);
            }
            else
            {
                isChase = true;
                isFollow = false;
                Attack();
            }
        }
        else
        {
            isWalk = false;
            isChase = true;
            Attack();
        }

    }
    private void Dead()//������������
    {
        _coll.enabled = false;//�ر���ײ��
        _anim.SetBool("death", true);
    }
    private void DestoryEnemy()//Animation Event �����������һ֡���ٵ���
    {
        Destroy(gameObject);
    }
    private void Attack()//�����á������ķ���
    {
        if (_attackCoolDownTimer < 0)
        {
            _anim.SetTrigger("attack");
            _attackCoolDownTimer = AttackCoolDown;
            //if (TargetInAttackRange())
            //{
            //    _anim.SetTrigger("attack");
            //    _attackCoolDownTimer = AttackCoolDown;
            //}
        }

    }



}