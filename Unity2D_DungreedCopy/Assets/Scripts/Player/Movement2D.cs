using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Movement2D : MonoBehaviour
{
    [Header("MoveX,Jump")]
    [SerializeField]
    private float moveSpeed = 3.0f;
    [SerializeField]
    private float jumpForce = 8.0f;
    [SerializeField]
    private float lowGravity;           // ����Ű�� ���� ������ ������ ����Ǵ� ���� �߷�
    [SerializeField]
    private float highGravity;          // �Ϲ������� ����Ǵ� ���� 
    public bool isJump = false;         // Jump���� äũ
    public bool isdownJump = false;     // Jump���� äũ
    public bool isWalk = false;         // Walk���� äũ
    [SerializeField]
    private float downJumpTime;


    [Header("DoubleJump")]
    public bool haveDoubleJump;
    [SerializeField]
    private int haveDoubleJump_MaxJumpCount = 2;
    [SerializeField]
    private int normalState_MaxJumpCount = 1;
    [SerializeField]
    private int curJumpCount;

    [Header("Checking Slope")]
    [SerializeField]
    private float dis;
    [SerializeField]
    private float angle;
    [SerializeField]
    private float maxAngle;   // YS: �ִ� ������ ������ �� ���� �̻����δ� ���ö󰡰� ������ �� ����
    [SerializeField]
    private bool isSlope = false;
    [SerializeField]
    private Vector2 prep;

    [Header("Checking Ground")]
    [SerializeField]
    private LayerMask collisionLayer;
    public bool isGrounded;
    [SerializeField]
    private Transform footPos;
    [SerializeField]
    private float checkRadius;
    private bool isDownJump;

    [Header("Dash")]
    public bool isDashing = false;
    public float dashDis = 3.0f;
    [SerializeField]
    private float dashSpeed = 20.0f;
    public float ghostDelay;
    [SerializeField]
    private float ghostDelaySeconds = 1.0f;
    [SerializeField]
    private GameObject dashPrefab;
    [SerializeField]
    public Vector3 dashDir;
    private PoolManager dashPoolManager;

    [Header("Dash Count")]
    public int maxDashCount = 3;
    public int curDashCount;
    public float dashCountChargeDelayTime = 5.0f;

    [Header("DustEffect")]
    private PoolManager dustPoolManager;
    [SerializeField]
    private GameObject dustPrefab;
    [SerializeField]
    private bool isSpawningDust = false;

    [Header("JumpEffect")]
    private PoolManager jumpDustPoolManager;
    [SerializeField]
    private GameObject jumpDustPrefab;

    [Header("DoubleJumpEffect")]
    private PoolManager doubleJumpDustPoolManager;
    [SerializeField]
    private GameObject doubleJumpDustPrefab;

    [Header("DieEffect")]
    private PoolManager dieEffectPoolManager;
    private PoolManager dieEffect2PoolManager;
    [SerializeField]
    private GameObject dieEffect2Prefab;
    [SerializeField]
    private GameObject dieEffectPrefab;
    [SerializeField]
    private GameObject dieUI;


    public GameObject curPassingPlatform;

    public bool isLongJump { set; get; } = false;

    [HideInInspector]
    public Rigidbody2D rigidbody;
    [HideInInspector]
    public CapsuleCollider2D capsulCollider2D;
    private PlayerStats playerStats;


    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        capsulCollider2D = GetComponent<CapsuleCollider2D>();
        playerStats = GetComponent<PlayerStats>();

        dashPoolManager = new PoolManager(dashPrefab);
        dustPoolManager = new PoolManager(dustPrefab);
        jumpDustPoolManager = new PoolManager(jumpDustPrefab);
        doubleJumpDustPoolManager = new PoolManager(doubleJumpDustPrefab);
        dieEffectPoolManager = new PoolManager(dieEffectPrefab);
        dieEffect2PoolManager = new PoolManager(dieEffect2Prefab);
    }
    private void Start()
    {
        ghostDelaySeconds = ghostDelay;

        // YS: Dash���� �ʱ�ȭ
        curDashCount = maxDashCount;
    }

    private void OnApplicationQuit()
    {
        dashPoolManager.DestroyObjcts();
        dustPoolManager.DestroyObjcts();
        jumpDustPoolManager.DestroyObjcts();
        doubleJumpDustPoolManager.DestroyObjcts();
        dieEffectPoolManager.DestroyObjcts();
        dieEffect2PoolManager.DestroyObjcts();
    }
    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, Vector2.down, dis, collisionLayer);
        CheckSlope(hit);

        GroundCheckAndJumpType();

        if (isDashing)
        {
            ActiveDashEffect();
        }

        // ���� ��ġ���� �������� ���� ���� �׸���, �ش� �� ���ο� �ִ� ������Ʈ�� ����
        Color rayColor = Color.red;
        RaycastHit2D hitPassing = Physics2D.Raycast(transform.position,Vector2.down,1,collisionLayer);
        Debug.DrawRay(transform.position, Vector2.down * 1, rayColor);

        if (hitPassing.collider != null && hitPassing.collider.CompareTag("PassingPlatform"))
        {
            curPassingPlatform = hitPassing.collider.gameObject; // �浹�� ������Ʈ�� curPassingPlatform�� ����
        }
        else
        {
            curPassingPlatform = null; // ����� ������Ʈ�� ������ curPassingPlatform�� null�� ����
        }
    }
    public void MoveTo(float x)
    {
        if (isSlope && isGrounded && !isJump && angle < maxAngle)
            rigidbody.velocity = prep * moveSpeed * x * -1f;
        else if (!isSlope && isGrounded && !isJump)
            rigidbody.velocity = new Vector2(x * moveSpeed, 0);
        else if (!isGrounded)
            rigidbody.velocity = new Vector2(x * moveSpeed, rigidbody.velocity.y);


        // DustEffect Active
        if (!isSpawningDust)
        {
            StartCoroutine("ActiveDustEffect");
        }
    }

    public bool JumpTo()
    {
        if (curJumpCount > 0)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpForce);
            curJumpCount--;
            isJump = true;
            isWalk = false;

            if (haveDoubleJump == true && curJumpCount < 1 && Input.GetKeyDown(KeyCode.Space))
            {
                ActiveDoubleJumpDustEffect();
            }

            if (isGrounded)
            {
                ActiveJumpDustEffect();
            }

            return true;

        }
        return false;
    }

    public IEnumerator DownJumpTo(float time, float speed)
    {
        CompositeCollider2D platformCollider = curPassingPlatform.GetComponent<CompositeCollider2D>();

        Physics2D.IgnoreCollision(capsulCollider2D, platformCollider, true);
        float elapsedTime = 0f;
        isDownJump = true;
        while (elapsedTime < time)
        {
            // ������ �ӵ��� �Ʒ��� �̵�
            transform.Translate(speed * Vector3.down * Time.deltaTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Physics2D.IgnoreCollision(capsulCollider2D, platformCollider, false);
    }

    private void CheckSlope(RaycastHit2D hit)
    {
        if (hit)
        {
            // YS: Vector2.Perpendicular(Vector2 A)�� ������ "�ݽð� ����"���� 90�� ȸ����
            //     ���Ͱ��� ��ȯ�Ѵ�.

            // YS: hit.normal�� �浹�� �������� �鿡 ������ ���� ������.
            prep = Vector2.Perpendicular(hit.normal).normalized;
            angle = Vector2.Angle(hit.normal, Vector2.up);

            // YS: ������ 0�� �ƴ����� ����϶�
            if (angle != 0) isSlope = true;
            else isSlope = false;

            Debug.DrawLine(hit.point, hit.point + hit.normal, Color.green);
            Debug.DrawLine(hit.point, hit.point + prep, Color.red);
        }
    }
    private void GroundCheckAndJumpType()
    {
        isGrounded = Physics2D.OverlapCircle(footPos.position, checkRadius, collisionLayer);

        if (isGrounded == true && rigidbody.velocity.y <= 0)
        {
            isJump = false;
            isdownJump = false;

            if (haveDoubleJump == true)
            {
                curJumpCount = haveDoubleJump_MaxJumpCount;
            }
            else if (haveDoubleJump == false)
            {
                curJumpCount = normalState_MaxJumpCount;
            }
        }

        if (isLongJump && rigidbody.velocity.y > 0)
        {
            rigidbody.gravityScale = lowGravity;
        }
        else
        {
            rigidbody.gravityScale = highGravity;
        }
    }
    public void PlayDash()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        dashDir = mousePos - transform.position;
        Vector3 moveTarget = transform.position + Vector3.ClampMagnitude(dashDir, dashDis);

        if (playerStats.DC > 0)
        {
            StartCoroutine(DashTo(moveTarget));
            playerStats.UseDC();
        }
    }
    private IEnumerator DashTo(Vector3 moveTarget)
    {
        isDashing = true;
        curDashCount--;
        AudioManager.Instance.PlaySFX("Dash");

        float dis = Vector3.Distance(transform.position, moveTarget);
        float step = (dashSpeed / dis) * Time.fixedDeltaTime;
        float t = 0f;

        Vector3 startingPos = transform.position;
        if (curPassingPlatform != null)
        {
            StartCoroutine(StatePassingPlatformInDasing());
        }

        while (t <= 1.0f)
        {
            t += step;
            rigidbody.MovePosition(Vector3.Lerp(startingPos, moveTarget, t));
            yield return new WaitForFixedUpdate();
        }
        playerStats.timer = 0;
        isDashing = false;
    }

    private IEnumerator StatePassingPlatformInDasing()
    {

        if (curPassingPlatform != null)
        {
            CompositeCollider2D platformCollider = curPassingPlatform.GetComponent<CompositeCollider2D>();

            while (isDashing)
            {
                Physics2D.IgnoreCollision(capsulCollider2D, platformCollider, true);

                if(curPassingPlatform == null)
                {
                    Physics2D.IgnoreCollision(capsulCollider2D, platformCollider, false);
                }

                yield return null;
            }
            Physics2D.IgnoreCollision(capsulCollider2D, platformCollider, false);
        }
    }
    //=====================================================================
    // YS: Player Effect Active
    //=====================================================================
    private IEnumerator ActiveDustEffect()
    {
        isSpawningDust = true;

        while (rigidbody.velocity.x != 0 && !isJump)
        {
            AudioManager.Instance.PlaySFX("Step");
            GameObject dustEffect = dustPoolManager.ActivePoolItem();
            dustEffect.transform.position = transform.position + new Vector3(0, -0.25f, -1f);
            dustEffect.transform.rotation = transform.rotation;
            dustEffect.GetComponent<EffectPool>().Setup(dustPoolManager);
            yield return new WaitForSeconds(0.3f);
        }
        isSpawningDust = false;
    }
    public void ActiveJumpDustEffect()
    {
        AudioManager.Instance.PlaySFX("Jump");
        GameObject jumpDustEffect = jumpDustPoolManager.ActivePoolItem();
        jumpDustEffect.transform.position = transform.position + new Vector3(0, -0.25f, 0);
        jumpDustEffect.transform.rotation = transform.rotation;
        jumpDustEffect.GetComponent<EffectPool>().Setup(jumpDustPoolManager);
    }
    public void ActiveDoubleJumpDustEffect()
    {
        AudioManager.Instance.PlaySFX("Jump");
        GameObject doubleJumpDustEffect = doubleJumpDustPoolManager.ActivePoolItem();
        doubleJumpDustEffect.transform.position = transform.position + new Vector3(0, -0.25f, 0);
        doubleJumpDustEffect.transform.rotation = transform.rotation;
        doubleJumpDustEffect.GetComponent<EffectPool>().Setup(doubleJumpDustPoolManager);
    }

    private IEnumerator ActiveDieEffect()
    {
        PlayerController.instance.spriteRenderer.color = new Color(1, 1, 1, 0);
        PlayerController.instance.weaponRenderer.color = new Color(1, 1, 1, 0);
        GameObject dieEffect = dieEffectPoolManager.ActivePoolItem();
        dieEffect.transform.position = transform.position;
        dieEffect.transform.rotation = transform.rotation;
        dieEffect.GetComponent<EffectPool>().Setup(dieEffectPoolManager);
        yield return new WaitForSeconds(0.5f);
        GameObject dieEffect2 = dieEffect2PoolManager.ActivePoolItem();
        dieEffect2.transform.position = transform.position + new Vector3(0, 1.6f);
        dieEffect2.transform.rotation = transform.rotation;
        dieEffect2.GetComponent<EffectPool>().Setup(dieEffect2PoolManager);
    }



    public IEnumerator Die()
    {
        AudioManager.Instance.PlaySFX("Fail");
        AudioManager.Instance.OffMusic();

        // �÷��̾ ���� �ð� ���
        PlayerDungeonData.instance.deathTime = Time.time;

        // �÷��̾ ������� 1�� ��
        yield return new WaitForSeconds(1);
        
        PlayerDungeonData.instance.totalTime = PlayerDungeonData.instance.deathTime - PlayerDungeonData.instance.enterTime;

        // GameOverUI Ȱ��ȭ
        dieUI.SetActive(true);

        // dieEffect Ȱ��ȭ
        StartCoroutine(ActiveDieEffect());
    }



    private void ActiveDashEffect()
    {
        if (ghostDelaySeconds > 0)
        {
            ghostDelaySeconds -= Time.deltaTime;
        }
        else
        {
            GameObject ghostEffect = dashPoolManager.ActivePoolItem();
            ghostEffect.transform.position = transform.position;
            ghostEffect.transform.rotation = transform.rotation;
            ghostEffect.GetComponent<EffectPool>().Setup(dashPoolManager);
            Sprite curSprite = GetComponent<SpriteRenderer>().sprite;
            ghostEffect.GetComponent<SpriteRenderer>().sprite = curSprite;
            ghostDelaySeconds = ghostDelay;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(footPos.position, checkRadius);
    }
}
