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
    
    
    void Awake()
    {
        _playerInput = gameObject.GetComponent<PlayerInput>();
        _multiplayerEventSystem =gameObject.GetComponent<MultiplayerEventSystem>();
        _inputSetting = GetComponent<InputSetting>();
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        Debug.Log(_inputSetting.inputDir);
    }

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
