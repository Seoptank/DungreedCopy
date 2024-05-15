using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    private Animator    ani;
    private NPC         npc;

    [SerializeField]
    private Animator invenAni;

    private void Awake()
    {
        ani = GetComponent<Animator>();

        npc = FindObjectOfType<NPC>();
    }

    public void HideShop()
    {
        ani.Play("ShopHide");
        invenAni.Play("Hide");
        npc.inputKey = false;
        PlayerController.instance.dontMovePlayer = false;
        DialogueManager.instance.onShop = false;
    }
}
