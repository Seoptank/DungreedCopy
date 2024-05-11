using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCollider : MonoBehaviour
{
    private PlayerStats         playerStats;
    private BossPattern         bossPattern;

    private void Awake()
    {
        playerStats         = FindObjectOfType<PlayerStats>();

        Transform parentT = transform.parent;
        if(parentT != null)
        {
            Transform grandParentT = parentT.transform.parent;

            if (grandParentT != null)
                bossPattern = grandParentT.gameObject.GetComponent<BossPattern>();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Player" && !PlayerController.instance.isDie && !bossPattern.isDie)
        {
            PlayerController.instance.TakeDamage(10);
        }
    }
}
