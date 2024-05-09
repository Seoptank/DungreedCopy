using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeTexture : MonoBehaviour
{
    private RawImage raw;
    void Awake()
    {
        raw = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        raw.texture = MiniMapManager.instance.minimapTexture[PlayerController.instance.curDungeonNum];
    }
}
