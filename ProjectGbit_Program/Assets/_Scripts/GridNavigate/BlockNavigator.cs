using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.UI;

/// <summary>
/// 工厂的传送带导航和选取确定功能
/// </summary>
public class BlockNavigator : MonoBehaviour
{
    public Block currentBlock;
    public GameObject navigateHost;

    private MultiplayerEventSystem _multiplayerEventSystem;
    private InputSetting _inputSetting;
    
    [HideInInspector]
    public Vector2 inputDir;
    [HideInInspector]
    public bool isUp;
    [HideInInspector]
    public bool isDown;
    [HideInInspector]
    public bool isLeft;
    [HideInInspector]
    public bool isRight;

    private float _navigateInputTimer;
    private float _pressSwitchTimer;
    private float _pressConfirmTimer;
    
    private void Awake()
    {
        _multiplayerEventSystem= navigateHost.GetComponent<MultiplayerEventSystem>();
        _inputSetting = navigateHost.GetComponent<InputSetting>();
    }

    void Update()
    {
        HandleInput(_inputSetting.inputDir);
        
        //带有计时器的执行导航，连续间隔不低于0.2秒
        _navigateInputTimer += Time.deltaTime; // 更新计时器
        if (_navigateInputTimer >= 0.1f) // 检查计时器是否超过间隔
        {
            Navigate();
            _navigateInputTimer = 0f; // 重置计时器
        }
        
        ConfirmOnOff();
        SwitchDirection();
        
    }
    
    /// <summary>
    /// 拟合输入向量
    /// </summary>
    /// <param name="dir"></param>
    void HandleInput(Vector2 dir)
    {
        // Reset all direction booleans
        isUp = isDown = isLeft = isRight = false;

        if (dir.magnitude > 0.5f)
        {
            // Determine the primary direction based on the angle of the input vector
            float angle = Vector2.SignedAngle(Vector2.up, dir);

            if (angle > -45f && angle <= 45f)
            {
                isUp = true;
            }
            else if (angle > 45f && angle <= 135f)
            {
                isLeft = true;
            }
            else if (angle > -135f && angle <= -45f)
            {
                isRight = true;
            }
            else
            {
                isDown = true;
            }
        }
    }

    /// <summary>
    /// 切换传送带方向
    /// </summary>
    private void SwitchDirection()
    {
        _pressSwitchTimer += Time.deltaTime; // 更新计时器
        if (_pressSwitchTimer >= 0.1f) // 检查计时器是否超过间隔
        {
            if (_inputSetting.isPressSwitch)
            {
                //这里写点击后的操作
                currentBlock.gameObject.GetComponent<MeshRenderer>().material.color=Color.cyan;
            }
            _pressSwitchTimer = 0f; // 重置计时器
        }
    }
    
    /// <summary>
    /// 开关传送带
    /// </summary>
    private void ConfirmOnOff()
    {
        _pressConfirmTimer += Time.deltaTime; // 更新计时器
        if (_pressConfirmTimer >= 0.1f) // 检查计时器是否超过间隔
        {
            if (_inputSetting.isPressConfirm)
            {
                //这里写点击后执行的操作
                currentBlock.gameObject.GetComponent<MeshRenderer>().material.color=Color.blue;
            }
            _pressConfirmTimer = 0f; // 重置计时器
        }
    }

    /// <summary>
    /// 根据导航索引移动聚焦
    /// </summary>
    private void Navigate()
    {
        if (isUp)
        {
            if (currentBlock.up != null)
            {
                currentBlock.gameObject.GetComponent<MeshRenderer>().material.color=Color.white;
                currentBlock.up.gameObject.GetComponent<MeshRenderer>().material.color=Color.red;
                
                currentBlock = currentBlock.up;
                _multiplayerEventSystem.SetSelectedGameObject(currentBlock.gameObject);
            }
        }
        else if (isDown)
        {
            if (currentBlock.down != null)
            {
                currentBlock.gameObject.GetComponent<MeshRenderer>().material.color=Color.white;
                currentBlock.down.gameObject.GetComponent<MeshRenderer>().material.color=Color.red;
                
                currentBlock = currentBlock.down;
                _multiplayerEventSystem.SetSelectedGameObject(currentBlock.gameObject);
            }
        }
        else if (isLeft)
        {
            if (currentBlock.left != null)
            {
                currentBlock.gameObject.GetComponent<MeshRenderer>().material.color=Color.white;
                currentBlock.left.gameObject.GetComponent<MeshRenderer>().material.color=Color.red;
                
                currentBlock = currentBlock.left;
                _multiplayerEventSystem.SetSelectedGameObject(currentBlock.gameObject);
            }
        }
        else if (isRight)
        {
            if (currentBlock.right != null)
            {
                currentBlock.gameObject.GetComponent<MeshRenderer>().material.color=Color.white;
                currentBlock.right.gameObject.GetComponent<MeshRenderer>().material.color=Color.red;
                
                currentBlock = currentBlock.right;
                _multiplayerEventSystem.SetSelectedGameObject(currentBlock.gameObject);
            }
        }
    }
}
