using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSwordCharge : MonoBehaviour
{
    private Transform   playerTransform;

    private PoolManager poolManager;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.Log("Sword�� ã�� ���߽��ϴ�.");
        }
    }
    public void Setup(PoolManager poolManager)
    {
        this.poolManager = poolManager;
    }
    private void Update()
    {
        DirectionToPlayer();
    }
    private void DirectionToPlayer()
    {
        if (playerTransform == null) return;


        Vector3 dirToPlayer = playerTransform.position - transform.position;
        float angle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;

        // ȸ�� ����
        transform.rotation = Quaternion.AngleAxis(angle+90, Vector3.forward);
    }
}
