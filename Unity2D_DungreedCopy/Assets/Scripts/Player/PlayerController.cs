using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayerState { Idle = 0, Walk, Jump, Die }   // YS: �÷��̾� ���� 
public class PlayerController : MonoBehaviour
{
    static public PlayerController instance;

    [Header("������ �������� ����")]
    public bool playerMeetsBoss;
    [Header("������ �׾����� ����")]
    public bool isBossDie = false;
    [Header("������ ������ �׾�����")]
    public bool bossOpentheStele = false;
    [Header("�÷��̾ ������ �� �ִ���")]
    public bool dontMovePlayer = false;
    [Header("�÷��̾ ������ �Ծ�����")]
    public bool eatFood = false;


    [Header("�÷��̾� ���� ����")]
    public bool canAttack;          // �÷��̾ ������ �� �� �ִ��� ����

    public NPC curNPC;

    [Header("����")]
    public float lastMoveDirX;
    private Vector3 seeDir;
    public Vector3 mousePos;

    [Header("�ǰ�")]
    [SerializeField]
    public bool isHurt;
    [SerializeField]
    private float hurtRoutineDuration = 2f;
    [SerializeField]
    private float blinkDuration = 0.5f;

    private Color halfA = new Color(1, 1, 1, 0.5f);
    private Color fullA = new Color(1, 1, 1, 1);
    public bool isDie;
    [SerializeField]
    private KeyCode jumpKey = KeyCode.Space;
    [SerializeField]
    private KeyCode dashKey = KeyCode.Mouse1;

    public PlayerState playerState;

    [Header("���� �� �̸�")]
    public string curSceneName;
    public string curDungeonName;
    public int curDungeonNum;
    [SerializeField]
    private Transform startPos;

    public Movement2D movement;
    public Animator ani;
    public SpriteRenderer spriteRenderer;
    public GameObject weaponDatabase;
    public SpriteRenderer weaponRenderer;
    private PlayerStats playerStats;
    private CapsuleCollider2D capsulCollider2D;

    private DungeonPortalController dungeonPortalController;

    [SerializeField]
    private UIManager UIManager;

    private void Awake()
    {
        if (instance == null)
        {
            // YS : �� ����ÿ��� �÷��̾� �ı����� �ʵ���
            DontDestroyOnLoad(gameObject);

            ChangeState(PlayerState.Idle);
            movement = GetComponent<Movement2D>();
            ani = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            playerStats = GetComponent<PlayerStats>();
            capsulCollider2D = GetComponent<CapsuleCollider2D>();
            weaponRenderer = weaponDatabase.GetComponentInChildren<SpriteRenderer>();


            dungeonPortalController = FindObjectOfType<DungeonPortalController>();

            transform.position = startPos.position;

            curSceneName = SceneManager.GetActiveScene().name;

            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        isDie = false;
        canAttack = true;
    }
    private void Update()
    {
        DontMovePlayer(dontMovePlayer);


        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        ChangeAnimation();
        CheckNPC();

        if (dontMovePlayer == false)
        {
            if (!isDie && !dontMovePlayer && !PlayerDungeonData.instance.isMoving)
            {
                capsulCollider2D.offset = new Vector2(0, -0.08f);
                capsulCollider2D.size = new Vector2(0.8f, 1.2f);
                UpdateMove();
                UpdateJump();
                UpdateSight();
                UpdateDash();
            }
            else if (isDie)
            {
                capsulCollider2D.offset = new Vector2(0, 0.1f);
                capsulCollider2D.size = Vector2.one;
            }
        }


        if (movement.isDashing) return;


        //########################################################################################
        // �� �����
        //########################################################################################

        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(PlayerStats.instance.MaxHP);
            StartCoroutine(HurtRoutine());
            StartCoroutine(BlinkPlayer());

            if (isDie)
            {
                StartCoroutine(movement.Die());
            }
        }

        weaponRenderer = weaponDatabase.GetComponentInChildren<SpriteRenderer>();

        //########################################################################################
        //########################################################################################

    }

    private void CheckNPC()
    {
        seeDir = new Vector3(lastMoveDirX, 0).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, seeDir, 2, LayerMask.GetMask("NPC"));
        Debug.DrawRay(transform.position, seeDir * 2, Color.red);

        if (hit.collider != null)
        {
            curNPC = hit.collider.gameObject.GetComponent<NPC>();
        }
    }


    //======================================================================================
    // YS: �÷��̾� ������
    //======================================================================================
    public void UpdateMove()
    {
        // ���� �̵�
        float x = Input.GetAxis("Horizontal");

        // ���� ���°� �ƴ� ��
        if (x != 0)
        {
            // ���� ���� ����
            lastMoveDirX = Mathf.Sign(x);
            movement.rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

        }
        // YS: x�� �Է��� ������ ���鿡�� �̲������� ���� ����
        else
        {
            movement.rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }

        movement.MoveTo(x);
        movement.isWalk = true;
    }

    public void UpdateJump()
    {
        if (Input.GetKeyDown(jumpKey))
        {
            bool isJump = movement.JumpTo();
        }
        else if (Input.GetKey(jumpKey))
        {
            movement.isLongJump = true;
        }
        else if (Input.GetKeyUp(jumpKey))
        {
            movement.isLongJump = false;
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (movement.curPassingPlatform != null)
            {
                StartCoroutine(movement.DownJumpTo(0.3f, 4));
            }
        }
    }
    public void UpdateSight()
    {
        if (mousePos.x < transform.position.x)
            transform.rotation = Quaternion.Euler(0, 180, 0);
        else
            transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void UpdateDash()
    {
        if (Input.GetKeyDown(dashKey) && movement.isDashing == false)
        {
            movement.PlayDash();
        }
    }

    private void DontMovePlayer(bool value)
    {
        // ����� ���ÿ� ���߿� �ִ� ���
        if (value && !movement.isGrounded)
        {
            movement.rigidbody.velocity = new Vector2(0, movement.rigidbody.velocity.y);
        }
        // ����� ���ÿ� ���� �ִ� ���
        else if (value && movement.isGrounded)
        {
            movement.rigidbody.velocity = Vector2.zero;
        }
    }
    //======================================================================================
    // YS: �÷��̾� ������ ������ ���
    //======================================================================================

    // �ܺο��� ���� ��� ����
    public IEnumerator AbleToAttack()
    {
        canAttack = false;

        float attackDelayTime = 1 / (PlayerStats.instance.ATS + PlayerStats.instance.WP_ATS);
        yield return new WaitForSeconds(attackDelayTime);

        canAttack = true;
    }

    public void TakeDamage(float mon_Att)
    {
        bool die = playerStats.DecreaseHP(mon_Att);

        if (die)
        {
            isDie = true;
            StopCoroutine(movement.Die());
            StartCoroutine(movement.Die());
        }
        else
        {
            if (!isHurt)
            {
                isHurt = true;
                StartCoroutine(BlinkPlayer());
                StartCoroutine(HurtRoutine());
            }
        }
        AudioManager.Instance.PlaySFX("Hit");
        MainCameraController.instance.OnShakeCamByPos(0.1f, 0.1f);
    }
    private IEnumerator HurtRoutine()
    {
        yield return new WaitForSeconds(hurtRoutineDuration);
        isHurt = false;
    }
    public IEnumerator BlinkPlayer()
    {
        while (isHurt)
        {
            yield return new WaitForSeconds(blinkDuration);
            spriteRenderer.color = halfA;
            yield return new WaitForSeconds(blinkDuration);
            spriteRenderer.color = fullA;
        }
    }

    //======================================================================================
    // YS: �÷��̾� Collider
    //======================================================================================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ItemFairy" && playerStats.HP < playerStats.MaxHP)
        {
            collision.GetComponent<ItemBase>().Use(this.gameObject);
        }
    }
    //======================================================================================
    // YS: �÷��̾� ���� ����
    //======================================================================================

    public void ChangeState(PlayerState newState)
    {
        playerState = newState;
    }

    public void ChangeAnimation()
    {
        // �ȴ� ����
        if (movement.rigidbody.velocity.x != 0)
        {
            ChangeState(PlayerState.Walk);
            ani.SetFloat("MoveSpeed", movement.rigidbody.velocity.x);
        }
        // ���� ����
        if (movement.isJump == true)
        {
            ChangeState(PlayerState.Jump);
            ani.SetBool("IsJump", true);
        }

        // �״� ����
        if (isDie)
        {
            ChangeState(PlayerState.Die);
            ani.SetBool("IsDie", true);


            if (!movement.isGrounded)
            {
                movement.rigidbody.velocity = new Vector2(0, movement.rigidbody.velocity.y);
            }
            else if (movement.isGrounded)
            {
                movement.rigidbody.velocity = Vector2.zero;
            }

        }

        // �⺻ ����
        if (movement.isGrounded == true && movement.rigidbody.velocity.x == 0)
        {
            ChangeState(PlayerState.Idle);
            ani.SetFloat("MoveSpeed", movement.rigidbody.velocity.x);
        }
        if (movement.isGrounded == true)
        {
            ani.SetBool("IsJump", false);
        }
    }

}
