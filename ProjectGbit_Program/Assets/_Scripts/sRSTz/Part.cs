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
    

    public void Use()
    {

    }
    public void Discard()
    {
        isPicked = false;
    }
}
