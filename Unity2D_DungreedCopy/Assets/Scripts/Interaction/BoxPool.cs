using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxPool : MonoBehaviour
{
    [SerializeField]
    private GameObject prefabItem;
    [SerializeField]
    private GameObject keyPrefab;
    private PoolManager itemPool;
    private Transform itemSpqwnPos;
    private bool onKey;
    public bool inputKey;

    private PoolManager poolManager;
    private Animator ani;
    private Rigidbody2D rigidBody;
    public void Setup(PoolManager poolManager)
    {
        this.poolManager = poolManager;
    }

    private void Awake()
    {
        ani = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();

        keyPrefab.SetActive(false);

        itemSpqwnPos = transform.GetChild(0).gameObject.GetComponent<Transform>();

        itemPool = new PoolManager(prefabItem);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            rigidBody.velocity = Vector2.zero;
            rigidBody.gravityScale = 0;
        }

        if (collision.gameObject.tag == "Player" && !inputKey)
        {
            onKey = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !inputKey)
        {
            onKey = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && onKey)
        {
            inputKey = true;
            onKey = false;

            ani.SetTrigger("IsOpen");
            SpawnRandomItem();
        }

        keyPrefab.SetActive(onKey);
    }

    private void SpawnRandomItem()
    {
        GameObject item = itemPool.ActivePoolItem();
        item.transform.position = itemSpqwnPos.position;
        item.transform.rotation = transform.rotation;
        item.GetComponent<RandomItemCreator>().Setup(itemPool);
    }
}
