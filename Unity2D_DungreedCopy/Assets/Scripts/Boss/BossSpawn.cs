using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawn : MonoBehaviour
{
    [Header("º¸½º Prefab")]
    [SerializeField]
    private GameObject          bossPrefab;

    private CircleCollider2D    circleCollider2D;

    private void Awake()
    {
        circleCollider2D    = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Player")
        {
            GameObject boss = Instantiate(bossPrefab, transform.position, transform.rotation);
            circleCollider2D.enabled = false;
        }
    }
}
