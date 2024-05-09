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
            // Map UI ��Ȱ��ȭ
            mapController.MapOn = false;

            // Teleport �÷��̾� �Դ� �ִ� ���
            mapController.startTeleport.GetComponent<Animator>().SetBool("EatPlayer",true);

            // �÷��̾� ����
            PlayerController.instance.dontMovePlayer = true;

            // MapController�� �ش� ���� Telepoort���� ����
            mapController.targetTeleport = targetTeleport.gameObject;

        }


    }
}
