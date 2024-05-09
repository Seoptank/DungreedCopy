using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DroppablaUI : MonoBehaviour, IPointerEnterHandler, IDropHandler, IPointerExitHandler
{
    private Image image;
    private RectTransform rect;
    public Sprite[] slotImage;

    public SpriteRenderer dragimage;
    private void Awake()
    {
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }

    // ���콺 �����Ͱ� ���� ������ ���� ���� ���η� �� �� 1ȸ ȣ��
    public void OnPointerEnter(PointerEventData eventData)
    {
        // ������ ���� �̹��� ����
        image.sprite = slotImage[1];
    }

    // ���콺 �����Ͱ� ���� ������ ���� ������ �������� �� 1ȸ ȣ��
    public void OnPointerExit(PointerEventData eventData)
    {
        // ������ ���� ���� �̹����� ����
        image.sprite = slotImage[0];
    }

    // ���� ������ ���� ���� ���ο��� ����� ���� �� 1ȸ ȣ��
    public void OnDrop(PointerEventData eventData)
    {
        // pointerDrag�� ���� �巡���ϰ� �ִ� ���(=������)
        if (eventData.pointerDrag != null)
        {
            // �巡���ϰ� �ִ� ����� �θ� ���� ������Ʈ�� �����ϰ�, ��ġ�� ���� ������Ʈ ��ġ�� �����ϰ� ����
            eventData.pointerDrag.transform.SetParent(transform);
            eventData.pointerDrag.GetComponent<RectTransform>().position = rect.position;
            dragimage.sprite = eventData.pointerDrag.GetComponent<Image>().sprite;
        }
    }
}
