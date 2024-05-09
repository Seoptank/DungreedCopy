using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// YS
public class HealText : MonoBehaviour
{
    [SerializeField]
    private float           moveSpeed;
    [SerializeField]
    private float           alphaSpeed;
    [SerializeField]
    private float           destroyTime;
    [SerializeField]
    private FairyController fairyController;

    private Color           alpha;

    private TextMeshPro     textHeal;
    private PoolManager     poolManager;

    public void Setup(PoolManager poolManager)
    {
        this.poolManager = poolManager;
    }
    

    private void Awake()
    {
        textHeal    = GetComponent<TextMeshPro>();
    }
    void Start()
    {
        alpha           = textHeal.color;
        textHeal.text   = "+" + (int)fairyController.increaseHP + "HP"; 
        Invoke("DeactivateEffect", destroyTime);
    }

    void Update()
    {
        // �ؽ�Ʈ�� �̵� ����
        transform.Translate(new Vector3(moveSpeed * Time.deltaTime, moveSpeed * Time.deltaTime,-moveSpeed * Time.deltaTime));

        // �ؽ�Ʈ�� ����ȯ�� ��ȯ
        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);

        // �ؽ�Ʈ�� ����ȯ �� ����
        textHeal.color = alpha;
    }
    public void DeactivateEffect()
    {
        poolManager.DeactivePoolItem(gameObject);
    }
}
