using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSwordSpawnEffect : MonoBehaviour
{
    [Header("Sword")]
    [SerializeField]
    private GameObject  SwordPrefab;

    private PoolManager thisPoolManager;
    private PoolManager SwordPoolManager;

    private BossPattern boss;
    private void Awake()
    {
        SwordPoolManager = new PoolManager(SwordPrefab);

        boss = FindObjectOfType<BossPattern>();
    }
    public void Setup(PoolManager poolManager)
    {
        this.thisPoolManager = poolManager;
    }
    public void DeactivateSpawnEffect()
    {
        thisPoolManager.DeactivePoolItem(gameObject);
    }
    public void SpawnSword()
    {
        GameObject sword = SwordPoolManager.ActivePoolItem();
        sword.transform.position = transform.position;
        sword.transform.rotation = transform.rotation;
        sword.GetComponent<BossSword>().Setup(SwordPoolManager);
    }
}
