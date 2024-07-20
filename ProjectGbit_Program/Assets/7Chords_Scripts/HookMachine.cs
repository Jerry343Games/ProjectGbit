using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HookMachine : MonoBehaviour
{
    private Animator _anim;

    public PartBox PartBox;

    public GameObject PartObject;

    public GameObject GetPartParent;

    public float MinX;

    public float MaxX;


    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }
    private void Start()
    {
        Invoke("test", 2f);
    }

    private void test()
    {
        _anim.Play("Get");
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="PartBox")
        {
            Debug.Log("PartBox");

            PartObject = Instantiate(other.gameObject.GetComponent<PartBox>().GetRandomPart(),transform.position + new Vector3(0,-2.5f,0), Quaternion.identity);

            PartObject.transform.SetParent(GetPartParent.transform);

            Sequence s = DOTween.Sequence();

            s.Append(PartObject.transform.DOLocalMoveZ(-0.02f, 2.5f)).SetEase(Ease.InSine);
        }
    }


    public void GetPartOver()
    {
        Sequence s = DOTween.Sequence();
        s.Append(transform.DOLocalMoveX(Random.Range(MinX, MaxX), 1f));
        s.Append(transform.DOLocalMoveX(-0.02f, 1f)).OnComplete(() =>
        {
            PartObject.transform.SetParent(null);
            //µôÂä£¿£¿
            PartObject = null;
        });

    }



}
