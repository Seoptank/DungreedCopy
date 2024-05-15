using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ParallaxBackgrounds : MonoBehaviour
{
    [SerializeField]
    private float parallaxEffectMultiplierX; // x축 이동에 대한 배율

    private Transform camTransform;
    private Vector3 lastCamPos;
    private float textureUnitSizeX;
    private SpriteRenderer spriteRenderer;

    [SerializeField]

    private Vector3 initialPosition;

    private void Awake()
    {
        initialPosition = transform.localPosition;

        camTransform = Camera.main.transform;
        lastCamPos = camTransform.position;

        spriteRenderer = GetComponent<SpriteRenderer>();
        CalculateTextureUnitSize();
    }

    private void Update()
    {
        // x축 이동량만 고려하여 배경을 이동시킵니다.
        float deltaX = camTransform.position.x - lastCamPos.x;
        transform.position += new Vector3(deltaX * parallaxEffectMultiplierX, 0f, 0f);
        lastCamPos = camTransform.position;

        // 텍스처 단위 크기를 넘어가면 배경을 반복하여 배치합니다.
        if (Mathf.Abs(camTransform.position.x - transform.position.x) >= textureUnitSizeX)
        {
            float offsetPosX = (camTransform.position.x - transform.position.x) % textureUnitSizeX;
            transform.position = new Vector3(camTransform.position.x + offsetPosX, transform.position.y);
        }
    }

    private void CalculateTextureUnitSize()
    {
        Sprite sprite = spriteRenderer.sprite;
        Texture2D texture = sprite.texture;
        textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
    }

    private void OnEnable()
    {
        transform.localPosition = initialPosition;
    }
}