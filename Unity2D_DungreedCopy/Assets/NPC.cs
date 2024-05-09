using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [Header("NPC의 DATA")]
    public string       name;
    public string[]     sentences;
    public string[]     visitedSentences;   // 방문여부에 따른 문장들

    [SerializeField]
    private GameObject  keyObj;             // F키 오브젝트
    private KeyCode     fKey = KeyCode.F;
    private bool        onKey;              // 키가 화면상에 보이는지 확인하는 변수 
    public bool         inputKey;           // 키를 눌렀는지 확인하기 위한 변수
    public bool         visited;            // 방문여부

    private void Update()
    {
        if (Input.GetKeyDown(fKey) && onKey && !inputKey)
        {
            DialogueManager dialogue = DialogueManager.instance;
            string[] lines = new string[] { };
            inputKey = true;
            onKey = false;

            if (!visited) lines = sentences;
            else          lines = visitedSentences;

            PlayerController.instance.dontMovePlayer = true;

            if (dialogue != null)
            {
                dialogue.OnDialogue(lines, name, this);
            }
            else
            {
                dialogue = GameObject.Find("MainCanvas").transform.GetChild(5).GetComponent<DialogueManager>();

                dialogue.OnDialogue(lines, name, this);
            }
        }

        keyObj.SetActive(onKey);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player" && !inputKey)
        {
            onKey = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player" && !inputKey)
        {
            onKey = false;
            inputKey = false;
        }
    }


}
