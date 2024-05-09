using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBatBullet : MonoBehaviour
{
    [SerializeField]
    private float inputSpeed;             // inputSpeed => speed에 적용
    [SerializeField]
    private float radius;                 // ray의 반지름
    private bool isExplosion = false;    // 폭발 애니 재생 여부
    public GameObject bulletPos;
    //public static bool      fullCharge;

    private PoolManager pool;
    private Animator ani;
    private Rigidbody2D rigid;
    private MonsterD parent;
    private bool shoot = false;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        //fullCharge = false;
    }

    public void Setup(PoolManager newPool, GameObject pos, GameObject parentBody)
    {
        pool = newPool;
        ani = GetComponent<Animator>();
        bulletPos = pos;
        parent = parentBody.GetComponent<MonsterD>();
        Invoke("DeactivateEffect", 5);
    }

    private void Update()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, radius, Vector2.zero);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Player"))
            {
                isExplosion = true;
            }
            else if (hit.collider.CompareTag("Platform"))
            {
                isExplosion = true;
            }
        }

        // 모든 bullet이 생성된 상탠
        if (parent.fullCharge)
        {
            // 물체에 부딪힌 상태 
            if (isExplosion)
            {
                rigid.velocity = Vector2.zero;
                ani.SetBool("OnEffect", true);
            }
            // 물체에 부딪히지 않은 상태 
            else if (shoot == false)
            {
                rigid.velocity = bulletPos.transform.right * inputSpeed;
                shoot = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !PlayerController.instance.isDie)
        {
            PlayerController.instance.TakeDamage(10);
        }
    }


    public void DeactivateEffect()
    {
        ani.SetBool("OnEffect", false);
        isExplosion = false;
        pool.DeactivePoolItem(this.gameObject);
        parent.explosionCount++;
        shoot = false;
        //fullCharge = false;
    }
}