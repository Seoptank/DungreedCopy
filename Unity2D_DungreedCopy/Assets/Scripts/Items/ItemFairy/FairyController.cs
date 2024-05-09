using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyController : ItemBase
{
    [SerializeField]
    private GameObject  fairyEffectPrefab;
    [SerializeField]
    private GameObject  fairyTextEffectPrefab;

    public float        increaseHP;

    private PoolManager fairyEffectPoolManager;
    private PoolManager fairyTextEffectPoolManager;
    private PoolManager poolManager;
    private void Awake()
    {
        fairyEffectPoolManager = new PoolManager(fairyEffectPrefab);
        fairyTextEffectPoolManager = new PoolManager(fairyTextEffectPrefab);
    }
    private void OnApplicationQuit()
    {
        fairyEffectPoolManager.DestroyObjcts();
        fairyTextEffectPoolManager.DestroyObjcts();
    }

    public override void Use(GameObject entity)
    {
        AudioManager.Instance.PlaySFX("Heal");
        entity.GetComponent<PlayerStats>().IncreaseHP(increaseHP);
        ActiveFairyEffect();
        ActiveTextEffect();
        Destroy(this.gameObject);
    }

    private void ActiveFairyEffect()
    {
        GameObject fairyEffect = fairyEffectPoolManager.ActivePoolItem();
        fairyEffect.transform.position = transform.position;
        fairyEffect.transform.rotation = transform.rotation;
        fairyEffect.GetComponent<EffectPool>().Setup(fairyEffectPoolManager);
    }
    private void ActiveTextEffect()
    {
        GameObject textEffect = fairyTextEffectPoolManager.ActivePoolItem();
        textEffect.transform.position = transform.position;
        textEffect.transform.rotation = transform.rotation;
        textEffect.GetComponent<HealText>().Setup(fairyTextEffectPoolManager);
    }
}
