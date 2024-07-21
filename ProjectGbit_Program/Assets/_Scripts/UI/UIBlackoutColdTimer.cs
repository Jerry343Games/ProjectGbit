using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIBlackoutColdTimer : MonoBehaviour
{
    
    public Image inner;
    public Image outer;

    public GameObject myBot;

    public float coldTime;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExcuteColdDown()
    {
        StartCoroutine(FillBar());
    }
    
    IEnumerator FillBar()
    {
        float elapsed = 0f;
        while (elapsed < coldTime)
        {
            elapsed += Time.deltaTime;
            outer.fillAmount = Mathf.Clamp01(elapsed / coldTime);
            yield return null;
        }
        outer.fillAmount = 1f;
        CompleteCountdown();
        // 确保最终填满
    }

    private void CompleteCountdown()
    {
        Destroy(gameObject);
    }
}
