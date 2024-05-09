using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSword : MonoBehaviour
{
    [SerializeField]
    private float           fadeTime;
    [SerializeField]
    private float           flyDelayTime;
    [SerializeField]
    private Color           thisColor;
    private bool            isFlyAway = false;
    private GameObject      player;

    [SerializeField]
    private GameObject      chargePrefab;
    [SerializeField]
    private GameObject      whiteSwordPrefab;

    private PoolManager     poolManager;
    private PoolManager     swordChargePoolManager;
    private PoolManager     whiteSwordPoolManager;

    private SpriteRenderer      spriteRenderer;
    private UIEffectManager     uiEffectManager;
    
    public void Setup(PoolManager poolManager)
    {
        this.poolManager = poolManager;
    }

    private void Awake()
    {
        uiEffectManager = FindObjectOfType<UIEffectManager>();
        
        swordChargePoolManager = new PoolManager(chargePrefab);
        whiteSwordPoolManager  = new PoolManager(whiteSwordPrefab);

        spriteRenderer = GetComponent<SpriteRenderer>();

        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Start()
    {
        thisColor = spriteRenderer.color;
        thisColor.a = 0;
        spriteRenderer.color = thisColor;

        StartCoroutine(uiEffectManager.UIFade(spriteRenderer, 0, 1, fadeTime));

        SpawnCharge();
    }
    private void Update()
    {
        // Sword가 플레이어 방향을 바라보도록
        DirectionToPlayer();

        //FlyAwayToPlayer();
    }
    
    private void DirectionToPlayer()
    {
        Vector2 len = player.transform.position - this.transform.position;
        float   z   = Mathf.Atan2(len.y, len.x) * Mathf.Rad2Deg;

        // 회전 적용
        transform.rotation = Quaternion.Euler(0,0, z-90);

        // flyDelayTime 이후 WhiteSword 생성
        StartCoroutine(ChangeWhiteSword());
    }
    private IEnumerator ChangeWhiteSword()
    {
        yield return new WaitForSeconds(flyDelayTime);
        swordChargePoolManager.DeactiveAllPoolItems();
        GameObject white = whiteSwordPoolManager.ActivePoolItem();
        DeactivateSword();
        white.transform.position = transform.position;
        white.transform.rotation = transform.rotation;
        white.GetComponent<WhiteSword>().Setup(whiteSwordPoolManager);

    }
    private void SpawnCharge()
    {
        GameObject charge = swordChargePoolManager.ActivePoolItem();
        charge.transform.position = transform.position;
        charge.transform.rotation= transform.rotation;
        charge.GetComponent<BossSwordCharge>().Setup(swordChargePoolManager);
    }
    private void DeactivateSword()
    {
        poolManager.DeactivePoolItem(this.gameObject);
    }
}

