using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuckSword : MonoBehaviour
{
    [SerializeField]
    private float       time;
    [SerializeField]
    private GameObject deactivateEffectPrefab;

    private PoolManager poolManager;
    private PoolManager deactivateEffectpoolManager;
    public void Setup(PoolManager poolManager)
    {
        this.poolManager = poolManager;
    }

    private void Awake()
    {
        deactivateEffectpoolManager = new PoolManager(deactivateEffectPrefab);
    }

    private void Start()
    {
        StartCoroutine(DeactivateEffect());
    }
    private IEnumerator DeactivateEffect()
    {
        yield return new WaitForSeconds(time);
        poolManager.DeactivePoolItem(this.gameObject);
        GameObject deactivateEffect = deactivateEffectpoolManager.ActivePoolItem();
        deactivateEffect.transform.position = transform.position;
        deactivateEffect.transform.rotation = deactivateEffect.transform.rotation;
        deactivateEffect.GetComponent<EffectPool>().Setup(deactivateEffectpoolManager);
    }
}
