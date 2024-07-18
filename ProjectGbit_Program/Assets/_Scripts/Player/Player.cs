using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;

public class Player : MonoBehaviour
{
    /// <summary>
    /// 多人输入事件系统
    /// </summary>
    private MultiplayerEventSystem _multiplayerEventSystem;
    
    /// <summary>
    /// 输入系统
    /// </summary>
    private InputSetting _inputSetting;
    
    /// <summary>
    /// 输入组件
    /// </summary>
    private PlayerInput _playerInput;
    
    /// <summary>
    /// 玩家类型
    /// </summary>
    [HideInInspector]
    public PlayerType myType;
    
    /// <summary>
    /// 玩家模型组件
    /// </summary>
    public GameObject[] playerModel;
    
    /// <summary>
    /// 移动速度
    /// </summary>
    [Header("移动速度")]
    public float moveSpeed;
    
    /// <summary>
    /// 玩家刚体
    /// </summary>
    private Rigidbody _rigidbody;

    /// <summary>
    /// 玩家是否完成设置
    /// </summary>
    private bool _isPlayerSetup;
    void Awake()
    {
        _isPlayerSetup = false;
        _playerInput = gameObject.GetComponent<PlayerInput>();
        _multiplayerEventSystem =gameObject.GetComponent<MultiplayerEventSystem>();
        _inputSetting = GetComponent<InputSetting>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        if (_isPlayerSetup)
        {
            Movement(_inputSetting.inputDir);
        }
    }

    /// <summary>
    /// 玩家移动方法
    /// </summary>
    /// <param name="inputDir"></param>
    private void Movement(Vector2 inputDir)
    {
        Vector3 movement = new Vector3(inputDir.x, 0, inputDir.y).normalized * moveSpeed ;
        // 移动玩家
        _rigidbody.velocity = new Vector3(movement.x, _rigidbody.velocity.y, movement.z);
    }
    
    /// <summary>
    /// 外部调用初始化
    /// </summary>
    public void InitiatePlayer()
    {
        //由类型分配模型
        switch (myType)
        {
            case PlayerType.A:
                playerModel[0].SetActive(true);
                break;
            case PlayerType.B:
                playerModel[1].SetActive(true);
                break;
            case PlayerType.C:
                playerModel[2].SetActive(true);
                break;
            case PlayerType.D:
                playerModel[3].SetActive(true);
                break;
        }

        _isPlayerSetup = true;
    }
    
    public void OnPlayerJoined()
    {

        // 检测玩家使用的设备
        var controlDevice = _playerInput.devices[0];

        if (controlDevice is Gamepad)
        {
            Debug.Log("Player " + _playerInput.playerIndex + " is using a Gamepad."+" Control Device: " + controlDevice);
        }
        else if (controlDevice is Keyboard)
        {
            Debug.Log("Player " + _playerInput.playerIndex + " is using a Keyboard.");
        }
        
    }
    
}
