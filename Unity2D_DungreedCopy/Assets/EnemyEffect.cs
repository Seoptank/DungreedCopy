using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEffect : MonoBehaviour
{
    public static event Action<GameObject> EnemyDieEvent; // 적이 죽을 때 발생하는 이벤트

    [Header("Enemy Info")]
    [SerializeField]
    private float       curHP;
    [SerializeField]
    private float       maxHP;
    [SerializeField]
    private bool        isDie;
    [SerializeField]
    private GameObject  prefabDieEffect;
    [SerializeField]
    private GameObject  CanvasHP;           // CanvasEnemy GameObject
    private bool        isGround;


    [Header("Text Effect")]
    [SerializeField]
    private GameObject  prefabDamageText;
    [SerializeField]
    private float       timeToReturnOriginColor = 0.3f;

    private Color originColor;
    private Color color;

    private PoolManager     textPoolManager;
    private PoolManager     dieEffectPoolManager;

    private SpriteRenderer  spriteRenderer;
    private HPBar           healthBar;
    private Movement2D      movement;
    private Rigidbody2D     rigidbody;

    private void Awake()
    {
        spriteRenderer  = GetComponent<SpriteRenderer>();
        rigidbody       = GetComponent<Rigidbody2D>();
        healthBar       = GetComponentInChildren<HPBar>();

        movement = FindObjectOfType<Movement2D>();

        textPoolManager = new PoolManager(prefabDamageText);
        dieEffectPoolManager = new PoolManager(prefabDieEffect);
    }
    private void Start()
    {
        curHP = maxHP;
        healthBar.UpdateHPBar(curHP, maxHP);

        originColor = spriteRenderer.color;
        color = Color.red;

        CanvasHP.SetActive(false);
    }

    private void Update()
    {
        if(curHP <= 0 && !isDie)
        {
            isDie = true;

            if(isDie)
            {
                StartCoroutine("Die");
            }
        }

        if(isGround)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0);
            rigidbody.gravityScale = 0;
        }
        else
        {
            rigidbody.gravityScale = 1;
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y);
        }
    }
    private IEnumerator Die()
    {
        ActivateDieEffect();

        // 적이 죽었음을 이벤트로 발생시킴
        if (EnemyDieEvent != null)
        {
            EnemyDieEvent(gameObject);
        }

        PlayerDungeonData.instance.countKill++;
        PlayerDungeonData.instance.totalEXP += 100;

        Destroy(this.gameObject);
        yield return null;
    }
    private void ActivateDieEffect()
    {
        GameObject effect = dieEffectPoolManager.ActivePoolItem();
        effect.transform.position = transform.position;
        effect.transform.rotation = transform.rotation;
        effect.GetComponent<EffectPool>().Setup(dieEffectPoolManager);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 무기 공격시 무기의 정보 받아와 상호작용
        if(collision.gameObject.tag == "PlayerAttack")
        {
            WeponInfo weapon = collision.gameObject.GetComponent<WeponInfo>();

            TakeAttack(weapon.curATK, weapon.textColor);
        }
        
        // 대시 공격시 PlayerStats에서 정보 받아와 상호작용
        else if( movement.isDashing && collision.gameObject.tag == "Player")
        {
            PlayerStats player = PlayerStats.instance;

            TakeAttack(player.DashATK, Color.blue);
        }

        if(collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            isGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            isGround = false;
        }
    }

    private void TakeAttack(int dam,Color textColor)
    {
        if(curHP >= 0)
        {
            // 첫 공격이 들어가는 순간부터 HP바 보이게
            CanvasHP.SetActive(true);

            // 타격받았을 때 Enemy색상 빨간색으로
            spriteRenderer.color = color;

            // Enemy 색상 원래 색상으 회귀
            StartCoroutine(ReturnColor());

            // HP 감소 함수
            TakeDamage(dam);

            // 타격 텍스트 Effect
            ActivateText(dam, textColor);

            // 카메라 흔들기
            MainCameraController.instance.OnShakeCamByPos(0.1f, 0.1f);

            // 현재 HP상태 UI에 업데이트
            healthBar.UpdateHPBar(curHP, maxHP);
        }
    }

    private IEnumerator ReturnColor()
    {
        yield return new WaitForSeconds(timeToReturnOriginColor);
        spriteRenderer.color = originColor;
    }

    private void TakeDamage(int dam)
    {
        curHP -= dam;
    }

    private void ActivateText(int damage,Color color)
    {
        GameObject dam = textPoolManager.ActivePoolItem();
        dam.transform.position = transform.position;
        dam.transform.rotation = transform.rotation;
        dam.GetComponent<DamageText>().Setup(textPoolManager, damage, color);
    }
}
