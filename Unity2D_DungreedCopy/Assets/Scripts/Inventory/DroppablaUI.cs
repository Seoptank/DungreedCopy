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

    // 마우스 포인터가 현재 아이템 슬롯 영역 내부로 들어가 때 1회 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 아이템 슬롯 이미지 변경
        image.sprite = slotImage[1];
    }

    // 마우스 포인터가 현재 아이템 슬롯 영역을 빠져나갈 때 1회 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        // 아이템 슬롯 원래 이미지로 변경
        image.sprite = slotImage[0];
    }

    // 현재 아이템 슬롯 영역 내부에서 드롭을 했을 때 1회 호출
    public void OnDrop(PointerEventData eventData)
    {
        // pointerDrag는 현재 드래그하고 있는 대상(=아이템)
        if (eventData.pointerDrag != null)
        {
            // 드래그하고 있는 대상의 부모를 현재 오브젝트로 설정하고, 위치를 현재 오브젝트 위치와 동일하게 설정
            eventData.pointerDrag.transform.SetParent(transform);
            eventData.pointerDrag.GetComponent<RectTransform>().position = rect.position;
            dragimage.sprite = eventData.pointerDrag.GetComponent<Image>().sprite;
        }
    }
}
