using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageGameOver : MonoBehaviour
{
    private float   duration = 1.0f;    // 값 변경에 걸리는 시간
    
    // image의 A값을 변경하기 위한 변수
    private float   startAlpha;         // 시작 A값
    private float   targetAlpha = 1.0f; // 최종 A값
    private bool    isLerping = false;  // 보간 중인지 여부 확인

    // image의 위치값을 변경하기 위한 변수
    private float           startY;
    private float           targetY = 425f;
    [SerializeField]
    private AnimationCurve  posCurve;

    [SerializeField]
    private GameObject      panelGameOver;

    private Image           thisImage;
    private RectTransform   thisRect;

    private void Awake()
    {
        this.gameObject.SetActive(false);

        thisImage   = GetComponent<Image>();
        thisRect    = GetComponent<RectTransform>();

        startY      = thisRect.anchoredPosition.y;   // 시작 Y좌표 설정
    }

    // 오브젝트가 활성화 되면
    private void OnEnable()
    {
        startAlpha = thisImage.color.a; // 시작 A값 설정
        isLerping = true;
        StartCoroutine("ChangeAlpha");

        thisRect.anchoredPosition = new Vector3(thisRect.anchoredPosition.x, startY);
        StartCoroutine("ChangeRectPosY");
    }

    private IEnumerator ChangeAlpha()
    {
        float elapsed = 0.0f;

        while(isLerping)
        {
            elapsed += Time.deltaTime;
            float t         = elapsed / duration;
            Color lerpColor = thisImage.color;

            lerpColor.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            thisImage.color = lerpColor;

            if(t >= duration)
            {
                isLerping = false;
                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator ChangeRectPosY()
    {
        yield return new WaitForSeconds(3.0f);

        float dura    = duration * 0.5f;
        float elapsed = 0.0f;
        
        while(elapsed < dura)
        {
            elapsed += Time.deltaTime;

            float t     = elapsed / dura;
            float posY  = Mathf.Lerp(startY, targetY, posCurve.Evaluate(t));

            // RectTransform의 PosY값 설정
            thisRect.anchoredPosition = new Vector2(thisRect.anchoredPosition.x, posY);

            if(elapsed >= dura)
            {
                panelGameOver.SetActive(true);
            }

            yield return null;
        }
    }
}
