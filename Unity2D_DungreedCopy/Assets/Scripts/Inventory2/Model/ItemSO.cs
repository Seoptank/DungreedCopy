using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemSO : ScriptableObject
{
    [field: SerializeField]
    public bool IsStackable { get; set; }
    [field: SerializeField]
    public bool Melee { get; set; }
    [field: SerializeField]
    public bool Range { get; set; }

    public int ID => GetInstanceID();

    [field: SerializeField]
    public int MaxStackSize { get; set; } = 1;
    [field: SerializeField]
    public int Code { get; set; }
    [field: SerializeField]
    public string Name { get; set; }
    [field: SerializeField]
    [field: TextArea]
    public string Description { get; set; }
    [field: SerializeField]
    public Sprite ItemImage { get; set; }
    [field: SerializeField]
    public int MinDamage { get; set; }
    [field: SerializeField]
    public int MaxDamage { get; set; }
    [field: SerializeField]
    public float AttckSpeed { get; set; }
    [field: SerializeField]
    public int Gold { get; set; }

    [field: SerializeField]
    public List<ItemParameter> DefaultParametersList { get; set; }
}

[Serializable]
public struct ItemParameter : IEquatable<ItemParameter>
{
    public ItemParameterSO itemParameter;
    public float value;

    public bool Equals(ItemParameter other)
    {
        return other.itemParameter == itemParameter;
    }
}

