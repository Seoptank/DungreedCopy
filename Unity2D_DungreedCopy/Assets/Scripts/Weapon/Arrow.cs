using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField]
    private GameObject  hitEffect;
    [SerializeField]
    private float       deactivateTime;
        [SerializeField]
    private bool ispierce;
    private PoolManager poolManager;
    private PoolManager hitEffectPoolManager;

    public void Setup(PoolManager newPool)
    {
        this.poolManager = newPool;

        hitEffectPoolManager = new PoolManager(hitEffect);

        StartCoroutine(DeactivateArrow());
    }

    private void OnApplicationQuit()
    {
        hitEffectPoolManager.DestroyObjcts();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!ispierce)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                poolManager.DeactivePoolItem(this.gameObject);

                HitEffect();
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Platform") && collision.gameObject.tag == "Platform")
            {
                poolManager.DeactivePoolItem(this.gameObject);
                HitEffect();
            }
        }
        else if(ispierce)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                HitEffect();
                if (collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
                {
                    poolManager.DeactivePoolItem(this.gameObject);
                }
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
            {
                HitEffect();

                poolManager.DeactivePoolItem(this.gameObject);
            }

        }
        
    }
    private IEnumerator DeactivateArrow()
    {
        yield return new WaitForSeconds(deactivateTime);
        poolManager.DeactivePoolItem(this.gameObject);
    }
    private void HitEffect()
    {
        GameObject hitEffect = hitEffectPoolManager.ActivePoolItem();
        hitEffect.transform.position = transform.position;
        hitEffect.transform.rotation = transform.rotation * Quaternion.Euler(0,0,90);
        hitEffect.GetComponent<RangedAttackHitEffect>().Setup(hitEffectPoolManager);
    }
}
