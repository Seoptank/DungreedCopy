using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI    ;

public class BossHP : MonoBehaviour
{
    public static BossHP instance;

    public float maxHP = 1000; 
    public float curHP;

    [SerializeField]
    private SpriteRenderer      spriteRenderer;

    private BossPattern             bossPattern;
    private BossController          bossController;

    private void Awake()
    {
        instance = this;

        bossPattern     = GetComponent<BossPattern>();
        bossController  = GetComponent<BossController>();

        curHP = maxHP;
    }
   
    private void BossTakeDamage(float damage)
    {
        curHP -= damage;

        StopCoroutine(HitColorAnimation());
        StartCoroutine(HitColorAnimation());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "PlayerAttack") 
        {
            BossTakeDamage(collision.gameObject.GetComponent<WeponInfo>().curATK);
            MainCameraController.instance.OnShakeCamByPos(0.1f, 0.1f);
        }

        if (collision.gameObject.tag == "Player" && PlayerController.instance.movement.isDashing)
        {
            BossTakeDamage(PlayerStats.instance.DashATK);
            MainCameraController.instance.OnShakeCamByPos(0.1f, 0.1f);
        }
    }


    private IEnumerator HitColorAnimation()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        spriteRenderer.color = Color.white;
    }

}
