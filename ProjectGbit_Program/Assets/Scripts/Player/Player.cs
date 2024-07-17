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
    /// 输入组件
    /// </summary>
    private PlayerInput _playerInput;
    
    /// <summary>
    /// 在分配设备时根据先后顺序分配到的序号
    /// </summary>
    [HideInInspector]
    public int myIndex;

    /// <summary>
    /// 选人界面菜单首选项
    /// </summary>
    public GameObject firstSelectBtn;
    
    void Awake()
    {
        _playerInput = gameObject.GetComponent<PlayerInput>();
        _multiplayerEventSystem =gameObject.GetComponent<MultiplayerEventSystem>();
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }
    
    public void OnDeviceAssigned(int index)
    {
        Debug.Log($"设备 "+_playerInput.devices+ " 已分配给玩家 {gameObject.name}");
        // 在这里添加更多逻辑，如更新状态、UI等
        myIndex = index;
        _multiplayerEventSystem.firstSelectedGameObject = firstSelectBtn;
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
