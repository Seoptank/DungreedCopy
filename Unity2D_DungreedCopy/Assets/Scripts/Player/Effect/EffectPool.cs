using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPool : MonoBehaviour
{
    private PoolManager poolManager;
    public void Setup(PoolManager poolManager)
    {
        this.poolManager = poolManager;

        StartCoroutine(DeactivateRoutain());
    }

    private IEnumerator DeactivateRoutain()
    {
        yield return new WaitForSeconds(3f);
        DeactivateEffect();
    }

    public void DeactivateEffect()
    {
        poolManager.DeactivePoolItem(gameObject);
    }
}
