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

    [Header("Chase 변수")]
    [SerializeField]
    private float   chaseRadius;
    [SerializeField]
    private float   attackRadius;
    [SerializeField]
    private bool    canAttack = false; 

    private float   dis;
    private bool    attacking = false;  // 공격중임을 알리는 변수로 쫒을 때는 UpdateSight 적용하지 않기 위함 

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

        // 처음 생성된 적의 canvasHP 비활성화
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
        // 플레이어 방향을 바라보도록
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
            // 플레이어 위치 파악해서 존나 따라감
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
        float attackDuration = 1.5f;  // 공격 지속 시간 (초)
        float time = 0f;            // 경과 시간

        Vector3 target = PlayerController.instance.transform.position;
        Vector3 dir = (target - transform.position).normalized;

        monData.animator.SetBool("IsAttack", true);

        attacking = true;
        // 공격 지속 시간 동안에만 이동
        while (true)
        {
            // 코루틴 들어올때 받아온 플레이어의 방향으로 원래속도의 1.5배의 속도로 쫒아간다.
            transform.position += dir *( monData.moveSpeed * 3) * Time.deltaTime;

            // 경과 시간 업데이트
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

            // 쫒을수 있는 거리 내에 있으면
            if(dis <= chaseRadius && !canAttack)
            {
                ChangeState(State.Chase);
            }
            // 모든 거리에서 벗어나면 => Idle
            else if(dis > chaseRadius && !canAttack)
            {
                ChangeState(State.Idle);
            }
            // 공격 거리 내에 들어오면 => Attack
            else if(dis <= attackRadius && canAttack)
            {
                ChangeState(State.ChaseAttack);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 무기 공격시 무기의 정보 받아와 상호작용
        if (collision.gameObject.tag == "PlayerAttack")
        {
            WeponInfo weapon = collision.gameObject.GetComponent<WeponInfo>();

            TakeAttack(weapon.curATK, weapon.textColor);
        }

        // 대시 공격시 PlayerStats에서 정보 받아와 상호작용
        else if (PlayerController.instance.movement.isDashing && collision.gameObject.tag == "Player")
        {
            PlayerStats player = PlayerStats.instance;

            TakeAttack(player.DashATK, Color.blue);
        }
        // LittleGhost가 공격중인 상태에서 Player와 부딪히면?
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

        // 이전에 재생하던 상태 종료 
        StopCoroutine(monState.ToString());

        // 상태 변경
        monState = newState;

        // 새로운 상태 재생
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
