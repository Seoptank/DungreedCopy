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
        // ������ �ؽ�Ʈ�� ���
        textDamage.text = damage.ToString();
        // ũ��Ƽ�ð� �Ϲ� ���� ����
        color = newColor;
        textDamage.color = color;
        // ȿ�� ��Ȱ��ȭ
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
