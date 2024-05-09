using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemParameterSO : ScriptableObject
{
    [field: SerializeField]
    public string ParameterName { get; private set; }

    [field: SerializeField]
    public int Damage { get; set; }
    [field: SerializeField]
    public float AttackSpeed { get; set; }
}
