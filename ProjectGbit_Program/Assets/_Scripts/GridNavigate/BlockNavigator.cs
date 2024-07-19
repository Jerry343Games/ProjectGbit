using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.UI;

public class BlockNavigator : MonoBehaviour
{
    public Block currentBlock;
    public GameObject navigateHost;

    private MultiplayerEventSystem _multiplayerEventSystem;
    private InputSetting _inputSetting;
    
    public Vector2 inputDir;
    public bool isUp;
    public bool isDown;
    public bool isLeft;
    public bool isRight;

    private float _navigateInputTimer;
    
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
        if (_navigateInputTimer >= 0.15f) // 检查计时器是否超过间隔
        {
            Navigate();
            _navigateInputTimer = 0f; // 重置计时器
        }
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
                Debug.Log("isUp");
            }
            else if (angle > 45f && angle <= 135f)
            {
                isLeft = true;
                Debug.Log("isRight");
            }
            else if (angle > -135f && angle <= -45f)
            {
                isRight = true;
                Debug.Log("isLeft");
            }
            else
            {
                isDown = true;
                Debug.Log("isDown");
            }
        }
    }

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

        Debug.Log(_multiplayerEventSystem.currentSelectedGameObject.name);
    }
}
