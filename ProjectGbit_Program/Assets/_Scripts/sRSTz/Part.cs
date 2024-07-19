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
    //�����ǰ״̬���ܼ��𣬾ͷ���false
    public bool TryPick()
    {
        if (isPicked) return false;
        isPicked = true;
        model.SetActive(false);
        return true;
    }

    public void Use()
    {

    }
    public void Discard()
    {
        isPicked = false;
    }
}
