using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenPieces : MonoBehaviour
{
    private Rigidbody2D rigidbody2D;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();

    }
    private void OnEnable()
    {
        float randomY = Random.Range(0, 1f);
        float flyForce = Random.Range(1, 4);

        Vector3 dir = new Vector3(PlayerController.instance.lastMoveDirX, randomY, 0);

        rigidbody2D.AddForce(dir * flyForce, ForceMode2D.Impulse);
    }
}
