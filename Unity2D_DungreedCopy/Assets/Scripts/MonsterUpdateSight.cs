using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterUpdateSight : MonoBehaviour
{
    public Transform player;
    public bool isChase;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
    }
    // Update is called once per frame
    void Update()
    {
        if (isChase)
        {
            UpdateSight();
        }
    }

    void UpdateSight()
    {
        // �÷��̾� ��ġ���� ���� ��ġ������ ���� ����
        Vector3 directionToMonster = player.position - transform.position;

        // ������Ʈ�� �÷��̾� �������� z�� ȸ��
        float angle = Mathf.Atan2(directionToMonster.y, directionToMonster.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // �÷��̾� x��ǥ�� �о� scale���� -1�� �ٲپ� �̹��� ����
        Vector2 scale = transform.localScale;
        if (directionToMonster.x < 0)
        {
            scale.y = -1;
        }
        else
        {
            scale.y = 1;
        }
        transform.localScale = scale;
    }
}
