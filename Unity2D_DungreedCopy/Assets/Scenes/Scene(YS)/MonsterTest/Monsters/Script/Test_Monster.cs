using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MonstterData
{
    public float curHP;         // ���� ü��
    public float maxHP;         // �ִ� ü��
    public float moveSpeed;     // ������ �ӵ�
    public float jumpForce;     // ���� ��
    public float ATK;     // ���ݷ�

    public bool isDie;         // �׾�����?          => DieEffect ���� ����
    public bool isGround;      // ������?            => Trigger�� ������ Collider�� ���ϰ��̱⿡ ���� �ô���ִ��� Ȯ��/ ����
    public bool canAttack;     // ������ ��������?   => Trigger�� ������ Collider�� ���ϰ��̱⿡ ���� �ô���ִ��� Ȯ��/ ����

    public Color originColor;   // �⺻ �÷�
    public Color hitColor;      // �ǰݽ� �÷�

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
    public GameObject prefabDieEffect;    // ������ ��Ÿ���� ����Ʈ
    public GameObject prefabDamageTest;   // Ÿ�ݽ� �ؽ�Ʈ ����Ʈ
    public GameObject prefabGold;         // ���� ���� �� �ִ� ���� ����
}

public abstract class Test_Monster : MonoBehaviour
{
    #region ������ Ŭ����
    public MonstterData monData;
    public EffectData monEffectData;
    #endregion

    #region ����Ʈ Pools
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

        // ����ĳ��Ʈ�� ���� �������� ���� ����
        Vector2 rayOrigin = monData.capsuleCollider2D.bounds.center;
        Vector2 rayDir = Vector2.down;

        float ratDis = 0.02f;

        // ���̸� �׸��� ���� ���� ���
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

    // ������ Monster Ŭ�������� �������� ������ �� �ְ�
    public abstract void InitValueSetting();

    // ������ Monster�� ������ Clone���� �ҷ��� �� �ְ�
    protected virtual void SetupEffectPools()
    {
        dieEffectPool = new PoolManager(monEffectData.prefabDieEffect);
        damageTextPool = new PoolManager(monEffectData.prefabDamageTest);
        goldPool = new PoolManager(monEffectData.prefabGold);
    }

    // dieEffect ���� �޼���
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
            // ù ������ ���� �������� HP�� ���̰�
            monData.canvasHP.SetActive(true);

            // Ÿ�ݹ޾��� �� Enemy���� ����������
            monData.spriteRenderer.color = monData.hitColor;

            // Enemy ���� ���� ������ ȸ��
            StartCoroutine(ReturnColor());

            // HP ���� �Լ�
            TakeDamage(dam);

            // Ÿ�� �ؽ�Ʈ Effect
            ActivateText(dam, textColor);

            // ī�޶� ����
            MainCameraController.instance.OnShakeCamByPos(0.1f, 0.1f);

            // ���� HP���� UI�� ������Ʈ
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
