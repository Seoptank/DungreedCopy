using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectedTeleport : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
{
    public Sprite[] sprites;        

    private Image thisImage;

    [SerializeField]
    private TeleportDungeon   targetTeleport;
    private MapController     mapController;

    private void Awake()
    {
        thisImage = GetComponent<Image>();
        mapController = FindObjectOfType<MapController>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        thisImage.sprite = sprites[1];
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        thisImage.sprite = sprites[0];
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            // Map UI 비활성화
            mapController.MapOn = false;

            // Teleport 플레이어 먹는 애니 재생
            mapController.startTeleport.GetComponent<Animator>().SetBool("EatPlayer",true);

            // 플레이어 멈춤
            PlayerController.instance.dontMovePlayer = true;

            // MapController에 해당 맵의 Telepoort정보 전달
            mapController.targetTeleport = targetTeleport.gameObject;

        }


    }
}
