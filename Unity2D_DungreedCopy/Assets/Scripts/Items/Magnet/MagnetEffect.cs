using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetEffect : MonoBehaviour
{
    [Header("자석효과 변수")]
    [SerializeField]
    private float       magnetDis;              
    [SerializeField]
    private float       magnetStrngth;          
    [SerializeField]
    private int         magnetDirection = 1;    

    private Transform   targetTransform;
    private void Awake()
    {
        targetTransform = GameObject.FindGameObjectWithTag("Player").transform;    
    }
    void Update()
    {
        if (Vector2.Distance(transform.position, targetTransform.position) <= magnetDis)
        {
            CheckDisToPlayer();
        }
    }
    private void CheckDisToPlayer()
    {
        Vector2 dirToTarget = targetTransform.position - transform.position;
        float dis = Vector2.Distance(targetTransform.position, transform.position);
        float magnetDisStr = (magnetDis / dis) * magnetStrngth;
        transform.Translate(magnetDisStr * (dirToTarget * magnetDirection) * Time.deltaTime);
    }
}
