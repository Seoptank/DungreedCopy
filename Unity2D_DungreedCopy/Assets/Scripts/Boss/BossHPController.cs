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
        // Mathf.Lerp(해당fillAmount, 현재체력 / 최대체력, Time.deltaTime * 속도);
        image.fillAmount = Mathf.Lerp(image.fillAmount, bossHp.curHP / bossHp.maxHP, Time.deltaTime * 5);
    }
}
