using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteleController : MonoBehaviour
{
    private UIEffectManager     uiEffectManager;
    private PlayerController    playerController;
    
    private Animator            ani;
    private BoxCollider2D       boxCollider;
    private SpriteRenderer      spriteRenderer;

    private void Start()
    {
        boxCollider     = GetComponent<BoxCollider2D>();
        spriteRenderer  = GetComponent<SpriteRenderer>();
        ani             = GetComponent<Animator>();

        playerController    = FindObjectOfType<PlayerController>();
        uiEffectManager     = FindObjectOfType<UIEffectManager>();
    }

    private void Update()
    {
        if (playerController.playerMeetsBoss)
        {
            StartCoroutine(uiEffectManager.UIFade(spriteRenderer, 0, 1, 0.1f));
            ani.SetTrigger("CloseTheDoor");
        }

        if (playerController.bossOpentheStele)
        {
            ani.SetTrigger("OpenTheDoor");
            boxCollider.enabled = false;
        }

        if (spriteRenderer.color.a < 0.8f)
            boxCollider.enabled = false;
        else
            boxCollider.enabled = true;

    }

    public void FalseToBossOpenTheStele()
    {
        playerController.bossOpentheStele = false;
        gameObject.SetActive(false);
    }
}