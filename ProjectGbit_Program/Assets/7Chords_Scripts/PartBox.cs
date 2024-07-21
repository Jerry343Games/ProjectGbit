using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartBox : MonoBehaviour
{

    public int PartTypeAmount;

    public List<GameObject> PartPrefabs;
    public GameObject GetRandomPart()
    {
        int randomNum = Random.Range(0, PartPrefabs.Count);

        return PartPrefabs[randomNum];

    }


}
