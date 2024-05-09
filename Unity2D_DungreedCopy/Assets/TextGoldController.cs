using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class TextGoldController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float alphaSpeed;
    [SerializeField]
    private float destroyTime;

    private Color alpha;

    private TextMeshPro textGold;
    private PoolManager poolManager;

    public void Setup(PoolManager poolManager, int goldValue)
    {
        this.poolManager = poolManager;
        textGold = GetComponent<TextMeshPro>();

        alpha = textGold.color;
        textGold.text = goldValue + "G";
        Invoke("DeactivateEffect", destroyTime);
    }
    void Update()
    {
        // �ؽ�Ʈ�� �̵� ����
        transform.Translate(new Vector3(moveSpeed * Time.deltaTime, moveSpeed * Time.deltaTime, -moveSpeed * Time.deltaTime));

        // �ؽ�Ʈ�� ����ȯ�� ��ȯ
        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);

        // �ؽ�Ʈ�� ����ȯ �� ����
        textGold.color = alpha;
    }

    public void DeactivateEffect()
    {
        poolManager.DeactivePoolItem(gameObject);
    }
}
