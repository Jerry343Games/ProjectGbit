using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitToDestory : MonoBehaviour
{

    public float myLifeTime;
    void Start()
    {
        Invoke("DestoryMyself",myLifeTime);
    }
    private void DestoryMyself()
    {
        Destroy(gameObject);
    }
}
