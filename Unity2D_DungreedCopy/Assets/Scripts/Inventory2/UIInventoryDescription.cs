using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryDescription : MonoBehaviour
{
    [SerializeField]
    private Image itemImage;
    [SerializeField]
    private TMP_Text title;
    [SerializeField]
    private TMP_Text description;

    private RectTransform   rect;               // Discription�� Rect
    [SerializeField]
    private GameObject      imageRightMouse;
    [SerializeField]
    private TextMeshProUGUI textSellAndPerchase; 

    public void Awake()
    {
        rect = GetComponent<RectTransform>(); 
        ResetDescription();

        imageRightMouse.SetActive(false);
    }

    private void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2);

        transform.position = mousePosition;

        // 4��и����� ������
        int quadrantX = (mousePosition.x > screenCenter.x) ? 1 : -1;
        int quadrantY = (mousePosition.y > screenCenter.y) ? 1 : -1;

        // ���콺 ��ġ�� ���� transform.position ���
        if (quadrantX == 1 && quadrantY == 1)
        {
            rect.pivot = new Vector2(1, 1);
        }
        else if (quadrantX == -1 && quadrantY == 1)
        {
            rect.pivot = new Vector2(0, 1);
        }
        else if (quadrantX == -1 && quadrantY == -1)
        {
            rect.pivot = new Vector2(1, 1);
        }
        else if (quadrantX == 1 && quadrantY == -1)
        {
            rect.pivot = new Vector2(1, 0);
        }

        imageRightMouse.SetActive(DialogueManager.instance.onShop);

        if (quadrantX == 1)
            textSellAndPerchase.text = "�Ǹ�";
        else
            textSellAndPerchase.text = "����";
    }

    public void ResetDescription()
    {
        this.itemImage.gameObject.SetActive(false);
        this.title.text = "";
        this.description.text = "";
    }

    public void SetDescription(Sprite sprite, string itemName, string itemDescription)
    {
        this.itemImage.gameObject.SetActive(true);
        this.itemImage.sprite = sprite;
        this.itemImage.SetNativeSize();
        this.title.text = itemName;
        this.description.text = itemDescription;
    }
}
