using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldItemController : MonoBehaviour
{
    public int             GoldValue;
    [SerializeField]
    private GameObject      textGoldPrefab;
    private Color           rayColor;
    private bool            isGround;

    private Rigidbody2D         rigid;
    private PoolManager         poolManager;
    private PoolManager         textGoldpoolManager;
    private CircleCollider2D    circleCollider2D;

    public void Setup(PoolManager newPool,Vector3 dir)
    {
        this.poolManager = newPool;

        rigid               = GetComponent<Rigidbody2D>();
        circleCollider2D    = GetComponent<CircleCollider2D>();

        rigid.velocity = new Vector3(dir.x, dir.y, 0);
    }

    private void Awake()
    {
        textGoldpoolManager = new PoolManager(textGoldPrefab);
    }
    private void FixedUpdate()
    {
        AddGravity();
        CheckWall();
        CheckCeiling();
    }

    private void CheckCeiling()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, circleCollider2D.radius, LayerMask.GetMask("Platform"));
        Debug.DrawRay(transform.position, Vector2.up * circleCollider2D.radius, rayColor);
        if (hit.collider != null)
        {
            rayColor = Color.green;
            rigid.velocity = new Vector2(0, 0);
            rigid.gravityScale = 2;
        }
    }

    private void CheckWall()
    {
        RaycastHit2D hitR = Physics2D.Raycast(transform.position, Vector2.right, circleCollider2D.radius, LayerMask.GetMask("Platform"));
        RaycastHit2D hitL = Physics2D.Raycast(transform.position, Vector2.left, circleCollider2D.radius, LayerMask.GetMask("Platform"));
        Debug.DrawRay(transform.position, Vector2.right * circleCollider2D.radius, rayColor);
        Debug.DrawRay(transform.position, Vector2.left * circleCollider2D.radius, rayColor);

        if (hitR.collider != null)
        {
            rayColor = Color.green;
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        }
        else
        {
            rayColor = Color.red;
            rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y);
        }

        if (hitL.collider != null)
        {
            rayColor = Color.green;
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        }
        else
        {
            rayColor = Color.red;
            rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y);
        }
    }

    private void AddGravity()
    {
        // 코인의 아래 방향으로 Ray를 쏨
        // collider로 하면 collider에 닿는 모든 부위를 검사하기 때문에 낙하시
        // 땅에 닿은 경우만 멈추도록 설정하기 위해 
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, circleCollider2D.radius*2, LayerMask.GetMask("Platform"));
        Debug.DrawRay(transform.position, Vector2.down * circleCollider2D.radius, rayColor);
        
        if (hit.collider != null)
        {
            isGround = true;
            rayColor = Color.green;
        }
        else
        {
            isGround = false;
            rayColor = Color.red;
        }

        if (isGround)
        {
            rigid.velocity = Vector2.zero;
            rigid.bodyType = RigidbodyType2D.Kinematic;
        }
        else
        {
            rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y);
            rigid.gravityScale = 2;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            // 플레이어에게 Bullion의 가치만큼 GOLD지급
            PlayerStats.instance.TakeGold(GoldValue);

            // Bullion Prefab 비활성화
            poolManager.DeactivePoolItem(gameObject);

            ActivateGoldText();

            AudioManager.Instance.PlaySFX("Coin");
        }
    }
    private void ActivateGoldText()
    {
        GameObject goldText = textGoldpoolManager.ActivePoolItem();
        goldText.transform.position = transform.position;
        goldText.transform.rotation = transform.rotation;
        goldText.GetComponent<TextGoldController>().Setup(textGoldpoolManager, GoldValue);
    }
}
