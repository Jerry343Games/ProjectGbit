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
    //如果当前状态不能捡起，就返回false
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
