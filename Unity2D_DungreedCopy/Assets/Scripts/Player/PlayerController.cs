using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayerState { Idle = 0, Walk, Jump, Die }   // YS: 플레이어 상태 
public class PlayerController : MonoBehaviour
{
    static public PlayerController instance;

    [Header("보스를 만났는지 여부")]
    public bool playerMeetsBoss;
    [Header("보스가 죽었는지 여부")]
    public bool isBossDie = false;
    [Header("보스가 완전히 죽었는지")]
    public bool bossOpentheStele = false;
    [Header("플레이어가 움직일 수 있는지")]
    public bool dontMovePlayer = false;
    [Header("플레이어가 음식을 먹었는지")]
    public bool eatFood = false;


    [Header("플레이어 공격 제어")]
    public bool canAttack;          // 플레이어가 공격을 할 수 있는지 여부

    public NPC curNPC;

    [Header("방향")]
    public float lastMoveDirX;
    private Vector3 seeDir;
    public Vector3 mousePos;

    [Header("피격")]
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

    [Header("현재 맵 이름")]
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

    private AudioManager audioManager;

    [SerializeField]
    private UIManager UIManager;

    private void Awake()
    {
        if (instance == null)
        {
            // YS : 씬 변경시에도 플레이어 파괴되지 않도록
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

            audioManager = AudioManager.Instance;
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
        // ▼ 실험용
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
    // YS: 플레이어 움직임
    //======================================================================================
    public void UpdateMove()
    {
        // 수평 이동
        float x = Input.GetAxis("Horizontal");

        // 정지 상태가 아닐 때
        if (x != 0)
        {
            // 현재 방향 대입
            lastMoveDirX = Mathf.Sign(x);
            movement.rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

        }
        // YS: x축 입렵이 없을시 경사면에서 미끄러지는 현상 수정
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
            audioManager.PlaySFX("Jump");
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
            audioManager.PlaySFX("Dash");
        }
    }

    private void DontMovePlayer(bool value)
    {
        // 멈춤과 동시에 공중에 있는 경우
        if (value && !movement.isGrounded)
        {
            movement.rigidbody.velocity = new Vector2(0, movement.rigidbody.velocity.y);
        }
        // 멈춤과 동시에 땅에 있는 경우
        else if (value && movement.isGrounded)
        {
            movement.rigidbody.velocity = Vector2.zero;
        }
    }
    //======================================================================================
    // YS: 플레이어 움직임 제외한 기능
    //======================================================================================

    // 외부에서 공격 제어를 위함
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
                audioManager.PlaySFX("Hit2");
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
    // YS: 플레이어 Collider
    //======================================================================================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ItemFairy" && playerStats.HP < playerStats.MaxHP)
        {
            collision.GetComponent<ItemBase>().Use(this.gameObject);
        }
    }
    //======================================================================================
    // YS: 플레이어 상태 변경
    //======================================================================================

    public void ChangeState(PlayerState newState)
    {
        playerState = newState;
    }

    public void ChangeAnimation()
    {
        // 걷는 상태
        if (movement.rigidbody.velocity.x != 0)
        {
            ChangeState(PlayerState.Walk);
            ani.SetFloat("MoveSpeed", movement.rigidbody.velocity.x);
        }
        // 점프 상태
        if (movement.isJump == true)
        {
            ChangeState(PlayerState.Jump);
            ani.SetBool("IsJump", true);
        }

        // 죽는 상태
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

        // 기본 상태
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
