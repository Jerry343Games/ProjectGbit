using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuardAttackCountPanel : MonoBehaviour
{

    public List<Image> imgs = new List<Image> ();

    public void SetAttackImgs(int currentAttackCount)
    {
        for(int i = 0; i < imgs.Count; i++)
        {
            if(i< currentAttackCount)
            {
                imgs[i].gameObject.SetActive(true);
            }
            else
            {
                imgs[i].gameObject.SetActive(false);
            }
        }
    }
}
