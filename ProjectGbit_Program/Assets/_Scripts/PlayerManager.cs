using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerManager : MonoBehaviour
{
    public List<GameObject> players; // 分配你的玩家对象
    private PlayerInputManager _playerInputManager;
    private void OnEnable()
    {
        _playerInputManager = GetComponent<PlayerInputManager>();
        _playerInputManager.onPlayerJoined += OnPlayerJoined;
    }
    private void OnDisable()
    {
        _playerInputManager.onPlayerJoined -= OnPlayerJoined;
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        //Debug.Log(playerInput.devices[0]);
    }
}
