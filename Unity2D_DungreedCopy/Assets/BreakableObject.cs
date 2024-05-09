using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [SerializeField]
    private GameObject  piecesPrefab;
    [SerializeField]
    private GameObject  coinMachinePrefab;
    private bool        isGround;

    private PoolManager piecesPoolManager;
    private PoolManager coinMachinePoolManager;
    private Rigidbody2D rigid;

    private void Awake()
    {
        piecesPoolManager       = new PoolManager(piecesPrefab);
        coinMachinePoolManager  = new PoolManager(coinMachinePrefab);

        rigid = GetComponent<Rigidbody2D>();
    }

    private void OnApplicationQuit()
    {
        piecesPoolManager.DestroyObjcts();
        coinMachinePoolManager.DestroyObjcts();
    }

    private void Update()
    {
        if(isGround)
        {
            rigid.velocity = Vector2.zero;
            rigid.gravityScale = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "PlayerAttack")
        {
            BreakThis();
        }

        if(collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            isGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            isGround = false;
        }
    }

    private void BreakThis()
    {
        PoolPieces();

        AudioManager.Instance.PlaySFX("WoodCrash");

        PoolCoinMachine();

        Destroy(this.gameObject);
    }

    private void PoolPieces()
    {
        GameObject pieces = piecesPoolManager.ActivePoolItem();
        pieces.transform.position = transform.position;
        pieces.transform.rotation = transform.rotation;
        pieces.GetComponent<EffectPool>().Setup(piecesPoolManager);
    }
    private void PoolCoinMachine()
    {
        GameObject coinMachine = coinMachinePoolManager.ActivePoolItem();
        coinMachine.transform.position = transform.position;
        coinMachine.transform.rotation = transform.rotation;
        coinMachine.GetComponent<ItemSpawnManager>().Setup(piecesPoolManager,2);
    }
}
