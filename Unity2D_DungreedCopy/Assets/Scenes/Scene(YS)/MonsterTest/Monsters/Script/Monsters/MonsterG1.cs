using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterG1 : Test_Monster
{
    public enum State
    {
        None,
        Idle,
        Chase,
        Attack,
        Die,
    }
    public State monState;

    //�����Ӱ���
    [SerializeField]
    private float jumpForce;
    private float gravity = -9.8f;
    private Vector3 vel = Vector3.zero;
    private Vector3 seeDir = Vector3.zero;
    private bool Jumping;
    private Color colorDebugGround;

    // �Ÿ�
    [SerializeField]
    private float chaseDis;
    [SerializeField]
    private float attackDis;

    // ���� ����
    private GameObject attackCollider;
    private BoxCollider2D attackBoxCollider;
    private const float attackCooldown = 3f; // ���� ��ٿ� �ð�
    private float lastAttackTime; // ������ ���� �ð�

    private PoolManager pool;

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

        attackCollider = transform.GetChild(2).gameObject;
        attackBoxCollider = attackCollider.GetComponent<BoxCollider2D>();
        attackBoxCollider.enabled = false;

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
        // ��
        CheckCeiling();

        float rayDis = monData.capsuleCollider2D.size.y * 0.6f;

        // �ٴ��� ���� ���� �߻�
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
            vel.y += (gravity * 2) * Time.deltaTime;
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
        monData.animator.SetBool("IsMove", false);

        while (true)
        {
            vel.x = 0;

            // Idle �� ��� �ൿ
            CalculateDisToTargetAndselectState();

            yield return null;
        }
    }

    // ��
    private IEnumerator Chase()
    {

        monData.animator.SetBool("IsMove", true);
        while (true)
        {
            // �Ÿ��� ���� ���� ��ȭ
            CalculateDisToTargetAndselectState();

            PlayerController player = PlayerController.instance;
            Vector3 target = player.transform.position;

            // Ÿ���� �ٶ󺸵���
            UpdateSight();

            // ���� �ִ���
            CheckWall();

            //transform.Translate(seeDir * monData.moveSpeed * Time.deltaTime);

            yield return null;
        }
    }


    private void CheckWall()
    {
        // Raycast �߻縦 ���� �������� ���� ����
        Vector3 rayOrigin = transform.position; // ĳ���Ϳ��� ����
        Vector3 rayDir = seeDir;                // ���� ���� = �ٶ󺸴� ����

        // Raycast �߻�
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDir, 1f, LayerMask.GetMask("Platform"));

        // ����׸� ���� Ray �׸���
        Debug.DrawRay(rayOrigin, rayDir * 1f, Color.red);

        // Platform �浹 Ȯ��
        if (hit.collider != null)
        {
            vel.x = 0;

            if (monData.isGround && !Jumping)
            {
                Jump();
            }
        }
        else
        {
            // Ÿ���� �������� �̵�
            vel.x = seeDir.x * monData.moveSpeed;
        }
    }
    // ��
    private void CheckCeiling()
    {
        // Raycast �߻�
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 1f, LayerMask.GetMask("Platform"));
        Color rayColor = Color.red;
        // ����׸� ���� Ray �׸���
        Debug.DrawRay(transform.position, Vector2.up * 1f, rayColor);

        if (hit.collider != null && hit.collider.CompareTag("Platform"))
        {
            vel.y = 0;
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }
    }

    private void Jump()
    {
        vel.y = jumpForce;
        Jumping = true;
    }

    private IEnumerator Attack()
    {
        monData.animator.SetBool("IsAttack", true);
        while (true)
        {
            vel.x = 0;

            // ���� �ٶ󺸰� ���� ��
            if (monData.spriteRenderer.flipX)
            {
                attackCollider.transform.localPosition = new Vector2(-1.5f, attackCollider.transform.localPosition.y);
            }
            // ������ �ٶ󺸰� ���� ��
            else
            {
                attackCollider.transform.localPosition = new Vector2(1.5f, attackCollider.transform.localPosition.y);
            }

            yield return null;
        }
        monData.animator.SetBool("IsAttack", false);
        ChangeState(State.Idle);
        lastAttackTime = Time.time;
    }
    public void EnableAttackCollider()
    {
        attackBoxCollider.enabled = true;
        AudioManager.Instance.PlaySFX("Claw");
    }
    public void DisableAttackCollider()
    {
        attackBoxCollider.enabled = false;
    }

    public void CutAni()
    {
        monData.animator.SetBool("IsAttack", false);
        ChangeState(State.Idle);
        lastAttackTime = Time.time;
    }

    private void CalculateDisToTargetAndselectState()
    {
        PlayerController player = PlayerController.instance;

        if (player != null)
        {
            Vector3 target = player.transform.position;
            float dis = Vector2.Distance(target, transform.position);

            // �i���� �ִ� �Ÿ� ���� ������
            if (dis <= chaseDis && dis > attackDis)
            {
                ChangeState(State.Chase);
            }
            // ��� �Ÿ����� ����� => Idle
            else if (dis > chaseDis)
            {
                ChangeState(State.Idle);
            }
            else if (dis <= attackDis && Time.time >= lastAttackTime + attackCooldown)
            {
                if (Time.time >= lastAttackTime + attackCooldown)
                    ChangeState(State.Attack);
                else ChangeState(State.Chase);
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

    private void UpdateSight()
    {
        bool isRight = PlayerController.instance.transform.position.x >= transform.position.x;
        monData.spriteRenderer.flipX = !isRight;

        seeDir = isRight ? Vector3.right : Vector3.left;
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
