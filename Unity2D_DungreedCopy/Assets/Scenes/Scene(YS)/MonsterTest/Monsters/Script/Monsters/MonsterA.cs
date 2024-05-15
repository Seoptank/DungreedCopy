using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterA : Test_Monster
{
    public enum State
    {
        None,
        Idle,
        Attack,
        Die,
    }
    public State monState;

    #region Bullet
    [SerializeField]
    private GameObject      prefabBullet;
    private PoolManager     bulletPool;
    #endregion

    private PoolManager pool;

    public override void InitValueSetting()
    {
        base.SetupEffectPools();

        monData.maxHP           = 50;
        monData.moveSpeed       = 5;
        monData.jumpForce       = 5;
        monData.isDie           = false;
        monData.isGround        = false;
        monData.originColor     = Color.white;
        monData.hitColor        = Color.red;
        monData.curHP = monData.maxHP;
    }

    public void Setup(PoolManager newPool)
    {
        this.pool = newPool;

        base.Awake();

        InitValueSetting();

        monData.hpBar.UpdateHPBar(monData.curHP, monData.maxHP);

        // ó�� ������ ���� canvasHP ��Ȱ��ȭ
        monData.canvasHP.SetActive(false);

        bulletPool = new PoolManager(prefabBullet);

        
    }

    private void OnEnable()
    {
        ChangeState(State.Idle);
    }
    private void OnDisable()
    {
        StopCoroutine(monState.ToString());
        monState = State.None;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ���� ���ݽ� ������ ���� �޾ƿ� ��ȣ�ۿ�
        if (collision.gameObject.tag == "PlayerAttack")
        {
            WeponInfo weapon = collision.gameObject.GetComponent<WeponInfo>();

            TakeAttack(weapon.curATK, weapon.textColor);
        }

        // ��� ���ݽ� PlayerStats���� ���� �޾ƿ� ��ȣ�ۿ�
        else if (PlayerController.instance.movement.isDashing && collision.gameObject.tag == "Player")
        {
            PlayerStats player = PlayerStats.instance;

            TakeAttack(player.DashATK, Color.blue);
        }
    }

    private void FixedUpdate()
    {
        // �÷��̾ �ִ� ������ �ٶ󺸵���
        UpdateSight();

        if (monData.curHP <= 0 && !monData.isDie)
        {
            monData.isDie = true;

            if (monData.isDie)
            {
                ChangeState(State.Die);
            }
        }
    }

    private IEnumerator Idle()
    {
        yield return new WaitForSeconds(5);
        ChangeState(State.Attack);
    }

    private IEnumerator Attack()
    {
        Vector3 dir     = (PlayerController.instance.transform.position - transform.position).normalized;
        Vector3 upDir   = Quaternion.Euler(0, 0, 15) * dir;
        Vector3 downDir = Quaternion.Euler(0, 0, -15) * dir;
            
        monData.animator.SetBool("IsAttack", true);
        AudioManager.Instance.PlaySFX("Bat");
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.3f);
            ActivateBullet(dir);
            ActivateBullet(upDir);
            ActivateBullet(downDir);

            if(i >= 2)
            {
                monData.animator.SetBool("IsAttack", false);
                ChangeState(State.Idle);
            }
        }
    }

    private IEnumerator Die()
    {
        ActivateDieEffect(transform);
        GiveCompensation(transform, 5);
        AudioManager.Instance.PlaySFX("EnemyDie");


        DoorDungeon dungeon = transform.parent.gameObject.GetComponent<DoorDungeon>();
        dungeon.enemiesCount--;

        PlayerDungeonData.instance.countKill++;
        PlayerDungeonData.instance.totalEXP += 100;

        pool.DeactivePoolItem(this.gameObject);
        yield return null;
    }

    private void ActivateBullet(Vector3 dir)
    {
        GameObject bullet = bulletPool.ActivePoolItem();
        bullet.transform.position = transform.position;
        bullet.transform.rotation = transform.rotation;
        bullet.GetComponent<BatBullet>().Setup(bulletPool, dir);
    }

    private void UpdateSight()
    {
        if(PlayerController.instance.transform.position.x > transform.position.x)
        {
            monData.spriteRenderer.flipX = false;
        }
        else
        {
            monData.spriteRenderer.flipX = true;
        }
    }

    public void ChangeState(State newState)
    {

        if (monState == newState) return;

        // ������ ����ϴ� ���� ���� 
        StopCoroutine(monState.ToString());

        // ���� ����
        monState = newState;

        // ���ο� ���� ���
        StartCoroutine(monState.ToString());
    }
}
