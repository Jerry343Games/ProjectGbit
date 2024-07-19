using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPolice : MonoBehaviour
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
    /// ����ƶ�����
    /// </summary>
    /// <param name="inputDir"></param>
    private void Movement(Vector2 inputDir)
    {
        Vector3 movement = new Vector3(inputDir.x, 0, inputDir.y).normalized * moveSpeed;
        // �ƶ����
        _rigidbody.velocity = new Vector3(movement.x, _rigidbody.velocity.y, movement.z);
    }


    /// <summary>
    /// ʹ��QTE����
    /// </summary>
    private void CallQTE()
    {
        
    }

}