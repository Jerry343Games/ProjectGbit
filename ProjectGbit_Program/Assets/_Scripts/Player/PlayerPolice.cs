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

    public UIOrderPanel uiOrderPanel;
    [Header("��������")]
    public float AttackCD;

    private float _attackTimer;
    public PlayerPoliceAttack policeAttackArea;

    public Animator myAnimator;
    [Header("���ʹ�����")]
    public bool isOnConveyBelt = false;
    public Vector3 conveyorVelocity;
    //���ڹ���������վ
    private bool _isAttack;


    [Header("���ʹ�����")]

    public Material ScanRangeMaterial;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        policeAttackArea = GetComponentInChildren<PlayerPoliceAttack>();
        //Invoke("CallQTE", 3f);
    }

    // Update is called once per frame
    // ����һ��������������ʼֵΪfalse
    private bool hasPressed = false;
    private float pressResetTime = 0.5f; // 0.5�������ʱ��
    private float pressTimer = 0f;
    void Update()
    {
        if (player._isPlayerSetup)
        {
            Movement(inputSetting.inputDir);
        }



        SwitchDirection();
        // ����Ƿ���Ҫ����hasPressed����
        ConfirmDirection();
        if (inputSetting.isPressScan)
        {
            StartScanPlayerBot();
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
        //��������
        if (inputDir!=Vector2.zero)
        {
            myAnimator.SetBool("isRun",true);
            //ת��
            Vector3 direction = new Vector3(inputDir.x, 0, inputDir.y);
            Vector3 targetPosition = transform.position +direction;
            transform.LookAt(new Vector3(targetPosition.x, transform.position.y, targetPosition.z));
        }
        else
        {
            myAnimator.SetBool("isRun",false);
        }

        if (!_isAttack)
        {
            Vector3 movement = new Vector3(inputDir.x, 0, inputDir.y).normalized * moveSpeed;
            // �ƶ����
            // �ƶ����
            if (!isOnConveyBelt)
            {
                _rigidbody.velocity = new Vector3(movement.x, _rigidbody.velocity.y, movement.z);
            }
            else
            {
                // Ӧ��������뷽��ʹ��ʹ��ٶ�
                Vector3 totalVelocity = new Vector3(movement.x, _rigidbody.velocity.y, movement.z) + conveyorVelocity * 10;
                _rigidbody.velocity = totalVelocity;
            }
        }
        else
        {
            _rigidbody.velocity = Vector3.zero;
        }
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
        if(_attackTimer > 0)
        {
            _attackTimer -= Time.deltaTime;
        }
    }


    /// <summary>
    /// ʹ��QTE����
    /// </summary>
    private void CallQTE()
    {
        Debug.Log("callQTE");
        if(_callQTECDTimer > 0)
        {
            return;
        }
        Camera.main.GetComponent<CameraShake>().ShakeCamera(0.3f,1f);
        uiOrderPanel.ShowPanel();
        Invoke(nameof(CallAIQte), uiOrderPanel.countDownMax);
        
        hasPressed = false;
    }
    public void CallAIQte()
    {
        foreach (var aiBot in SceneManager.Instance.AIBotList)
        {
            aiBot.GetComponent<AIBot>().ExecuteQTE();
        }
    }
    private float _pressConfirmTimer = 2;
    private void ConfirmDirection()
    {
        
        if (!GameManager.Instance.GameStarted) return;
        _pressConfirmTimer -= Time.deltaTime; // ���¼�ʱ��
        if (_pressConfirmTimer <= 0f) // ����ʱ���Ƿ񳬹����
        {

            if (inputSetting.isPressConfirm)
            {
                
                _pressConfirmTimer = CallQTECD; // ���ü�ʱ��
                Debug.Log("confirm");
                CallQTE();
                
            }
            
        }
    }
    private float _pressSwitchTimer = 0;
    private void SwitchDirection()
    {
        _pressSwitchTimer -= Time.deltaTime; 
        if (_pressSwitchTimer <= 0f) 
        {
            if (inputSetting.isPressSwitch)
            {

                Attack();
                _pressSwitchTimer = AttackCD; 
            }

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
            ScanRangeMaterial.SetFloat("_Alpha", 0);
            return;
        }
        ScanRangeMaterial.SetFloat("_Alpha", 0.05f);
        Collider[] colliders = Physics.OverlapSphere(transform.position, CheckPlayerBotDistance);
        ScanRangeMaterial.SetColor("_Color", Color.white);

        foreach (Collider collider in colliders)
        {

            if (collider.CompareTag("Player"))
            {
                Vector3 directionToPlayer = collider.transform.position - transform.position;
                float angle = Vector3.Angle(transform.forward, directionToPlayer);

                if (angle < CheckPlayerBotAngle / 2f)
                {
                    ScanRangeMaterial.SetColor("_Color", Color.red);

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
        myAnimator.SetTrigger("Attack");
        _isAttack = true;
        StartCoroutine(WaitToEndAttack(0.7f));
        policeAttackArea.AttackPlayer();
    }

    IEnumerator WaitToEndAttack(float duration)
    {
        yield return new WaitForSeconds(duration);
        _isAttack = false;
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
