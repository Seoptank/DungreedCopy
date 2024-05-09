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
        // 플레이어 위치에서 몬스터 위치까지의 방향 벡터
        Vector3 directionToMonster = player.position - transform.position;

        // 오브젝트를 플레이어 방향으로 z축 회전
        float angle = Mathf.Atan2(directionToMonster.y, directionToMonster.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 플레이어 x좌표를 읽어 scale값을 -1로 바꾸어 이미지 반전
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
