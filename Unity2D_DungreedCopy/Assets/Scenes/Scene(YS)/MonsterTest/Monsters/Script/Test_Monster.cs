using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MonstterData
{
    public float curHP;         // 현재 체력
    public float maxHP;         // 최대 체력
    public float moveSpeed;     // 움직임 속도
    public float jumpForce;     // 점프 힘
    public float ATK;     // 공격력

    public bool isDie;         // 죽었는지?          => DieEffect 실행 위함
    public bool isGround;      // 땅인지?            => Trigger로 몬스터의 Collider을 붙일것이기에 땅과 맡닿아있는지 확인/ 점프
    public bool canAttack;     // 공격이 가능한지?   => Trigger로 몬스터의 Collider을 붙일것이기에 땅과 맡닿아있는지 확인/ 점프

    public Color originColor;   // 기본 컬러
    public Color hitColor;      // 피격시 컬러

    public GameObject canvasHP;

    public HPBar hpBar;
    public SpriteRenderer spriteRenderer;
    public CapsuleCollider2D capsuleCollider2D;
    public Rigidbody2D rigidbody2D;
    public Animator animator;
}

[Serializable]
public class EffectData
{
    public GameObject prefabDieEffect;    // 죽음시 나타나는 이펙트
    public GameObject prefabDamageTest;   // 타격시 텍스트 이펙트
    public GameObject prefabGold;         // 몬스터 죽을 시 주는 보상 골드들
}

public abstract class Test_Monster : MonoBehaviour
{
    #region 데이터 클래스
    public MonstterData monData;
    public EffectData monEffectData;
    #endregion

    #region 이펙트 Pools
    protected PoolManager dieEffectPool;
    protected PoolManager damageTextPool;
    protected PoolManager goldPool;
    #endregion


    protected void Awake()
    {
        monData.spriteRenderer = GetComponent<SpriteRenderer>();
        monData.capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        monData.rigidbody2D = GetComponent<Rigidbody2D>();
        monData.animator = GetComponent<Animator>();
        monData.canvasHP = transform.GetChild(1).gameObject;
        monData.hpBar = monData.canvasHP.GetComponentInChildren<HPBar>();
    }

    public virtual void CheckGround()
    {

        // 레이캐스트를 위한 시작점과 방향 설정
        Vector2 rayOrigin = monData.capsuleCollider2D.bounds.center;
        Vector2 rayDir = Vector2.down;

        float ratDis = 0.02f;

        // 레이를 그리기 위한 끝점 계산
        Vector2 rayEnd = rayOrigin + rayDir * ratDis;

        RaycastHit2D hit = Physics2D.BoxCast(rayOrigin,
                                             monData.capsuleCollider2D.bounds.size,
                                             0, rayDir, ratDis, LayerMask.GetMask("Platform"));
        if (hit.collider != null)
        {
            monData.isGround = true;
        }
        else
            monData.isGround = false;

        if (!monData.isGround)
        {
            monData.rigidbody2D.velocity = new Vector2(monData.rigidbody2D.velocity.x, monData.rigidbody2D.velocity.y);
            monData.rigidbody2D.gravityScale = 1;
        }
        else
        {
            monData.rigidbody2D.velocity = new Vector2(monData.rigidbody2D.velocity.x, 0);
            monData.rigidbody2D.gravityScale = 0;
        }
    }

    // 각각의 Monster 클래스에서 변수값을 셋팅할 수 있게
    public abstract void InitValueSetting();

    // 각각의 Monster가 생성될 Clone들을 불러올 수 있게
    protected virtual void SetupEffectPools()
    {
        dieEffectPool = new PoolManager(monEffectData.prefabDieEffect);
        damageTextPool = new PoolManager(monEffectData.prefabDamageTest);
        goldPool = new PoolManager(monEffectData.prefabGold);
    }

    // dieEffect 생성 메서드
    protected virtual void ActivateDieEffect(Transform transform)
    {
        GameObject dieEffect = dieEffectPool.ActivePoolItem();
        dieEffect.transform.position = transform.position;
        dieEffect.transform.rotation = transform.rotation;
        dieEffect.GetComponent<EffectPool>().Setup(dieEffectPool);
    }
    protected virtual void GiveCompensation(Transform transform,int maxItemCount)
    {
        GameObject gold = goldPool.ActivePoolItem();
        gold.transform.position = transform.position;
        gold.transform.rotation = transform.rotation;
        gold.GetComponent<ItemSpawnManager>().Setup(goldPool,maxItemCount);
    }
    protected virtual void TakeAttack(int dam, Color textColor)
    {
        if (monData.curHP >= 0)
        {
            // 첫 공격이 들어가는 순간부터 HP바 보이게
            monData.canvasHP.SetActive(true);

            // 타격받았을 때 Enemy색상 빨간색으로
            monData.spriteRenderer.color = monData.hitColor;

            // Enemy 색상 원래 색상으 회귀
            StartCoroutine(ReturnColor());

            // HP 감소 함수
            TakeDamage(dam);

            // 타격 텍스트 Effect
            ActivateText(dam, textColor);

            // 카메라 흔들기
            MainCameraController.instance.OnShakeCamByPos(0.1f, 0.1f);

            // 현재 HP상태 UI에 업데이트
            monData.hpBar.UpdateHPBar(monData.curHP, monData.maxHP);
        }
    }
    private IEnumerator ReturnColor()
    {
        yield return new WaitForSeconds(0.3f);
        monData.spriteRenderer.color = monData.originColor;
    }
    private void TakeDamage(int dam)
    {
        monData.curHP -= dam;
    }
    private void ActivateText(int damage, Color color)
    {
        GameObject dam = damageTextPool.ActivePoolItem();
        dam.transform.position = transform.position;
        dam.transform.rotation = transform.rotation;
        dam.GetComponent<DamageText>().Setup(damageTextPool, damage, color);
    }
}
