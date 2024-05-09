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
        // 텍스트의 이동 방향
        transform.Translate(new Vector3(moveSpeed * Time.deltaTime, moveSpeed * Time.deltaTime,-moveSpeed * Time.deltaTime));

        // 텍스트의 색변환값 변환
        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);

        // 텍스트의 갯변환 값 저장
        textHeal.color = alpha;
    }
    public void DeactivateEffect()
    {
        poolManager.DeactivePoolItem(gameObject);
    }
}
