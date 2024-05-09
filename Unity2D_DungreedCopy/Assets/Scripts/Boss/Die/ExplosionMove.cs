using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionMove : MonoBehaviour
{
    [SerializeField]
    private float   moveSpeed;

    private void Update()
    {
        transform.Translate(Vector2.right * (moveSpeed * Time.deltaTime), Space.Self);
    }

    public void DestroyEffect()
    {
        Destroy(gameObject);
    }

}
