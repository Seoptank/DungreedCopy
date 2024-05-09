using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnManager : MonoBehaviour
{
    [Header("Item 생성 변수")]
    private float       forceY= 5f;
    private float       forceX= 3f;
    private int         minItemCount = 0;
    private int         maxItemCount;
    [SerializeField]
    private GameObject  SpawnItemPrefab;

    private PoolManager thisPool;
    private PoolManager ItemPoolManager;
    public void Setup(PoolManager pool,int maxCount)
    {
        this.thisPool = pool;
        maxItemCount = maxCount;

        ItemPoolManager = new PoolManager(SpawnItemPrefab);
        StartCoroutine(SpawnCoin());
        DeactivatePoolItem();
    }
    private IEnumerator DeactivatePoolItem()
    {
        yield return new WaitForSeconds(1);
        thisPool.DeactivePoolItem(this.gameObject);
    }

    private void OnApplicationQuit()
    {
        ItemPoolManager.DestroyObjcts();
    }

    private IEnumerator SpawnCoin()
    {
        int itemCount = Random.Range(minItemCount, maxItemCount);
        
        for (int i = 0; i < itemCount; ++i)
        {
            Vector3 targetPos   = new Vector3(transform.position.x + Random.Range(-forceX, forceX),
                                              transform.position.y + Random.Range(forceY *0.5f, forceY),
                                              0);
            Vector3 dir         = targetPos - transform.position; 

            GameObject item = ItemPoolManager.ActivePoolItem();
            item.transform.position = transform.position;
            item.transform.rotation = transform.rotation;
            item.GetComponent<GoldItemController>().Setup(ItemPoolManager, dir);
        }
        yield return null;
    }
}
