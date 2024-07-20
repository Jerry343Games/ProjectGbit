using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSetting : MonoBehaviour
{
    private InputAction _inputAction;
    /// <summary>
    /// 输入方向
    /// </summary>
    [HideInInspector]
    public Vector2 inputDir;
    /// <summary>
    /// 用于接收InputAction返回的玩家输入数据,玩家每次输入会被Call一次
    /// </summary>
    /// <param name="value0">输入数据</param>
    public void OnMovement(InputAction.CallbackContext value0)
    {
        inputDir = value0.ReadValue<Vector2>();
    }

    [HideInInspector]
    public bool isPressConfirm;
    public void OnPressConfirm (InputAction.CallbackContext context)
    {
        // 检测按键被按下
        if (context.started)
        {
            isPressConfirm = true;
        }
        // 检测按键被抬起
        else if (context.canceled)
        {
            isPressConfirm = false;
        }
    }
    
    [HideInInspector]
    public bool isPressSwitch;
    public void OnPressSwitch(InputAction.CallbackContext context)
    {
        // 检测按键被按下
        if (context.started)
        {
            isPressSwitch = true;
        }
        // 检测按键被抬起
        else if (context.canceled)
        {
            isPressSwitch = false;
        }
    }
    
    [HideInInspector]
    public bool isPressScan;
    public void OnPressScan(InputAction.CallbackContext context)
    {
        // 检测按键被按下
        if (context.started)
        {
            isPressScan = true;
        }
        // 检测按键被抬起
        else if (context.canceled)
        {
            isPressScan = false;
        }
    }
}
