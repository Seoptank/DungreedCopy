using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackHitEffect : MonoBehaviour
{
    private PoolManager     poolManager;


    public void Setup(PoolManager pool)
    {
        this.poolManager = pool;
    }

    public void DeactivateEffect()
    {
        poolManager.DeactivePoolItem(gameObject);
    }
}
