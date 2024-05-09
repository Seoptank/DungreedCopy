using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// YS
public class PoolManager : MonoBehaviour
{
    private class PoolItem
    {
        public bool isActive;               // obj�� Ȱ��/��Ȱ�� ����   
        public GameObject gameObject;       // ȭ�鿡 ���̴� ���� obj
    }
    
    private int increaseCount = 1;          // obj ������ �� Instantiate()�� �߰� ������ obj ����
    private int maxCount;                   // ���� ����Ʈ�� ��ϵǾ��ִ� obj ����
    private int activeCount;                // Ȱ��ȭ�� obj ����
    
    private GameObject poolObj;             // ������Ʈ Ǯ������ �����ϴ� obj ������
    private List<PoolItem> poolItemList;    // �����Ǵ� ��� obj�� �����ϴ� List
    
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
