using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DialogueManager : MonoBehaviour, IPointerDownHandler
{
    public static DialogueManager instance;

    [SerializeField]
    private TextMeshProUGUI textName;
    [SerializeField]
    private TextMeshProUGUI textDialogue;
    public TextMeshProUGUI endingDialogue;
    [SerializeField]
    private GameObject nextText;
    public Queue<string> sentences;

    private string curSentence;
    private string curNPCName;

    [SerializeField]
    private float typingEffectWaitTime;
    [SerializeField]
    private bool isTyping;
    public bool openDialogue;
    private Animator ani;
    [SerializeField]
    private Animator[] buttonsAnimators;

    [Header("Ability UI")]
    [SerializeField]
    private Animator abillityAnimator;

    [Header("Shop UI")]
    [SerializeField]
    private Animator shopAnimator;
    [SerializeField]
    private Animator invenAnimator;
    public bool onShop;
    
    [SerializeField]
    private GameObject  effect;
    private PoolManager effectPool;

    private NPC npc;
    private SpriteRenderer npcSpriteRenderer;

    private void Awake()
    {
        instance = this;
        effectPool = new PoolManager(effect);
    }

    private void OnApplicationQuit()
    {
        effectPool.DestroyObjcts();
    }

    private void OnEnable()
    {
        sentences = new Queue<string>();
        ani = GetComponent<Animator>();
    }


    public void OnDialogue(string[] lines, string name,NPC newNPC)
    {
        openDialogue = true;
        sentences.Clear();
        textName.text = name;
        curNPCName = name;
        npc                 = newNPC;
        npcSpriteRenderer   = newNPC.gameObject.GetComponent<SpriteRenderer>(); 

        foreach (string line in lines)
        {
            sentences.Enqueue(line);
        }
        ani.Play("Show");

        NextSentence();
    }

    public void OnEnding(string[] lines)
    {
        openDialogue = true;
        sentences.Clear();

        foreach (string line in lines)
        {
            sentences.Enqueue(line);
        }

        NextSentenceEnding();
    }

    public void NextSentence()
    {
        if (sentences.Count != 0)
        {
            curSentence = sentences.Dequeue();

            isTyping = true;
            nextText.SetActive(false);
            StartCoroutine(Typing(curSentence));
        }
        else
        {
            for (int i = 0; i < buttonsAnimators.Length; ++i)
            {
                buttonsAnimators[i].Play("ShowBottons");
            }
        }
    }

    public void NextSentenceEnding()
    {
        if (sentences.Count != 0)
        {
            curSentence = sentences.Dequeue();

            isTyping = true;
            nextText.SetActive(false);
            StartCoroutine(EndingTyping(curSentence));
        }
    }

    private IEnumerator Typing(string line)
    {
        textDialogue.text = "";
        foreach (char letter in line.ToCharArray())
        {
            textDialogue.text += letter;
            AudioManager.Instance.PlaySFX("Text");
            yield return new WaitForSeconds(typingEffectWaitTime);
        }
    }

    private IEnumerator EndingTyping(string line)
    {
        endingDialogue.text = "";
        foreach (char letter in line.ToCharArray())
        {
            endingDialogue.text += letter;
            AudioManager.Instance.PlaySFX("Text");
            yield return new WaitForSeconds(typingEffectWaitTime);
        }
    }

    private void Update()
    {
        if (textDialogue.text.Equals(curSentence))
        {
            isTyping = false;
            nextText.SetActive(true);
        }

        if (endingDialogue.text.Equals(curSentence))
        {
            isTyping = false;
            nextText.SetActive(true);
        }

        if (openDialogue && !isTyping)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                NextSentence();
            }
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isTyping)
        {
            NextSentence();
        }
    }

    public void OnEnterButton()
    {
        // ����� �ִϸ��̼� ���
        ani.Play("Hide");
        // ��ư ������� �ִϸ��̼� ���
        for (int i = 0; i < buttonsAnimators.Length; ++i)
        {
            buttonsAnimators[i].Play("HideBottons");
        }
        npc.inputKey = false;
        openDialogue = false;

        if (curNPCName == "ũ��")
        {
            Debug.Log("ũ��UI");
            shopAnimator.gameObject.SetActive(true);
            invenAnimator.gameObject.SetActive(true);
            shopAnimator.Play("ShopShow");
            invenAnimator.Play("Show");
            onShop = true;
        }
        else if (curNPCName == "ī��κ�")
        {
            Debug.Log("ī��κ�UI");
            abillityAnimator.gameObject.SetActive(true);
            abillityAnimator.Play("AbilityShow");
        }
        else if(curNPCName == "����� ī��κ�")
        {
            // ���� �������
            StartCoroutine(UIEffectManager.instance.UIFade(npcSpriteRenderer,1,0));
            PlayerController.instance.dontMovePlayer = false;
            // ������ ��Ÿ���� Ȱ��ȭ
            NPCManager.instance.meetKablovinaInDungeon = true;
        }
        else if (curNPCName == "ȣ����ī")
        {
            PlayerController.instance.dontMovePlayer = false;

            if (!npc.visited)
            {
                PlayerStats info = PlayerStats.instance;

                // ��尡 300 �̻��̰� ���� HP�� maxHP�� �������϶�
                if (info.HP <= info.MaxHP * 0.5f && info.GOLD >= 300)
                {
                    // ����Ʈ ����
                    GameObject effect = effectPool.ActivePoolItem();
                    effect.transform.position = PlayerController.instance.transform.position;
                    effect.transform.rotation = Quaternion.identity;
                    effect.GetComponent<EffectPool>().Setup(effectPool);

                    // ��� �Ҹ�
                    info.GOLD -= 300;

                    // ü�� ȸ��
                    info.HP += 50;

                    AudioManager.Instance.PlaySFX("Heal");

                    // �湮 ���� Ȱ��ȭ
                    npc.visited = true;
                }
                else if (info.HP > info.MaxHP * 0.5f)
                {
                    string line = "HP�� ����մϴ�.";
                    UIManager.instance.UpdateTextNoGold(true);
                    UIManager.instance.ChangeTextNoGold(line);
                }
                else if (info.GOLD < 300)
                {
                    string line = "��尡 �����մϴ�.";
                    UIManager.instance.UpdateTextNoGold(true);
                    UIManager.instance.ChangeTextNoGold(line);
                }
            }
            else return;
        }
    }
    public void OnExitButton()
    {
        ani.Play("Hide");
        for (int i = 0; i < buttonsAnimators.Length; ++i)
        {
            buttonsAnimators[i].Play("HideBottons");
        }
        openDialogue = false;
        npc.inputKey = false;
        PlayerController.instance.dontMovePlayer = false;
    }
}
