using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class SceneLoader : Singleton<SceneLoader>
{
    public RawImage BlackScreen;

    public float FadeDuration = 0.5f;

    private void Start()
    {
        BlackScreen.GetComponent<RawImage>().color = new Color(0, 0, 0, 1);

        Sequence s = DOTween.Sequence();

        Tween t1 = BlackScreen.DOFade(0, FadeDuration);


        s.Join(t1);


    }

    public void LoadScene(string sceneName)
    {
        BlackScreen.GetComponent<RawImage>().color = new Color(0, 0, 0, 0);

        Sequence s = DOTween.Sequence();

        Tween t1 = BlackScreen.DOFade(1, FadeDuration).OnComplete(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        });


        s.Join(t1);
    }
}