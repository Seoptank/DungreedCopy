 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterG2 : Test_Monster
{
    public enum State
    {
        None,
        Idle,
        Dash,
        Attack,
        Die,
    }
    public State monState;

    //움직임관련
    [SerializeField]
    private AnimationCurve  dashCurve;
    [SerializeField]
    private float           groundRayDis;
    private float           gravity = -9.8f;
    private Vector3         vel     = Vector3.zero;
    private Vector3         seeDir  = Vector3.zero;
    private Color           colorDebugGround;


    // 거리
    [SerializeField]
    private float   findTargetDis;
    [SerializeField]
    private bool    findTarget;
    [SerializeField]
    private bool    isDashing = false;
    [SerializeField]
    private bool    collideToPlayer = false;
    private bool    disableToDash = false;
    private bool    isWall = false;


    // 공격 관련
    private GameObject      attackCollider;
    private BoxCollider2D   attackBoxCollider;
    private Vector3         thrustPos;

    private PoolManager     pool;

    public override void InitValueSetting()
    {
        base.SetupEffectPools();
        monData.capsuleCollider2D.isTrigger = true;

        monData.maxHP = 50;
        monData.ATK   = 10;
        monData.moveSpeed = 3;
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

        attackCollider  = transform.GetChild(2).gameObject;
        attackBoxCollider = attackCollider.GetComponent<BoxCollider2D>();
        attackBoxCollider.enabled = false;

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
        CalaulateDisToTarget();
        ChangeThrustPos();
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

        // 바닥을 향해 레이 발사
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundRayDis, LayerMask.GetMask("Platform"));
        Debug.DrawRay(transform.position, Vector2.down * groundRayDis, colorDebugGround);

        if(hit.collider != null)
        {
            monData.isGround = true;
            colorDebugGround = Color.green;
        }
        else
        {
            monData.isGround = false;
            colorDebugGround = Color.red;
        }
        
        if(!monData.isGround)
        {
            vel.y += gravity * Time.deltaTime;
        }
        else if(monData.isGround)
        {
            vel.y = 0;
        }

        if (collideToPlayer)
        {
            PlayerController.instance.transform.position = thrustPos;
        }

        transform.position += vel * Time.deltaTime;
        
    }
    private void CheckWall()
    {
        Color rayColor = Color.white;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, seeDir, 2, LayerMask.GetMask("Platform"));
        Debug.DrawRay(transform.position, seeDir * 2, rayColor);

        if(hit.collider!= null)
        {
            isWall = true;
            rayColor = Color.green;
        }
        else
        {
            isWall = false;
            rayColor = Color.red;
        }
    }

    // 플레이어가 대시 공격시 위치하는 위치
    private void ChangeThrustPos()
    {
        if (seeDir == Vector3.left)
        {
            thrustPos = new Vector3(transform.position.x - 1.5f, transform.position.y - 0.7f);
        }
        else
        {
            thrustPos = new Vector3(transform.position.x + 1.5f, transform.position.y - 0.7f);
        }
    }

    #region 상태
    private IEnumerator Idle()
    {
        monData.animator.SetBool("IsMove", false);

        while (true)
        {
            vel.x = 0;

            if(findTarget)
            {
                yield return new WaitForSeconds(2f);
                UpdateSight();

                if(disableToDash)
                {
                    ChangeState(State.Attack);
                }
                else
                {
                    ChangeState(State.Dash);
                }
            }

            yield return null;
        }
    }
    private void CalaulateDisToTarget()
    {
        PlayerController player = PlayerController.instance;

        if(player != null)
        {
            Vector2 targetPos = player.transform.position;
            float dis = Vector2.Distance(targetPos, transform.position);

            if(dis <= findTargetDis)
            {
                findTarget = true;
            }

            if(targetPos.x <= transform.position.x + 2 && targetPos.x >= transform.position.x - 2)
            {
                disableToDash = true;
            }
            else
            {
                disableToDash = false;
            }
        }
    }
    private IEnumerator Dash()
    {
        monData.animator.SetBool("IsDash", true);

        float time = 0;
        float duration = 1;

        float startSpeed = monData.moveSpeed * 4; // 처음 속도 저장
        yield return new WaitForSeconds(0.2f);
        isDashing = true;
        while (isDashing)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(time / duration);
            float curveValue = dashCurve.Evaluate(t);


            if(isWall)
            {
                vel.x = 0;
                ChangeState(State.Attack);
            }
            else vel.x = seeDir.x * startSpeed * curveValue;



            if (collideToPlayer)
            {
                PlayerController.instance.dontMovePlayer = true;
            }

            if (duration <= time)
            {
                isDashing = false;
                StartCoroutine(RoutainDontMovePlayer());
                ChangeState(State.Attack);
            }
            yield return null;
        }
    }

    private IEnumerator RoutainDontMovePlayer()
    {
        yield return new WaitForSeconds(2);
        PlayerController.instance.dontMovePlayer = false;
    }

    private IEnumerator Attack()
    {
        monData.animator.SetBool("IsDash", false);
        yield return new WaitForSeconds(0.5f);
        collideToPlayer = false;
        monData.animator.SetBool("IsAttack", true);
        while (true)
        {
            vel.x = 0;

            yield return null;
        }
    }
    public void EnableAttackCollider()
    {
        attackBoxCollider.enabled = true;
    }
    public void CutAni()
    {
        monData.animator.SetBool("IsAttack", false);
        attackBoxCollider.enabled = false;
        ChangeState(State.Idle);
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
    #endregion

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

        // Dash중 플레이어와 부딪히면
        if(isDashing && collision.gameObject.tag == "Player")
        {
            collideToPlayer = true;
            
            if(!PlayerController.instance.isDie)
                PlayerController.instance.TakeDamage(15);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        // Dash중 플레이어와 부딪히면
        if (isDashing && collision.gameObject.tag == "Player")
        {
            collideToPlayer = true;
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
        Gizmos.DrawWireSphere(transform.position, findTargetDis);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(4,10));

    }
}
