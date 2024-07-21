using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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


    [Header("技能零件消耗")]
    public int SwitchDirectionNeedPart;//转换方向需要的零件数量

    public int ConfirmOnOffNeedPart;//开关传送带需要的零件数量


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
    private float _pressSwitchTimer = 0;
    private float _pressConfirmTimer = 0;
    
    private void Awake()
    {
        _multiplayerEventSystem= navigateHost.GetComponent<MultiplayerEventSystem>();
        _inputSetting = navigateHost.GetComponent<InputSetting>();
        currentBlock.gameObject.GetComponent<BeltSurfaceSet>().BeSelected();
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
        
        _pressSwitchTimer -= Time.deltaTime; // 更新计时器
        if (_pressSwitchTimer <= 0) // 检查计时器是否超过间隔
        {
            if (_inputSetting.isPressSwitch)
            {
                //这里写点击后的操作

                //消耗零件
                PartTask task = GameManager.Instance.Tasks.Find(x => x.currentAmount >= SwitchDirectionNeedPart);
                if (task == null)
                {
                    return;
                }
                task.currentAmount -= SwitchDirectionNeedPart;
                ConveyorBelt conveyorBelt = currentBlock.GetComponent<ConveyorBelt>();
                //Debug.Log("change");
                conveyorBelt.ChangeReverse();
                _pressSwitchTimer = 0.5f; // 重置计时器
            }
            
        }
    }
    
    /// <summary>
    /// 开关传送带
    /// </summary>
    private void ConfirmOnOff()
    {
        _pressConfirmTimer -= Time.deltaTime; // 更新计时器
        if (_pressConfirmTimer <= 0f) // 检查计时器是否超过间隔
        {
            if (_inputSetting.isPressConfirm)
            {
                //这里写点击后执行的操作

                //消耗零件
                PartTask task = GameManager.Instance.Tasks.Find(x => x.currentAmount >= ConfirmOnOffNeedPart);
                if(task == null)
                {
                    return;
                }
                task.currentAmount -= ConfirmOnOffNeedPart;
                
                ConveyorBelt conveyorBelt = currentBlock.GetComponent<ConveyorBelt>();
                conveyorBelt.ChangeOnOff();
                _pressConfirmTimer = 0.5f; // 重置计时器
            }
            
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
                currentBlock.gameObject.GetComponent<BeltSurfaceSet>().ExitSelected();
                currentBlock.up.gameObject.GetComponent<BeltSurfaceSet>().BeSelected();
                
                currentBlock = currentBlock.up;
                _multiplayerEventSystem.SetSelectedGameObject(currentBlock.gameObject);
            }
        }
        else if (isDown)
        {
            if (currentBlock.down != null)
            {
                currentBlock.gameObject.GetComponent<BeltSurfaceSet>().ExitSelected();
                currentBlock.down.gameObject.GetComponent<BeltSurfaceSet>().BeSelected();
                currentBlock = currentBlock.down;
                _multiplayerEventSystem.SetSelectedGameObject(currentBlock.gameObject);
            }
        }
        else if (isLeft)
        {
            if (currentBlock.left != null)
            {
                currentBlock.gameObject.GetComponent<BeltSurfaceSet>().ExitSelected();
                currentBlock.left.gameObject.GetComponent<BeltSurfaceSet>().BeSelected();
                
                currentBlock = currentBlock.left;
                _multiplayerEventSystem.SetSelectedGameObject(currentBlock.gameObject);
            }
        }
        else if (isRight)
        {
            if (currentBlock.right != null)
            {
                currentBlock.gameObject.GetComponent<BeltSurfaceSet>().ExitSelected();
                currentBlock.right.gameObject.GetComponent<BeltSurfaceSet>().BeSelected();
                
                currentBlock = currentBlock.right;
                _multiplayerEventSystem.SetSelectedGameObject(currentBlock.gameObject);
            }
        }
    }
}
