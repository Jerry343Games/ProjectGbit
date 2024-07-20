using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    private Animator _anim;

    public float SelectHookThreshold;


    private void Awake()
    {
        _anim = transform.GetChild(0).GetComponent<Animator>();
    }

    public void GetPart()
    {
        _anim.Play("Get");


    }


}
