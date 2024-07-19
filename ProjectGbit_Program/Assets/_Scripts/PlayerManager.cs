using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerManager : MonoBehaviour
{
    // 私有静态实例变量
    private static PlayerManager _instance;

    // 公共静态属性用于访问实例
    public static PlayerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // 查找现有实例
                _instance = FindObjectOfType<PlayerManager>();

                // 如果实例不存在，则创建新的 GameObject 并添加 PlayerManager 组件
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<PlayerManager>();
                    singletonObject.name = typeof(PlayerManager).ToString() + " (Singleton)";
                }

                // 确保实例在场景之间持久化
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    // 私有构造函数，防止从外部实例化
    private PlayerManager() { }

    
    public List<GameObject> players; // 分配你的玩家对象
    private PlayerInputManager _playerInputManager;

    void Awake()
    {
        // 检查是否已有另一个实例，如果有则销毁新创建的实例
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        _playerInputManager = GetComponent<PlayerInputManager>();
        if (_playerInputManager != null)
        {
            _playerInputManager.onPlayerJoined += OnPlayerJoined;
        }
    }

    private void OnDisable()
    {
        if (_playerInputManager != null)
        {
            _playerInputManager.onPlayerJoined -= OnPlayerJoined;
        }
    }

    /// <summary>
    /// 计算最大玩家个数
    /// </summary>
    /// <returns></returns>
    public int MaxPlayerNumber()
    {
        return players.Count;
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        // Debug.Log(playerInput.devices[0]);
    }
}

