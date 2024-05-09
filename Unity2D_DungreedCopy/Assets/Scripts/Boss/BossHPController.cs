using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHPController : MonoBehaviour
{
    private Image   image;
    private BossHP  bossHp;
    private void Awake()
    {
        bossHp = FindObjectOfType<BossHP>();

        image = GetComponent<Image>();
    }
    private void Update()
    {
        // Mathf.Lerp(�ش�fillAmount, ����ü�� / �ִ�ü��, Time.deltaTime * �ӵ�);
        image.fillAmount = Mathf.Lerp(image.fillAmount, bossHp.curHP / bossHp.maxHP, Time.deltaTime * 5);
    }
}
