using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQTEbubble : MonoBehaviour
{
    public Image inner;
    public Image outer;

    [HideInInspector]
    public float duration;
    public GameObject myBot;
    private RectTransform _rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveBubble(myBot);
    }
    /*
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
        CompleteCountdown();
        // ȷ����������
    }*/
    

    private void MoveBubble(GameObject bot)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(bot.transform.position);
        _rectTransform.position = screenPos;
    }

    /// <summary>
    /// ��ɼ�ʱ
    /// </summary>
    private void CompleteCountdown()
    {
        myBot.GetComponent<BotProperty>().qteBubble = null;
        if (!myBot.GetComponent<BotProperty>().isAIBot)
            myBot.GetComponent<PlayerBot>().SubmitPart();


        Destroy(gameObject);
    }
}
