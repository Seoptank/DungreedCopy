using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private string transferSceneName;

    [SerializeField]
    private TextMeshProUGUI textTime;
    [SerializeField]
    private TextMeshProUGUI textGold;
    [SerializeField]
    private TextMeshProUGUI textKill;
    [SerializeField]
    private TextMeshProUGUI textEnemyName;

    [SerializeField]
    private GameObject      button;
    [SerializeField]
    private GameObject      title;

    private void Awake()
    {
        this.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        PlayerDungeonData.instance.TimeChangeToText(textTime);

        textGold.text = PlayerStats.instance.GOLD + " G";

        textKill.text = PlayerDungeonData.instance.countKill + " ����";

        FadeInExceptSelf();

        StartCoroutine(OnButton());

        PlayerStats.instance.AddEXP(PlayerDungeonData.instance.totalEXP);
    }

    private void FadeInExceptSelf()
    {
        Image[] childrenImages = GetComponentsInChildren<Image>(true);
        TextMeshProUGUI[] childrenTexts = GetComponentsInChildren<TextMeshProUGUI>(true);

        Image selfImage = GetComponent<Image>();

        foreach (Image image in childrenImages)
        {
            if (image == selfImage)
                continue;

            StartCoroutine(FadeInRoutine(image, 0.5f));
        }

        foreach (TextMeshProUGUI text in childrenTexts)
        {
            StartCoroutine(FadeInRoutine(text, 0.5f));
        }
    }

    private IEnumerator FadeInRoutine(Image image, float duration)
    {
        float timer = 0.0f;

        // �̹����� ��Ȱ��ȭ ������ �� ���� ���� 0���� ����
        Color startColor = image.color;
        startColor.a = 0.0f;
        image.color = startColor;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            // ���� ���� ������ ������ŵ�ϴ�.
            float alpha = Mathf.Clamp01(timer / duration);
            Color newColor = image.color;
            newColor.a = alpha;
            image.color = newColor;

            yield return null;
        }
    }

    private IEnumerator FadeInRoutine(TextMeshProUGUI textMeshProUGUI, float duration)
    {
        float timer = 0.0f;

        // �ؽ�Ʈ�� ��Ȱ��ȭ ������ �� ���� ���� 0���� ����
        Color startColor = textMeshProUGUI.color;
        startColor.a = 0.0f;
        textMeshProUGUI.color = startColor;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            // ���� ���� ������ ������ŵ�ϴ�.
            float alpha = Mathf.Clamp01(timer / duration);
            Color newColor = textMeshProUGUI.color;
            newColor.a = alpha;
            textMeshProUGUI.color = newColor;

            yield return null;
        }
    }

    private IEnumerator OnButton()
    {
        yield return new WaitForSeconds(3.0f);
        button.SetActive(true);
    }

    public void ButtonFunction()
    {
        FadeEffectController.instance.OnFade(FadeState.FadeOut);
        StartCoroutine(ChangeScene());
    }

    private IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(FadeEffectController.instance.fadeTime);
        button.SetActive(false);
        this.gameObject.SetActive(false);
        title.SetActive(false);
        PlayerController.instance.curSceneName = transferSceneName;
        SceneManager.LoadScene(transferSceneName);
        PlayerDungeonData.instance.isFighting = false;
    }
}
