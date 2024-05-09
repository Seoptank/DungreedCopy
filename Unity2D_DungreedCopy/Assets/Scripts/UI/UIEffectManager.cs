using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIEffectManager : MonoBehaviour
{
    static public UIEffectManager instance;

    private float limitTime = 1;

    [SerializeField]
    private AnimationCurve  activeCurve;

    [SerializeField]
    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    
    //========================================================================================
    // YS: StarteSetting 오버라이드 함수 
    //========================================================================================
    public void StarteSetting(SpriteRenderer newSprite)
    {
        Color color = newSprite.color;

        color.a = 0;

        newSprite.color = color;
    }
    public void StarteSetting(Image newImage)
    {
        Color color = newImage.color;

        color.a = 0;

        newImage.color = color;
    }
    public void StarteSetting(TextMeshProUGUI newTMP)
    {
        Color color = newTMP.color;

        color.a = 0;

        newTMP.color = color;
    }
    
    //========================================================================================
    // YS: UIFade 오버라이드 함수 
    //========================================================================================
    public IEnumerator UIFade(SpriteRenderer newSprite, float start, float end)
    {
        float curTime = 0;
        float percent = 0;

        while (percent < 1)
        {
            curTime += Time.deltaTime;
            percent = curTime / limitTime;

            Color color = newSprite.color;
            color.a = Mathf.Lerp(start, end, activeCurve.Evaluate(percent));
            newSprite.color = color;

            yield return null;
        }
    }
    public IEnumerator UIFade(SpriteRenderer newSprite, float start, float end,float time)
    {
        float curTime = 0;
        float percent = 0;

        while (percent < 1)
        {
            curTime += Time.deltaTime;
            percent = curTime / time;

            Color color = newSprite.color;
            color.a = Mathf.Lerp(start, end, activeCurve.Evaluate(percent));
            newSprite.color = color;

            yield return null;
        }
    }
    public IEnumerator UIFade(Image newImage, float start, float end)
    {
        float curTime = 0;
        float percent = 0;

        while (percent < 1)
        {
            curTime += Time.deltaTime;
            percent = curTime / limitTime;

            Color color = newImage.color;
            color.a = Mathf.Lerp(start, end, activeCurve.Evaluate(percent));
            newImage.color = color;

            yield return null;
        }
    }
    public IEnumerator UIFade(TextMeshProUGUI newTMP, float start, float end)
    {
        float curTime = 0;
        float percent = 0;

        while (percent < 1)
        {
            curTime += Time.deltaTime;
            percent = curTime / limitTime;

            Color color = newTMP.color;
            color.a = Mathf.Lerp(start, end, activeCurve.Evaluate(percent));
            newTMP.color = color;

            yield return null;
        }
    }
    
}
