using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteSword : MonoBehaviour
{
    [SerializeField]
    private GameObject  swordHitEffectPrefab;
    [SerializeField]
    private float       moveSpeed;
    [SerializeField]
    private float       swordAtt;
    [SerializeField]
    private Sprite      origineSwordSprite;
    [SerializeField]
    private bool        isThrowing; 

    [Header("Deactivate Sword")]
    [SerializeField]
    private float       FadeTime;
    [SerializeField]
    private GameObject  deactivateEffectPrefab;
    private PoolManager deactivateEffectpoolManager;

    private SpriteRenderer  spriteRenderer;
    private Rigidbody2D     rigid;
    private PoolManager     poolManager;
    private PoolManager     swordHitEffectPoolManager;
    private PlayerStats     playerStats;
    [SerializeField]
    private BossPattern     bossPattern;
    public void Setup(PoolManager poolManager,BossPattern parentBossPattern)
    {
        this.poolManager = poolManager;
        swordHitEffectPoolManager   = new PoolManager(swordHitEffectPrefab);
        deactivateEffectpoolManager = new PoolManager(deactivateEffectPrefab);
        bossPattern = parentBossPattern;

        isThrowing = true;

        playerStats     = FindObjectOfType<PlayerStats>();

        rigid           = GetComponent<Rigidbody2D>();
        spriteRenderer  = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if(isThrowing)
        {
            transform.Translate(moveSpeed * Vector2.up * Time.deltaTime);
        }
        else
        {
            rigid.velocity = new Vector2(0, 0);
            rigid.bodyType = RigidbodyType2D.Static;
        }
       
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Player" && !PlayerController.instance.isDie && !bossPattern.isDie)
        {
            PlayerController.instance.TakeDamage(swordAtt);
        }

        if(collision.gameObject.CompareTag("Platform"))
        {
            isThrowing = false;
            SpawnHitEffect();
            spriteRenderer.sprite = origineSwordSprite;
            StartCoroutine("DeactivateEffect");
        }
    }

    private void SpawnHitEffect()
    {
        GameObject hitEffect = swordHitEffectPoolManager.ActivePoolItem();
        hitEffect.transform.position = transform.position;
        hitEffect.transform.rotation = Quaternion.Euler(0,0,transform.rotation.eulerAngles.z -180f);
        hitEffect.GetComponent<EffectPool>().Setup(swordHitEffectPoolManager);
    }
    private IEnumerator DeactivateEffect()
    {
        yield return new WaitForSeconds(FadeTime);
        poolManager.DeactivePoolItem(this.gameObject);
        GameObject deactivateEffect = deactivateEffectpoolManager.ActivePoolItem();
        deactivateEffect.transform.position = transform.position;
        deactivateEffect.transform.rotation = deactivateEffect.transform.rotation;
        deactivateEffect.GetComponent<EffectPool>().Setup(deactivateEffectpoolManager);
        rigid.bodyType = RigidbodyType2D.Dynamic;
        poolManager.DeactivePoolItem(gameObject);
    }
}
