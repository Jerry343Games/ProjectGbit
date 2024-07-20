using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PartType
{
    Normal,
    type1,
    type2,
    type3
}
public class Part : MonoBehaviour
{
    public PartType partType;
    public bool isPicked;
    public GameObject model;
    public float destroyTime = 5f;
    private float destroyTimer;
    private void Awake()
    {
        destroyTimer = destroyTime;
    }
    private void Update()
    {
        destroyTimer -= Time.deltaTime;
        if (destroyTimer <= 0)
        {
            Destroy(this.gameObject);
        }
    }
    public void Use()
    {

    }
    public void Discard()
    {
        isPicked = false;
    }
}
