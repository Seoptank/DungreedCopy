using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterC : Test_Monster
{
    public enum State
    {
        None,
        Idle,
        Chase,
        ChaseAttack,
        Die,
    }
    public State monState;

    [Header("Chase ����")]
    [SerializeField]
    private float   chaseRadius;
    [SerializeField]
    private float   attackRadius;
    [SerializeField]
    private bool    canAttack = false; 

    private float   dis;
    private bool    attacking = false;  // ���������� �˸��� ������ �i�� ���� UpdateSight �������� �ʱ� ���� 

    private PoolManager pool;

    public override void InitValueSetting()
    {
        base.SetupEffectPools();
        monData.capsuleCollider2D.isTrigger = true;

        monData.maxHP = 20;
        monData.ATK = 5;
        monData.moveSpeed = 2;
        monData.isDie = false;
        monData.isGround = false;
        monData.originColor = Color.white;
        monData.hitColor = Color.red;
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

    private void FixedUpdate()
    {
        // �÷��̾� ������ �ٶ󺸵���
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
        while(true)
        {
            CalculateDisToTargetAndselectState();

            yield return null;
        }
    }

    private IEnumerator Chase()
    {
        float canAttackDuration = 3;
        float time = 0;

        while (true) 
        {
            // �÷��̾� ��ġ �ľ��ؼ� ���� ����
            transform.position = Vector2.MoveTowards(transform.position, PlayerController.instance.transform.position, monData.moveSpeed * Time.deltaTime);

            CalculateDisToTargetAndselectState();

            time += Time.deltaTime;
            
            if(time > canAttackDuration)
            {
                canAttack = true;
            }
            yield return null;
        }
    }

    private IEnumerator ChaseAttack()
    {
        float attackDuration = 1.5f;  // ���� ���� �ð� (��)
        float time = 0f;            // ��� �ð�

        Vector3 target = PlayerController.instance.transform.position;
        Vector3 dir = (target - transform.position).normalized;

        monData.animator.SetBool("IsAttack", true);

        attacking = true;
        // ���� ���� �ð� ���ȿ��� �̵�
        while (true)
        {
            // �ڷ�ƾ ���ö� �޾ƿ� �÷��̾��� �������� �����ӵ��� 1.5���� �ӵ��� �i�ư���.
            transform.position += dir *( monData.moveSpeed * 3) * Time.deltaTime;

            // ��� �ð� ������Ʈ
            time += Time.deltaTime;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 1, LayerMask.GetMask("Platform"));
            Debug.DrawRay(transform.position, dir * 1, Color.red);

            if(hit.collider != null && hit.collider.CompareTag("Platform"))
            {
                canAttack = false;
                attacking = false;
                monData.animator.SetBool("IsAttack", false);
                CalculateDisToTargetAndselectState();
            }

            if(time > attackDuration)
            {
                canAttack = false;
                attacking = false;  
                monData.animator.SetBool("IsAttack", false);
                CalculateDisToTargetAndselectState();
            }
            yield return null;
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

    private void CalculateDisToTargetAndselectState()
    {
        PlayerController player = PlayerController.instance;

        if(player != null)
        {
            Vector3 target = player.transform.position; 
            dis = Vector2.Distance(target, transform.position);

            // �i���� �ִ� �Ÿ� ���� ������
            if(dis <= chaseRadius && !canAttack)
            {
                ChangeState(State.Chase);
            }
            // ��� �Ÿ����� ����� => Idle
            else if(dis > chaseRadius && !canAttack)
            {
                ChangeState(State.Idle);
            }
            // ���� �Ÿ� ���� ������ => Attack
            else if(dis <= attackRadius && canAttack)
            {
                ChangeState(State.ChaseAttack);
            }
        }
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
        // LittleGhost�� �������� ���¿��� Player�� �ε�����?
        else if(collision.gameObject.tag == "Player" && attacking)
        {
            if (!PlayerController.instance.isDie)
                PlayerController.instance.TakeDamage(monData.ATK);
        }
    }

    private void UpdateSight()
    {
        if(!attacking)
        {
            if (PlayerController.instance.transform.position.x > transform.position.x)
            {
                monData.spriteRenderer.flipX = false;
            }
            else
            {
                monData.spriteRenderer.flipX = true;
            }
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
