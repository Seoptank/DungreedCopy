using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EdibleItemSO : ItemSO, IDestroyableItem, IItemAction
{
    [SerializeField]
    private List<ModifierData> modifierData = new List<ModifierData>();
    public string ActionName => "Consume";
    public AudioClip actionSFX { get; private set; }
    public bool PerformAction(GameObject charater, List<ItemParameter> itemState = null)
    {
        foreach (ModifierData data in modifierData)
        {
            data.statModifier.AffectCharacter(charater, data.value);
        }
        return true;
    }
}

public interface IDestroyableItem
{

}

public interface IItemAction
{
    public string ActionName { get; }
    public AudioClip actionSFX { get; }
    bool PerformAction(GameObject character, List<ItemParameter> itemState);
}

[Serializable]
public class ModifierData
{
    public CharacterStatModifierSO statModifier;
    public float value;
}