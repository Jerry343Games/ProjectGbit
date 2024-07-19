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
    /// 玩家是否完成设置
    /// </summary>
    [HideInInspector]
    public bool _isPlayerSetup;
    void Awake()
    {
        _isPlayerSetup = false;
        _playerInput = gameObject.GetComponent<PlayerInput>();
        _multiplayerEventSystem =gameObject.GetComponent<MultiplayerEventSystem>();
        _inputSetting = GetComponent<InputSetting>();
    }

    void Start()
    {
        
    }
    
    void Update()
    {

    }
    
    /// <summary>
    /// 外部调用初始化
    /// </summary>
    public void InitiatePlayer()
    {
        //由类型分配模型
        switch (myType)
        {
            case PlayerType.PlayerBot1:
                playerModel[0].SetActive(true);
                break;
            case PlayerType.PlayerBot2:
                playerModel[1].SetActive(true);
                break;
            case PlayerType.PlayerFactory:
                playerModel[2].SetActive(true);
                break;
            case PlayerType.PlayerPolice:
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
