using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartBox : MonoBehaviour
{

    public int PartTypeAmount;
    public GameObject GetRandomPart()
    {
        int randomNum = Random.Range(0, PartTypeAmount + 1);

        GameObject test = Resources.Load<GameObject>("Prefab/Scene/Obstacle");

        return test;


        //switch������
        //����
        //return null;
    }


}
