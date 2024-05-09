using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField]
    private Vector3 originPos;
    [SerializeField]
    private Vector3 originRot;
    [SerializeField]
    [Range(-1.0f, 1.0f)]
    private float moveSpeed = 0.1f;
    private Material material;

    private void Awake()
    {
        material = GetComponent<Renderer>().material;
    }

    private void OnEnable()
    {
        transform.position = originPos;
        transform.rotation = Quaternion.Euler(originRot);
    }

    // Update is called once per frame
    void Update()
    {
        material.SetTextureOffset("_MainTex", Vector2.right * moveSpeed * Time.time);
    }
}
