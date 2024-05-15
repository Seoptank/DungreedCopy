using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomItemCreator : MonoBehaviour
{
    private float           speedY = 5.0f;
    [SerializeField]
    private ItemSO[]            items;
    [SerializeField]
    private InventorySO         inventory;
    [SerializeField]
    private int                 randomNum;             

    private PoolManager         poolManager;
    private Rigidbody2D         rigidBody;
    private SpriteRenderer      spriteRenderer;

    public void Setup(PoolManager poolManager)
    {
        this.poolManager = poolManager;

        rigidBody       = GetComponent<Rigidbody2D>();
        spriteRenderer  = GetComponent<SpriteRenderer>();
        rigidBody.velocity = new Vector3(0, speedY);

        int random = Random.Range(0,items.Length);
        randomNum = random;

        spriteRenderer.sprite = items[random].ItemImage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            rigidBody.velocity = Vector3.zero;
            rigidBody.gravityScale = 0;
        }

        if(collision.gameObject.name == "Player")
        {
            AudioManager.Instance.PlaySFX("TakeItem");
            inventory.AddItem(items[randomNum], 1);
            UIManager.instance.OnAcquiredItem(items[randomNum].Name, spriteRenderer.sprite);
            DeactivateEffect();
        }
    }
    public void DeactivateEffect()
    {
        poolManager.DeactivePoolItem(gameObject);
    }
}
