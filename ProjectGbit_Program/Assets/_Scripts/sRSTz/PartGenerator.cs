using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 零件生成器，负责自动在生成点的位置生成零件，可以通过调用方法让下一个零件变成特定零件
/// </summary>
public class PartGenerator : MonoBehaviour
{
    public GameObject[] parts; // 零件预制体数组
    public Transform spawnPoint; // 生成点
    public float spawnInterval = 2.0f; // 生成间隔

    private GameObject nextPart; // 下一个生成的特定零件
    private float timer; // 计时器

    void Start()
    {
        timer = spawnInterval;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            GeneratePart();
            timer = spawnInterval;
        }
    }

    /// <summary>
    /// 生成零件，如果有提前指定就生成指定的，没有就随机选一个
    /// </summary>
    void GeneratePart()
    {
        GameObject partToSpawn = nextPart != null ? nextPart : parts[Random.Range(0, parts.Length)];
        Instantiate(partToSpawn, spawnPoint.position, Quaternion.identity);
        nextPart = null;
    }

    /// <summary>
    /// 设置下一个生成的零件
    /// </summary>
    /// <param name="part">要生成的特定零件</param>
    public void SetNextPart(GameObject part)
    {
        nextPart = part;
    }
}

