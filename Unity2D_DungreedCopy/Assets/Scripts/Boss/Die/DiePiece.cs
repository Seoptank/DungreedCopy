using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiePiece : MonoBehaviour
{
    private float       originTimeScale = 1.0f;

    [Header("Item »ý¼º º¯¼ö")]
    [SerializeField]
    private float       forceX;
    [SerializeField]
    private float       forceY;
    [SerializeField]
    private GameObject  BullionPrefab;
    [SerializeField]
    private GameObject  fairyXLPrefab;

    private PoolManager      BullionPoolManager;
    private PlayerController playerController;

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        BullionPoolManager = new PoolManager(BullionPrefab);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ¶¥¿¡ ´êÀº °æ¿ì
        if (collision.gameObject.layer == 6 || collision.gameObject.layer == 9)
        {
            Time.timeScale = originTimeScale;
            playerController.isBossDie = false;
            StartCoroutine("SpawnFairyXL");
            StartCoroutine("SpawnCoin");
        }
    }
    private IEnumerator SpawnCoin()
    {
        yield return new WaitForSeconds(2.5f);

        for (int i = 0; i < 20; i++)
        {
            Vector3 targetPos = new Vector3(Random.Range(-forceX, forceX), Random.Range(0f, forceY), 0);
            Vector3 dir = targetPos - transform.position;

            GameObject item = BullionPoolManager.ActivePoolItem();
            item.transform.position = transform.position;
            item.transform.rotation = transform.rotation;
            item.GetComponent<GoldItemController>().Setup(BullionPoolManager, dir);

            playerController.bossOpentheStele = true;
        }
        yield return null;
    }

    private IEnumerator SpawnFairyXL()
    {
        yield return new WaitForSeconds(2f);

        GameObject temp = Instantiate(fairyXLPrefab);
        temp.transform.position = new Vector2(transform.position.x, transform.position.y + 3);
        temp.transform.rotation = temp.transform.rotation;
    }
}
