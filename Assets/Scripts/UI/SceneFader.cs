using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneFader : MonoBehaviour
{
    CanvasGroup canvasGroup;
    public float fadeInDuration;
    public float fadeOutDuration;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        DontDestroyOnLoad(gameObject);
    }
    public IEnumerator FadeOutIn()
    {
        yield return FadeIn(fadeInDuration);
        yield return FadeOut(fadeOutDuration);
    }
    public IEnumerator FadeOut(float time)
    {
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime / time;
            yield return null;
        }
        Destroy(gameObject);
    }
    public IEnumerator FadeIn(float time)
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / time;
            yield return null;
        }
    }
}
