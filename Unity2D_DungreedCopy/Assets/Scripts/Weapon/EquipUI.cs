using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipUI : MonoBehaviour
{
    [SerializeField]
    private InventorySO inventoryData;

    [SerializeField]
    private GameObject equipImage;

    private Image uiImage;

    public int itemCode;

    private void Awake()
    {
        uiImage = equipImage.GetComponent<Image>();
    }
    void Update()
    {
        if (inventoryData.inventoryItems[itemCode].item == null)
        {
            equipImage.SetActive(false);
        }
        else if (equipImage != null) 
        {
            equipImage.SetActive(true);
            uiImage.sprite = inventoryData.inventoryItems[itemCode].item.ItemImage;
            uiImage.SetNativeSize();
        }
    }
}
