using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBot : MonoBehaviour
{
    public float moveSpeed;

    private Rigidbody _rigidbody;

    public InputSetting inputSetting;

    public Player player;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player._isPlayerSetup)
        {
            Movement(inputSetting.inputDir);
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
        _rigidbody.velocity += new Vector3(movement.x, _rigidbody.velocity.y, movement.z)*Time.deltaTime*30;
    }
    
    /// <summary>
    /// 开始QTE
    /// </summary>
    public void BeginQTE()
    {
        StartCoroutine(QTERoutine());
    }

    IEnumerator QTERoutine()
    {
        float QTETimer = 0;
        float QTEMaxTime = GetComponent<BotProperty>().QTEMaxTime;

        while (QTETimer < QTEMaxTime)
        {
            //输入按键后结束
            QTETimer += Time.deltaTime;
            yield return null;
        }
        //未完成输入，暴露
    }

}
