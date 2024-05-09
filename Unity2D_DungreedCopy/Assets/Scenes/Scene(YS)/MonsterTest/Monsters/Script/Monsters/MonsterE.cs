using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterE : Test_Monster
{
    public enum State
    {
        None,
        Idle,
        Wander,
        Attack,
        Die,
    }
    public State monState;

    #region Bullet
    [SerializeField]
    private GameObject prefabBullet;
    private PoolManager bulletPool;
    #endregion

    [SerializeField]
    private float radius;
    [SerializeField]
    private float range;
    [SerializeField]
    private float maxDis;
    private Vector2 point;
    private int wanderCount = 0;
    private Vector2 dir;


    private PoolManager pool;

    public override void InitValueSetting()
    {
        base.SetupEffectPools();
        monData.capsuleCollider2D.isTrigger = true;

        monData.maxHP = 20;
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

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 1f, LayerMask.GetMask("Platform"));
        Debug.DrawRay(transform.position, dir * 1f,Color.red);

        if(hit.collider != null)
        {
            monData.rigidbody2D.velocity = Vector2.zero;
        }
    }

    private IEnumerator Idle()
    {
        float time = 0;
        float dura = 0.5f;
        while (time < dura)
        {
            time += Time.deltaTime;

            yield return null;
        }
        ChangeState(State.Wander);
    }

    private void SetNewDestination()
    {
        Vector2 randomPoint = new Vector2(transform.position.x + Random.Range(-radius,radius),
                                          transform.position.y + Random.Range(-radius, radius));

        Vector2 newDir = randomPoint - (Vector2)transform.position;
        dir = newDir.normalized;
    }
    private IEnumerator Wander()
    {
        float time = 0;
        float dura = Random.Range(0.5f, 2f);
        SetNewDestination();

        while (time < dura)
        {
            time += Time.deltaTime;

            transform.Translate(dir * monData.moveSpeed * Time.deltaTime);
            
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 1f, LayerMask.GetMask("Platform"));
            Debug.DrawRay(transform.position, dir * 1f, Color.red);

            if (hit.collider != null)
            {
                ChangeState(State.Idle);
            }

            if (time >= dura)
            {
                // Wander 횟수 증가
                wanderCount++;
            }
            yield return null;
        }
        if(wanderCount >= 3) ChangeState(State.Attack);
        else                 ChangeState(State.Idle);
    }

    private IEnumerator Attack()
    {
        AudioManager.Instance.PlaySFX("Bat");
        monData.animator.SetBool("IsAttack", true);

        yield return null;
    }

    public void CreateBatBullet()
    {
        Vector3 target = PlayerController.instance.transform.position;
        Vector3 dir = (target - transform.position).normalized;

        ActivateBullet(dir);
    }
    public void CutAni()
    {
        monData.animator.SetBool("IsAttack", false);
        wanderCount = 0;
        ChangeState(State.Idle);
    }

    private void ActivateBullet(Vector3 dir)
    {
        GameObject bullet = bulletPool.ActivePoolItem();
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity;
        bullet.GetComponent<BatBullet>().Setup(bulletPool, dir);
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
        if (PlayerController.instance.transform.position.x > transform.position.x)
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

        // 이전에 재생하던 상태 종료 
        StopCoroutine(monState.ToString());

        // 상태 변경
        monState = newState;

        // 새로운 상태 재생
        StartCoroutine(monState.ToString());
    }
}
