using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PartType
{
    Empty,
    type1,
    type2,
    type3,
}
public class Part : MonoBehaviour
{
    public PartType partType;
    public bool isPicked;
    public GameObject model;
    public float destroyTime = 5f;
    public float destroyTimer;
    private void Awake()
    {
        destroyTimer = destroyTime;
    }

    private void Start()
    {
        SceneManager.Instance.RegisterPart(this);
    }
    private void Update()
    {
        destroyTimer -= Time.deltaTime;
        if (destroyTimer <= 0)
        {
            SceneManager.Instance.RemovePart(this);
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
