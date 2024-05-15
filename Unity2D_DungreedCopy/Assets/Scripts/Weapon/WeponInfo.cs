using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeponInfo : MonoBehaviour
{
    [Header("���� ����")]    
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
            // ���� ���ݷ� ���
            int randomATK = Random.Range(stats.WP_MINATK, stats.WP_MAXATK + 1);

            //ũ��Ƽ�� �ߵ���
            if (IsCritical())
            {
                // ũ��Ƽ�ý� ���ݷ� = �ִ� �������� + (�ִ빫������ * 0.5) + �÷��̾� ���ݷ�
                curATK = stats.WP_MAXATK + (int)(stats.WP_MAXATK * 0.5f) + stats.ATK;
                textColor = Color.yellow;
            }
            else
            {
                // �Ϲݰ��ݽ� ���ݷ� = �ִ� �������� + �÷��̾� ���ݷ�
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
