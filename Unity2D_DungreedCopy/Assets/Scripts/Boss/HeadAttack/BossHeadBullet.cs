using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHeadBullet : MonoBehaviour
{
    [Header("HeadBullet 정보")]
    [SerializeField]
    private int         dam;
    [SerializeField]
    private float       moveSpeed;


    [Header("BossBulletGameObject")]
    private PoolManager     bossBulletEffectPoolManager;
    [SerializeField]
    private GameObject      bossBulletEffectPrefab;

    private Rigidbody2D             rigidbody2D;
    private PlayerStats             playerStats;
    private PoolManager             poolManager;
    private MainCameraController    mainCam;
    private BoxCollider2D           thisBound;

    public void Setup(PoolManager poolManager)
    {
        this.poolManager = poolManager;
    }

    private void Awake()
    {
        bossBulletEffectPoolManager = new PoolManager(bossBulletEffectPrefab);

        rigidbody2D     = GetComponent<Rigidbody2D>(); 
        playerStats     = FindObjectOfType<PlayerStats>();
        mainCam         = FindObjectOfType<MainCameraController>();

        thisBound = GameObject.FindGameObjectWithTag("BossBound").GetComponent<BoxCollider2D>();
    }
    private void OnApplicationQuit()
    {
        bossBulletEffectPoolManager.DestroyObjcts();
    }
    private void Update()
    {
        rigidbody2D.velocity = moveSpeed * transform.right;

        CheckPosInBound(this.transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Player")
        {
            // bullet 비활성화
            DeactivateBullet();

            // Effect 활성화
            ActivateBossBulletEffect();

            // Player에게 데미지 주는 로직
            PlayerController.instance.TakeDamage(dam);
        }
    }

    private void CheckPosInBound(Vector2 bulletPos)
    {
        if(!thisBound.bounds.Contains(bulletPos))
        {
            DeactivateBullet();
        }
    }

    private void DeactivateBullet()
    {
        poolManager.DeactivePoolItem(gameObject);
    }
    private void ActivateBossBulletEffect()
    {
        GameObject bossBulletEffect = bossBulletEffectPoolManager.ActivePoolItem();
        bossBulletEffect.transform.position = this.transform.position;
        bossBulletEffect.transform.rotation = transform.rotation;
        bossBulletEffect.GetComponent<EffectPool>().Setup(bossBulletEffectPoolManager);
    }
}
