using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPolice : MonoBehaviour
{
    public float moveSpeed;

    private Rigidbody _rigidbody;

    public InputSetting inputSetting;

    public Player player;

    [Header("��QTE�������")]
    public float CallQTECD;//��QTE�����cd

    private float _callQTECDTimer;

    [Header("ɨ�����")]
    public float ScanBotDuration;//ɨ�����ʱ��

    private float _scanBotTimer;

    public float ScanBotCD;//ɨ��CD

    private float _scanBotCDTimer;

    public float CheckPlayerBotDistance;//����ɨ�������ҳ�

    public float CheckPlayerBotAngle;//����ɨ������Ƕ�

    private bool _isBeingScan;

    private bool _canStartScan;

    [Header("��������")]
    public float AttackCD;

    private float _attackTimer;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player._isPlayerSetup)
        {
            Movement(inputSetting.inputDir);
        }
        if (inputSetting.isPressConfirm)
        {
            CallQTE();
        }
        if(inputSetting.isPressScan)
        {
            StartScanPlayerBot();
        }
        if(inputSetting.isPressSwitch)
        {
            Attack();
        }
        SkillCDUpdate();
        ScanPlayerBot();
    }

    /// <summary>
    /// ����ƶ�����
    /// </summary>
    /// <param name="inputDir"></param>
    private void Movement(Vector2 inputDir)
    {
        Vector3 movement = new Vector3(inputDir.x, 0, inputDir.y).normalized * moveSpeed;
        // �ƶ����
        _rigidbody.velocity = new Vector3(movement.x, _rigidbody.velocity.y, movement.z);
    }

    /// <summary>
    /// ����CD��ʱ��
    /// </summary>
    private void SkillCDUpdate()
    {
        //QTECD��ʱ
        if(_callQTECDTimer>0)
        {
            _callQTECDTimer -= Time.deltaTime;
        }
        //ɨ��ʱ���ʱ
        if (_scanBotTimer>0)
        {
            _scanBotTimer -= Time.deltaTime;
        }
        else
        {
            _isBeingScan = false;
        }
        //ɨ��CD��ʱ
        if (_scanBotCDTimer > 0)
        {
            _scanBotCDTimer -= Time.deltaTime;
        }
        else
        {
            _canStartScan = true;
        }
        //����CD��ʱ
        if(_attackTimer>0)
        {
            _attackTimer -= Time.deltaTime;
        }
    }


    /// <summary>
    /// ʹ��QTE����
    /// </summary>
    private void CallQTE()
    {
        if(_callQTECDTimer > 0)
        {
            return;
        }
        Debug.Log("���쿪ʼqte");

        foreach(var aiBot in SceneManager.Instance.AIBotList)
        {
            aiBot.GetComponent<AIBot>().ExecuteQTE();
        }
        foreach (var playerBot in SceneManager.Instance.PlayerBotList)
        {
            playerBot.GetComponent<PlayerBot>().BeginQTE();
        }
    }


    /// <summary>
    /// ��ʼɨ�������
    /// </summary>
    private void StartScanPlayerBot()
    {
        if(_canStartScan)
        {
            _isBeingScan = true;

            _canStartScan = false;

            _scanBotTimer = ScanBotDuration;

            _scanBotCDTimer = ScanBotCD;
        }
    }    

    /// <summary>
    /// ɨ������˼���
    /// </summary>
    private void ScanPlayerBot()
    {
        if(!_isBeingScan)
        {
            return;
        }


        Collider[] colliders = Physics.OverlapSphere(transform.position, CheckPlayerBotDistance);
        foreach (Collider collider in colliders)
        {

            if (collider.CompareTag("Player"))
            {
                Vector3 directionToPlayer = collider.transform.position - transform.position;
                float angle = Vector3.Angle(transform.forward, directionToPlayer);

                if (angle < CheckPlayerBotAngle / 2f)
                {
                    Debug.Log("ɨ��������");
                }
            }

        }
    }

    /// <summary>
    /// ���칥��
    /// </summary>
    private void Attack()
    {
        if(_attackTimer>0)
        {
            return;
        }
        Debug.Log("��ҷ����˹���");

    }

    /// <summary>
    /// �����߻���
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, CheckPlayerBotDistance);

        Vector3 viewAngleA = Quaternion.AngleAxis(CheckPlayerBotAngle / 2, transform.up) * transform.forward;//������Ұ���߳�ʼ�Ƕ�
        Vector3 viewAngleB = Quaternion.AngleAxis(-CheckPlayerBotAngle / 2, transform.up) * transform.forward;//������Ұ������ֹ�Ƕ�


        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * CheckPlayerBotDistance);//��㣬����*����
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * CheckPlayerBotDistance);//ͬ��
    }

}
