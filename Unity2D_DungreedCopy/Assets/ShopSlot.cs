using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ShopSlot : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler, IPointerClickHandler
{
    [Header("아이템SO")]
    [SerializeField]
    private ItemSO              item;
    [SerializeField]
    private InventorySO         inventory;

    [Header("UI요소")]
    private Image               baseImage;
    [SerializeField]
    private Sprite[]            baseSprites;
    [SerializeField]
    private Image               itemImage;
    [SerializeField]
    private TextMeshProUGUI     textName;
    [SerializeField]
    private TextMeshProUGUI     textGold;

    private GameObject          discriptionUI;

    private void Awake()
    {
        baseImage   = GetComponent<Image>();

        discriptionUI = GameObject.Find("MainCanvas/InventoryUI/InventoryDescription");
    }
    private void Start()
    {
        itemImage.sprite = item.ItemImage;
        itemImage.SetNativeSize();
        textName.text = item.Name;
        textGold.text = item.Gold.ToString();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        discriptionUI.SetActive(true);
        baseImage.sprite = baseSprites[1];
        discriptionUI.GetComponent<UIInventoryDescription>().SetDescription(itemImage.sprite, textName.text, item.Description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        discriptionUI.SetActive(false);
        baseImage.sprite = baseSprites[0];
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 마우스 우클릭시
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            // 구매 가능
            if(PlayerStats.instance.GOLD >= item.Gold)
            {
                Debug.Log("구매!");
                inventory.AddItem(item, 1);
                AudioManager.Instance.PlaySFX("Buy");
                PlayerStats.instance.gold -= item.Gold;
                this.gameObject.SetActive(false);
            }
            // 골드 부족
            else
            {
                Debug.Log("구매 불가");
                string line = "골드가 부족합니다.";
                UIManager.instance.UpdateTextNoGold(true);
                UIManager.instance.ChangeTextNoGold(line);
            }
        }
    }
}
