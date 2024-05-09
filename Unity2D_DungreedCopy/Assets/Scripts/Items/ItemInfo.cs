using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo : MonoBehaviour
{ 
    public ItemSO   item;

    [SerializeField]
    private InventorySO inventory;

    private void Update()
    {
        inventory.AddItem(item, 1);
    }
}
