using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ParallaxBackgrounds : MonoBehaviour
{
    [SerializeField]
    private float parallaxEffectMultiplierX; // x�� �̵��� ���� ����

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
        // x�� �̵����� ����Ͽ� ����� �̵���ŵ�ϴ�.
        float deltaX = camTransform.position.x - lastCamPos.x;
        transform.position += new Vector3(deltaX * parallaxEffectMultiplierX, 0f, 0f);
        lastCamPos = camTransform.position;

        // �ؽ�ó ���� ũ�⸦ �Ѿ�� ����� �ݺ��Ͽ� ��ġ�մϴ�.
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