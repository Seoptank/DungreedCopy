using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    private bool            activate = false;
    private float           blinkDura = 0.5f;
    private Color           halfA = new Color(1, 1, 1, 0.5f);
    private Color           fullA = new Color(1, 1, 1, 1);
    
    private SpriteRenderer  spriteRenderer;
    private PoolManager     pool;
    public void Setup(PoolManager newPool)
    {
        this.pool = newPool;

        spriteRenderer = GetComponent<SpriteRenderer>();

        activate = true;
        StartCoroutine(DeactivateEffect());
        StartCoroutine(Blink());
    }

    private void Update()
    {
        PlayerController player = PlayerController.instance;
        Vector3 playerPos = player.transform.position;

        transform.position = new Vector3(playerPos.x, playerPos.y + 1f);
    }

    private IEnumerator Blink()
    {
        while(activate)
        {
            yield return new WaitForSeconds(blinkDura);
            spriteRenderer.color = halfA;
            yield return new WaitForSeconds(blinkDura);
            spriteRenderer.color = fullA;
        }
    }

    private IEnumerator DeactivateEffect()
    {
        yield return new WaitForSeconds(3f);
        activate = false;
        pool.DeactivePoolItem(this.gameObject);
    }
}
