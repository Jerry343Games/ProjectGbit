using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UICountdownBubble : MonoBehaviour
{
    public Image inner;
    public Image outer;

    private float duration;
    public GameObject myBot;
    private RectTransform _rectTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        _rectTransform=GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveBubble(myBot);
    }

    public void ExcuteFillBar()
    {
        StartCoroutine(FillBar());
    }

    IEnumerator FillBar()
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            outer.fillAmount = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }
        outer.fillAmount = 1f;
        // 确保最终填满
    }

    private void MoveBubble(GameObject bot)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(bot.transform.position);
        _rectTransform.position = screenPos;
    }
    
}
