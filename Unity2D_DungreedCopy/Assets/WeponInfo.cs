using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeponInfo : MonoBehaviour
{
    [Header("무기 정보")]    
    [HideInInspector]
    public int      curATK;
    [HideInInspector]
    public Color    textColor;

    private System.Random random = new System.Random();

    private void OnEnable()
    {
        CalculateDamage();
    }
    private void CalculateDamage()
    {
        PlayerStats stats = PlayerStats.instance;

        if (stats != null)
        {
            // 랜덤 공격력 계산
            int randomATK = Random.Range(stats.WP_MINATK, stats.WP_MAXATK + 1);

            //크리티컬 발동시
            if (IsCritical())
            {
                // 크리티컬시 공격력 = 최대 무기피해 + (최대무기피해 * 0.5) + 플레이어 공격력
                curATK = stats.WP_MAXATK + (int)(stats.WP_MAXATK * 0.5f) + stats.ATK;
                textColor = Color.yellow;
            }
            else
            {
                // 일반공격시 공격력 = 최대 무기피해 + 플레이어 공격력
                curATK = randomATK + stats.ATK;
                textColor = Color.white;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Boss")
        {
            Debug.Log("zzzz");
        }
    }

    public bool IsCritical()
    {
        return (random.NextDouble() < PlayerStats.instance.CRI);
    }
}
