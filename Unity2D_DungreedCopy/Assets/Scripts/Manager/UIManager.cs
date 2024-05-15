using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum UIType
{
    PlayerUI,
    MapUI,
    InvenUI,
}

public class UIManager : MonoBehaviour
{

    #region UI������Ʈ
    public GameObject[]     playerStateUI;
    #endregion

    static public UIManager instance;

    [Header("LV")]
    [SerializeField]
    private TextMeshProUGUI textLV;
    
    [Header("HP")]
    [SerializeField]
    private Image           imageHP;
    [SerializeField]
    private Image           imageBloodScreen;
    [SerializeField]
    private AnimationCurve  curveBloodScreen;
    [SerializeField]
    private TextMeshProUGUI textHP;
    
    [Header("DC")]
    [SerializeField]
    private Image[]         imageDC;

    [Header("GOLD")]
    [SerializeField]
    private TextMeshProUGUI textGOLD;
    [SerializeField]
    private GameObject      textNoGold;

    [Header("Acquired Item")]
    [SerializeField]
    private GameObject      AcquiredItemUI;         // �������� ������ Ȱ��/��Ȱ��ȭ
    [SerializeField]
    private Image           imageAcquiredItem;      // ���� ������ �̹���
    [SerializeField]
    private TextMeshProUGUI textAcquiredItemName;   // ���� ������ �̸�

    [SerializeField]
    private PlayerStats         playerStats;

    [SerializeField]
    private GameObject menuUI;          // �޴�â
    [SerializeField]
    private GameObject exitCeckUI;      // ������ Ȯ��â

    [SerializeField]
    private bool menuUIon = true;
    [SerializeField]
    private GameObject notificationTxt;     // UI��� �Ұ��� �˸�
    [SerializeField]
    private GameObject[] menuButtons;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);

            playerStats.onHPEvent.AddListener(UpdateImageBloodScreenAndTextHP);


            textNoGold.SetActive(false);
            AcquiredItemUI.SetActive(false);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void Update()
    {
        UpdateImageDC();
        UpdateImageHP();
        UpdateTextGold();
        UpdateTextLV();
        OnMenuUI();
        ChangeButton();

        textHP.text = (int)playerStats.HP + "/" + (int)playerStats.MaxHP;

        PlayerStats playerStat = PlayerStats.instance;
    }
    private void UpdateImageHP()
    {
        imageHP.fillAmount = Mathf.Lerp(imageHP.fillAmount, playerStats.HP/playerStats.MaxHP, Time.deltaTime * 5);
        
    }

    public void UpdateTextNoGold(bool onText)
    {
        textNoGold.SetActive(onText);

        if(onText)
        {
            StartCoroutine(RoutainTextnoGold());
        }
    }

    private IEnumerator RoutainTextnoGold()
    {
        yield return new WaitForSeconds(1.5f);
        textNoGold.SetActive(false); ;
    }

    public void ChangeTextNoGold( string newText)
    {
        textNoGold.GetComponent<TextMeshProUGUI>().text = newText.ToString();
    }

    private void UpdateTextGold()
    {
        textGOLD.text = playerStats.GOLD.ToString();
    }
    private void UpdateTextLV()
    {
        textLV.text = playerStats.LV.ToString();
    }

    private void UpdateImageDC()
    {
        for (int i = 0; i < imageDC.Length; ++i)
        {
            float fillAmount = i >= playerStats.DC ? 0f : 1f;
            imageDC[i].fillAmount = Mathf.Lerp(imageDC[i].fillAmount, fillAmount, Time.deltaTime * 5f); ;
        }
    }

    public IEnumerator OnBloodScreen()
    {
        float percent = 0;

        while(percent < 1)
        {
            percent += Time.deltaTime;

            Color color = imageBloodScreen.color;
            color.a = Mathf.Lerp(1, 0, curveBloodScreen.Evaluate(percent));
            imageBloodScreen.color = color;

            yield return null;
        }
    }

   public void UpdateImageBloodScreenAndTextHP(float pre, float cur)
    {
        textHP.text = (int)playerStats.HP + "/"  + (int)playerStats.MaxHP;

        if (pre <= cur) return;
        
        if (pre - cur > 0)
        {
            StopCoroutine(OnBloodScreen());
            StartCoroutine(OnBloodScreen());
        }
    }
    public void OnAcquiredItem(string itemName, Sprite itemImage)
    {
        // UI Ȱ��ȭ
        AcquiredItemUI.SetActive(true);

        // �̹��� ����
        imageAcquiredItem.sprite = itemImage;

        // �̹��� ������ ���� ����� �°�
        imageAcquiredItem.SetNativeSize();

        // �̸� ����
        textAcquiredItemName.text = itemName;

        // ��Ȱ��ȭ �ڷ�ƾ
        StartCoroutine(OffAcquiredItem());
    }

    private IEnumerator OffAcquiredItem()
    {
        yield return new WaitForSeconds(3f);
        // UI ��Ȱ��ȭ
        AcquiredItemUI.SetActive(false);
    }

    public void OnMenuUI()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && DialogueManager.instance.onShop == false)
        {
            if (menuUIon == true)
            {
                Time.timeScale = 0f;
                PlayerController.instance.dontMovePlayer = true;
            }
            else
            {
                Time.timeScale = 1f;
                PlayerController.instance.dontMovePlayer = false;
            }
            menuUI.SetActive(menuUIon);
            menuUIon = !menuUIon;
        }
    }

    public void OnMenuUIButton()
    {
        if (menuUIon == true)
        {
            Time.timeScale = 0f;
            PlayerController.instance.dontMovePlayer = true;
        }
        else
        {
            Time.timeScale = 1f;
            PlayerController.instance.dontMovePlayer = false;
        }
        menuUI.SetActive(menuUIon);
        menuUIon = !menuUIon;
    }

    public void OffExitCeck()
    {
        exitCeckUI.SetActive(false);
    }

    public IEnumerator OnNotificationTxt()
    {
        notificationTxt.SetActive(true);
        yield return new WaitForSeconds(2f);
        notificationTxt.SetActive(false);
    }

    public void ChangeButton()
    {
        if (MySceneManager.instance.curSceneName == "Scene(Yuseop)")
        {
            menuButtons[0].SetActive(true);
            menuButtons[1].SetActive(false);
        }
        else if (MySceneManager.instance.curSceneName == "Original")
        {
            menuButtons[0].SetActive(false);
            menuButtons[1].SetActive(true);
        }
    }

    public void ChangeScene()
    {
        PlayerController.instance.dontMovePlayer = true;
        StartCoroutine(PlayerController.instance.movement.Die());
    }
}
