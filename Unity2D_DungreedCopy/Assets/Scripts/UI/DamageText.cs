using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [SerializeField]
    private float           waitTime;
    [SerializeField]
    private float           fadeSpeed;
    [SerializeField]
    private float           speedX, speedY;

    private Color           color;
    private TextMeshPro     textDamage;
    private PoolManager     pool;
    public void Setup(PoolManager newPool,int damage,Color newColor)
    {
        textDamage = GetComponent<TextMeshPro>();

        pool = newPool;
        // 데미지 텍스트로 띄움
        textDamage.text = damage.ToString();
        // 크리티컬과 일반 공격 구분
        color = newColor;
        textDamage.color = color;
        // 효과 비활성화
        StartCoroutine(DeactivateText());
    }
    private void Update()
    {
        transform.Translate(new Vector3(speedX * Time.deltaTime, speedY * Time.deltaTime, 0));
        color.a = Mathf.Lerp(color.a, 0, Time.deltaTime * fadeSpeed);
        textDamage.color = color;
    }
    private IEnumerator DeactivateText()
    {
        yield return new WaitForSeconds(waitTime);
        pool.DeactivePoolItem(this.gameObject);

    }
}
