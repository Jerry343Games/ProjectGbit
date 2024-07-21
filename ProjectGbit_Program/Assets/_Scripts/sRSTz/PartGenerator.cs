using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ����������������Զ������ɵ��λ���������������ͨ�����÷�������һ���������ض����
/// </summary>
public class PartGenerator : MonoBehaviour
{
    public GameObject[] parts; // ���Ԥ��������
    public Transform spawnPoint; // ���ɵ�
    public float spawnInterval = 2.0f; // ���ɼ��

    private GameObject nextPart; // ��һ�����ɵ��ض����
    private float timer; // ��ʱ��

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
    /// ����������������ǰָ��������ָ���ģ�û�о����ѡһ��
    /// </summary>
    void GeneratePart()
    {
        GameObject partToSpawn = nextPart != null ? nextPart : parts[Random.Range(0, parts.Length)];
        Instantiate(partToSpawn, spawnPoint.position, Quaternion.identity);
        nextPart = null;
    }

    /// <summary>
    /// ������һ�����ɵ����
    /// </summary>
    /// <param name="part">Ҫ���ɵ��ض����</param>
    public void SetNextPart(GameObject part)
    {
        nextPart = part;
    }
}

