using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonName : MonoBehaviour
{
    public string dungeonName;
    public int    dungeonNum;
    public bool   haveTeleport;

    private void Awake()
    {
        dungeonName = this.gameObject.name;
    }
}
