using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// YS
public class PoolManager : MonoBehaviour
{
    private class PoolItem
    {
        public bool isActive;               // obj의 활성/비활성 정보   
        public GameObject gameObject;       // 화면에 보이는 실제 obj
    }
    
    private int increaseCount = 1;          // obj 부족할 때 Instantiate()로 추가 생성할 obj 개수
    private int maxCount;                   // 현재 리스트에 등록되어있는 obj 개수
    private int activeCount;                // 활성화된 obj 개수
    
    private GameObject poolObj;             // 오브젝트 풀링에서 관리하는 obj 프리팹
    private List<PoolItem> poolItemList;    // 관리되는 모든 obj를 저장하는 List
    
    public PoolManager(GameObject poolObj)
    {
        maxCount = 0;
        activeCount = 0;
        this.poolObj = poolObj;
    
        poolItemList = new List<PoolItem>();
    
        InstantiateObjects();
    }
    public void InstantiateObjects()
    {
        maxCount += increaseCount;
    
        for (int i = 0; i < increaseCount; ++i)
        {
            PoolItem poolItem = new PoolItem();
    
            poolItem.isActive = false;
            poolItem.gameObject = GameObject.Instantiate(poolObj);
            poolItem.gameObject.SetActive(false);
    
            poolItemList.Add(poolItem);
        }
    }
    public void DestroyObjcts()
    {
        if (poolItemList == null) return;
    
        int count = poolItemList.Count;
        for (int i = 0; i < count; ++i)
        {
            GameObject.Destroy(poolItemList[i].gameObject);
        }
    
        poolItemList.Clear();
    }
    public GameObject ActivePoolItem()
    {
        if (poolItemList == null) return null;
    
        if (maxCount == activeCount)
        {
            InstantiateObjects();
        }
    
        int count = poolItemList.Count;
        for (int i = 0; i < count; ++i)
        {
            PoolItem poolItem = poolItemList[i];
    
            if (poolItem.isActive == false)
            {
                activeCount++;
    
                poolItem.isActive = true;
                poolItem.gameObject.SetActive(true);
    
                return poolItem.gameObject;
            }
        }
    
        return null;
    }
    
    public void DeactivePoolItem(GameObject removeObject)
    {
        if (poolItemList == null || removeObject == null) return;
    
        int count = poolItemList.Count;
        for (int i = 0; i < count; ++i)
        {
            PoolItem poolItem = poolItemList[i];
    
            if (poolItem.gameObject == removeObject)
            {
                activeCount--;
    
                poolItem.isActive = false;
                poolItem.gameObject.SetActive(false);
    
                return;
            }
        }
    }
    public void DeactiveAllPoolItems()
    {
        if (poolItemList == null) return;
    
        int count = poolItemList.Count;
        for (int i = 0; i < count; ++i)
        {
            PoolItem poolItem = poolItemList[i];
    
            if (poolItem.gameObject != null && poolItem.isActive == true)
            {
                poolItem.isActive = false;
                poolItem.gameObject.SetActive(false);
            }
        }
    
        activeCount = 0;
    }
}
