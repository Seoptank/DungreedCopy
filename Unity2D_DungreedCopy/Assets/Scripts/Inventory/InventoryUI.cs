using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    Inventory inven;
    [SerializeField]
    private GameObject inventoryPanel;

    public Slot[] slots;
    public int slotCount;
    public Transform slotHolder;
    public GameObject ItemImage;
    [SerializeField]
    private bool        activeInventory = false;

    private void Start()
    {
        slots = slotHolder.GetComponentsInChildren<Slot>();
        inven = Inventory.instance;
        inven.onChangeItem += RedrawSlotUI;

        // 최초 실행시에는 인벤토리 비활성화로 초기화
        inventoryPanel.SetActive(activeInventory);

    }

    private void Update()
    {
        // v키 입력시 activeInventory(bool) 활성화 반전
        if (Input.GetKeyDown(KeyCode.V))
        {
            activeInventory = !activeInventory;
            inventoryPanel.SetActive(activeInventory);
        }
    }

    void RedrawSlotUI()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            slots[i].RemoveSlot();
        }
        for (int i = 0; i < inven.items.Count; i++)
        {
            slots[i].item = inven.items[i];
            slots[i].UpdateSlotUI();
            slotCount = i;
        }
    
    }
}
