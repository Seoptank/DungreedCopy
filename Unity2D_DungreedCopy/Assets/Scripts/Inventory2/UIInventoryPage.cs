using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.Experimental.AI;

public class UIInventoryPage : MonoBehaviour
{
    [SerializeField]
    private UIInventoryItem         itemPrefab;
    [SerializeField]
    private RectTransform           contentPanel;
    [SerializeField]
    private UIInventoryDescription itemDescription;
    [SerializeField]
    private MouseFollower           mouseFollower;
    [SerializeField]
    List<UIInventoryItem>           listOfUIItems = new List<UIInventoryItem>();    //인벤토리 아이템 리스트
    [SerializeField]
    private TextMeshProUGUI         textGold;

    private int currentlyDraggedItemIndex = -1;

    public event Action<int> OnDescriptionRequested, OnItemActionRequested, OnStartDragging;

    public event Action<int, int> OnSwapItems;

    private Animator            ani;

    private void Awake()
    {
        ani     = GetComponent<Animator>();
        mouseFollower.Toggle(false);
        itemDescription.ResetDescription();

        gameObject.SetActive(false);
    }

    private void Update()
    {
        textGold.text = PlayerStats.instance.GOLD.ToString();
    }

    // inventorysize의 설정값만큼 인벤토리 칸 생성
    public void InitializeInventoryUI(int inventorysize)
    {
        for (int i = 0; i < inventorysize; i++)
        {
            UIInventoryItem uiItem = listOfUIItems[i];

            // 드래그앤드롭 이벤트
            uiItem.OnItemClicked += HandleItemSelection;
            uiItem.OnItemPointed += HandleItemSelection;
            uiItem.OnItemBeginDrag += HandleBeginDrag;
            uiItem.OnItemDroppedOn += HandleSwap;
            uiItem.OnItemEndDrag += HandleEndDrag;
            uiItem.OnRightMouseBtnClick += HandleShowItemActions;
        }
    }

    internal void ResetAllItems()
    {
        foreach (var item in listOfUIItems)
        {
            item.ResetData();
            item.Deselect();
        }
    }

    internal void UpdateDescription(int itemIndex, Sprite itemImage, string name, string description)
    {
        itemDescription.SetDescription(itemImage, name, description);
        DeselectAllItems();
        listOfUIItems[itemIndex].Select();
    }

    public void UpdateData(int itemIndex, Sprite itemImage, int itemQuantity)
    {
        if(listOfUIItems.Count > itemIndex)
        {
            listOfUIItems[itemIndex].SetData(itemImage, itemQuantity);
        }
    }

    // inventoryItemUI의 값은 UIInventoryItem class가 생성된 순서에 맞춰 0부터 시작
    private void HandleShowItemActions(UIInventoryItem inventoryItemUI)
    {
        int index = listOfUIItems.IndexOf(inventoryItemUI);
        if (index == -1)
        {
            return;
        }
        OnItemActionRequested?.Invoke(index);
    }

    private void HandleEndDrag(UIInventoryItem inventoryItemUI)
    {
        ResetDraggedItem();
    }

    private void HandleSwap(UIInventoryItem inventoryItemUI)
    {
        int index = listOfUIItems.IndexOf(inventoryItemUI);
        if (index == -1)
        {
            return;
        }
        OnSwapItems?.Invoke(currentlyDraggedItemIndex, index);
        HandleItemSelection(inventoryItemUI);
    }

    private void ResetDraggedItem()
    {
        mouseFollower.Toggle(false);
        currentlyDraggedItemIndex = -1;
    }

    private void HandleBeginDrag(UIInventoryItem inventoryItemUI)
    {
        int index = listOfUIItems.IndexOf(inventoryItemUI);
        if (index == -1)
            return;
        currentlyDraggedItemIndex = index;
        HandleItemSelection(inventoryItemUI);
        OnStartDragging?.Invoke(index);
    }

    public void CreateDraggedItem(Sprite sprite, int quantity)
    {
        mouseFollower.Toggle(true);
        mouseFollower.SetData(sprite, quantity);
    }

    private void HandleItemSelection(UIInventoryItem inventoryItemUI)
    {
        int index = listOfUIItems.IndexOf(inventoryItemUI);
        if (index == -1)
            return;
        OnDescriptionRequested?.Invoke(index);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        ani.Play("Show");
        PlayerDungeonData.instance.isMoving = true;
        PlayerController.instance.dontMovePlayer   = true;
        ResetSelection();
    }

    public void ResetSelection()
    {
        itemDescription.ResetDescription();
        DeselectAllItems();
    }

    private void DeselectAllItems()
    {
        foreach(UIInventoryItem item in listOfUIItems)
        {
            item.Deselect();
        }
    }

    public void Hide()
    {
        ani.Play("Hide");

        PlayerDungeonData.instance.isMoving = false;
        PlayerController.instance.dontMovePlayer = false;
        ResetDraggedItem();
        StartCoroutine("RealDeactivate");
    }

    private IEnumerator RealDeactivate()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}
