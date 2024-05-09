using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateFairy : MonoBehaviour
{
    [SerializeField]
    private float       size;
    [SerializeField]
    private float       speed;

    private Vector2     originScale;
    private float       time;

    private CircleCollider2D circleCollider2D;

    private void Awake()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
        
        originScale = transform.localScale;
    }

    private void OnEnable()
    {
        circleCollider2D.enabled = false;
        StartCoroutine("ScaleUp");
    }

    private IEnumerator ScaleUp()
    {
        while(transform.localScale.x <size)
        {

            transform.localScale = originScale * (1 * time * speed);
            time += Time.deltaTime;

            if(transform.localScale.x >= size)
            {
                time = 0;
                circleCollider2D.enabled = true;
                break;
            }
            yield return null;
        }
    }
}

