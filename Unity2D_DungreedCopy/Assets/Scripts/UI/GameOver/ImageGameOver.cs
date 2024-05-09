using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageGameOver : MonoBehaviour
{
    private float   duration = 1.0f;    // �� ���濡 �ɸ��� �ð�
    
    // image�� A���� �����ϱ� ���� ����
    private float   startAlpha;         // ���� A��
    private float   targetAlpha = 1.0f; // ���� A��
    private bool    isLerping = false;  // ���� ������ ���� Ȯ��

    // image�� ��ġ���� �����ϱ� ���� ����
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

        startY      = thisRect.anchoredPosition.y;   // ���� Y��ǥ ����
    }

    // ������Ʈ�� Ȱ��ȭ �Ǹ�
    private void OnEnable()
    {
        startAlpha = thisImage.color.a; // ���� A�� ����
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

            // RectTransform�� PosY�� ����
            thisRect.anchoredPosition = new Vector2(thisRect.anchoredPosition.x, posY);

            if(elapsed >= dura)
            {
                panelGameOver.SetActive(true);
            }

            yield return null;
        }
    }
}
