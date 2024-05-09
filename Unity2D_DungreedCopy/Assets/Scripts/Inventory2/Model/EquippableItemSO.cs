using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EquippableItemSO : ItemSO, IDestroyableItem, IItemAction
{
    public string ActionName => "Equip";

    public AudioClip actionSFX { get; private set; }

    public bool PerformAction(GameObject character, List<ItemParameter> itemState = null)
    {
        AgentWeapon weaponSystem = character.GetComponent<AgentWeapon>();
        if(weaponSystem != null)
        {
            weaponSystem.SetWeapon(this, itemState == null ? DefaultParametersList : itemState);
            return true;
        }
        return false;
    }
}


