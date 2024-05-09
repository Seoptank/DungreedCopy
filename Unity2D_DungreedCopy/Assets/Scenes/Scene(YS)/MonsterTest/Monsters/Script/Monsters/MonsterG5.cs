using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MonsterG5 : Test_Monster
{
    public enum State
    {
        None,
        Idle,
        Chase,
        Charge,
        Attack,
        Die,
    }
    public State monState;


    //움직임관련
    [SerializeField]
    private float gravity = -9.8f;      // 중력 계수
    private Vector3 vel = Vector3.zero;
    private Vector3 seeDir = Vector3.zero;
    private bool Jumping;
    private Color colorDebugGround;
    private float groundRayDis;
    [SerializeField]
    private bool findTarget;
    // 포물선 운동에 사용할 각도
    private const float launchAngle = 45f;
    private bool isAttack;

    // 거리
    [SerializeField]
    private float chaseDis;
    [SerializeField]
    private float attackDis;

    // 공격 관련
    private const float attackCooldown = 3f; // 공격 쿨다운 시간
    private float lastAttackTime; // 마지막 공격 시간

    private PoolManager pool;

    public override void InitValueSetting()
    {
        base.SetupEffectPools();
        monData.capsuleCollider2D.isTrigger = true;

        monData.maxHP = 50;
        monData.ATK   = 5;
        monData.moveSpeed = 4.5f;
        monData.jumpForce = 4;
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

        groundRayDis = monData.capsuleCollider2D.size.y * 0.5f;

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
        FindTarget();
        CheckWall();

        #region Die
        if (monData.curHP <= 0 && !monData.isDie)
        {
            monData.isDie = true;

            if (monData.isDie)
            {
                ChangeState(State.Die);
            }
        }
        #endregion

        #region 바닥을 검사하는 ray/ isGround 관리
        // 바닥을 향해 레이 발사
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundRayDis, LayerMask.GetMask("Platform"));
        Debug.DrawRay(transform.position, Vector2.down * groundRayDis, colorDebugGround);

        if (hit.collider != null)
        {
            monData.isGround = true;
            colorDebugGround = Color.green;
        }
        else
        {
            monData.isGround = false;
            colorDebugGround = Color.red;
        }

        // isGround상태에 따른 y값 변환
        if (!monData.isGround)
        {
            vel.y += gravity * Time.deltaTime;
        }
        else if (monData.isGround && !Jumping)
        {
            vel.y = 0;
        }
        else if (monData.isGround)
        {
            Jumping = false;
        }
        #endregion

        transform.position += vel * Time.deltaTime;

    }
    private void CheckWall()
    {
        // Raycast 발사를 위한 시작점과 방향 설정
        Vector3 rayOrigin = transform.position; // 캐릭터에서 시작
        Vector3 rayDir = seeDir;                // 레이 방향 = 바라보는 방향

        // Raycast 발사
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDir, 1f, LayerMask.GetMask("Platform"));

        // 디버그를 위한 Ray 그리기
        Debug.DrawRay(rayOrigin, rayDir * 1f, Color.red);

        // Platform 충돌 확인
        if (hit.collider != null)
        {
            vel.x = 0;
        }
    }
    private IEnumerator Idle()
    {
        while (true)
        {
            vel.x = 0;

            if (findTarget)
            {
                yield return new WaitForSeconds(0.5f);
                ChangeState(State.Chase);
            }

            yield return null;
        }
    }

    private IEnumerator Chase()
    {
        PlayerController player = PlayerController.instance;

        monData.animator.SetBool("IsMove", true);
        while (true)
        {
            vel.x = seeDir.x * monData.moveSpeed;
            UpdateSight();

            // 거리에 따른 상태 변화
            CalculateDisToTargetAndselectState();

            yield return null;
        }
        monData.animator.SetBool("IsMove", false);

    }

    private IEnumerator Charge()
    {
        monData.animator.SetBool("IsMove", false);
        vel.x = 0;

        yield return new WaitForSeconds(0.3f);
        ChangeState(State.Attack);
    }

    private IEnumerator Attack()
    {

        PlayerController player = PlayerController.instance;

        monData.animator.SetBool("IsJumpAttack", true);
        isAttack = true;
        Jump();
        while (true)
        {
            if (vel.y < 0)
            {
                monData.animator.SetBool("IsJumpAttack", false);
                monData.animator.SetBool("IsFall", true);
            }

            if (vel.y == 0)
            {
                // 공격 후 공격 쿨다운 시작
                lastAttackTime = Time.time;

                CutAni();
            }
            yield return null;
        }
    }
    private void Jump()
    {
        vel.y = monData.jumpForce;
        vel.x = seeDir.x * (monData.moveSpeed * 2f);
        Jumping = true;
    }
    public void CutAni()
    {
        monData.animator.SetBool("IsFall", false);
        isAttack = false;
        ChangeState(State.Idle);
    }

    private void FindTarget()
    {
        PlayerController player = PlayerController.instance;

        if (player != null)
        {
            Vector3 target = player.transform.position;
            float dis = Vector2.Distance(target, transform.position);

            if (dis <= chaseDis)
            {
                findTarget = true;
            }
        }
    }

    private void CalculateDisToTargetAndselectState()
    {
        PlayerController player = PlayerController.instance;

        if (player != null)
        {
            Vector3 target = player.transform.position;
            float dis = Vector2.Distance(target, transform.position);

            if (dis <= attackDis)
            {
                if (Time.time >= lastAttackTime + attackCooldown)
                    ChangeState(State.Charge);
                else
                    ChangeState(State.Chase);
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

        if (isAttack && collision.gameObject.tag == "Player")
        {
            if (!PlayerController.instance.isDie)
                PlayerController.instance.TakeDamage(monData.ATK);
        }
    }

    private void UpdateSight()
    {
        bool isRight = PlayerController.instance.transform.position.x >= transform.position.x;
        monData.spriteRenderer.flipX = !isRight;

        seeDir = isRight ? Vector3.right : Vector3.left;
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
        Gizmos.DrawWireSphere(transform.position, chaseDis);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDis);
    }
}
