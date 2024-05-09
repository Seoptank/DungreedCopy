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
        // 숨기는 애니메이션 재생
        ani.Play("Hide");
        // 버튼 사라지는 애니메이션 재생
        for (int i = 0; i < buttonsAnimators.Length; ++i)
        {
            buttonsAnimators[i].Play("HideBottons");
        }
        npc.inputKey = false;
        openDialogue = false;

        if (curNPCName == "크록")
        {
            Debug.Log("크록UI");
            shopAnimator.gameObject.SetActive(true);
            invenAnimator.gameObject.SetActive(true);
            shopAnimator.Play("ShopShow");
            invenAnimator.Play("Show");
            onShop = true;
        }
        else if (curNPCName == "카블로비나")
        {
            Debug.Log("카블로비나UI");
            abillityAnimator.gameObject.SetActive(true);
            abillityAnimator.Play("AbilityShow");
        }
        else if(curNPCName == "방랑자 카블로비나")
        {
            // 점점 사라지게
            StartCoroutine(UIEffectManager.instance.UIFade(npcSpriteRenderer,1,0));
            PlayerController.instance.dontMovePlayer = false;
            // 마을에 나타나게 활성화
            NPCManager.instance.meetKablovinaInDungeon = true;
        }
        else if (curNPCName == "호레리카")
        {
            PlayerController.instance.dontMovePlayer = false;

            if (!npc.visited)
            {
                PlayerStats info = PlayerStats.instance;

                // 골드가 300 이상이고 현재 HP가 maxHP의 반이하일때
                if (info.HP <= info.MaxHP * 0.5f && info.GOLD >= 300)
                {
                    // 이팩트 생성
                    GameObject effect = effectPool.ActivePoolItem();
                    effect.transform.position = PlayerController.instance.transform.position;
                    effect.transform.rotation = Quaternion.identity;
                    effect.GetComponent<EffectPool>().Setup(effectPool);

                    // 골드 소모
                    info.GOLD -= 300;

                    // 체력 회복
                    info.HP += 50;

                    AudioManager.Instance.PlaySFX("Heal");

                    // 방문 여부 활성화
                    npc.visited = true;
                }
                else if (info.HP > info.MaxHP * 0.5f)
                {
                    string line = "HP가 충분합니다.";
                    UIManager.instance.UpdateTextNoGold(true);
                    UIManager.instance.ChangeTextNoGold(line);
                }
                else if (info.GOLD < 300)
                {
                    string line = "골드가 부족합니다.";
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
