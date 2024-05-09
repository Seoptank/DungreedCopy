using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterG3 : Test_Monster
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
    private float jumpForce;
    private float gravity = -9.8f;
    private Vector3 vel = Vector3.zero;
    private Vector3 seeDir = Vector3.zero;
    private bool Jumping;
    private Color colorDebugGround;

    // 거리
    [SerializeField]
    private float chaseDis;
    [SerializeField]
    private float attackDis;

    // 공격 관련
    private GameObject attackCollider;
    private BoxCollider2D attackBoxCollider;
    private bool isAttacking;

    private PoolManager pool;
    public MonsterUpdateSight updateSight;
    public Animator bow;
    private Vector3 dir;
    #region Bullet
    [SerializeField]
    private GameObject prefabBullet;
    private PoolManager bulletPool;
    [SerializeField]
    private GameObject bulletPos;
    [SerializeField]
    private GameObject bowPos;
    #endregion

    public override void InitValueSetting()
    {
        base.SetupEffectPools();
        monData.capsuleCollider2D.isTrigger = true;

        monData.maxHP = 50;
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

        InitValueSetting();

        monData.hpBar.UpdateHPBar(monData.curHP, monData.maxHP);

        // 처음 생성된 적의 canvasHP 비활성화
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
    private void FixedUpdate()
    {
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

        float rayDis = monData.capsuleCollider2D.size.y * 0.5f; 

        // 바닥을 향해 레이 발사
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, rayDis, LayerMask.GetMask("Platform"));
        Debug.DrawRay(transform.position, Vector2.down * rayDis, colorDebugGround);

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
        transform.position += vel * Time.deltaTime;

    }

    private IEnumerator Idle()
    {
        while (true)
        {
            vel.x = 0;

            // Idle 인 경우 행동
            CalculateDisToTargetAndselectState();

            yield return null;
        }
    }

    private IEnumerator Chase()
    {
        float time = 0;
        float duration = 3.0f;
        // 타겟을 바라보도록
        while (time < duration)
        {
            time += Time.deltaTime;
            UpdateSight();
            updateSight.isChase = true;
            // 거리에 따른 상태 변화
            CalculateDisToTargetAndselectState();

            //transform.Translate(seeDir * monData.moveSpeed * Time.deltaTime);
            yield return null;
        }
        updateSight.isChase = false;
        dir = (PlayerController.instance.transform.position - bulletPos.transform.position).normalized;
        ChangeState(State.Charge);
    }

    private IEnumerator Charge()
    {
        bow.SetBool("IsCharge", true);

        yield return new WaitForSeconds(2.0f);
        bow.SetBool("IsCharge", false);
        ChangeState(State.Attack);
    }

    private IEnumerator Attack()
    {

        isAttacking = true;
        bow.SetBool("IsAttack", true);
        CreateBullet(dir);
        yield return new WaitForSeconds(1.0f);
        bow.SetBool("IsAttack", false);
        ChangeState(State.Idle);
    }

    void CreateBullet(Vector3 dir)
    {
        GameObject bullet = bulletPool.ActivePoolItem();
        bullet.transform.position = bulletPos.transform.position;
        bullet.transform.rotation = bowPos.transform.rotation;
        bullet.GetComponent<BatBullet>().Setup(bulletPool, dir);
    }
    public void EnableAttackCollider()
    {
        attackBoxCollider.enabled = true;
    }
    public void DisableAttackCollider()
    {
        attackBoxCollider.enabled = false;
    }

    public void CutAni()
    {
        isAttacking = false;
        monData.animator.SetBool("IsAttack", false);
        ChangeState(State.Idle);
    }

    private void CalculateDisToTargetAndselectState()
    {
        PlayerController player = PlayerController.instance;

        if (player != null)
        {
            Vector3 target = player.transform.position;
            float dis = Vector2.Distance(target, transform.position);

            // 쫒을수 있는 거리 내에 있으면
            if (dis <= chaseDis && dis > attackDis)
            {
                ChangeState(State.Chase);
            }
            // 모든 거리에서 벗어나면 => Idle
            else if (dis > chaseDis)
            {
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
}
