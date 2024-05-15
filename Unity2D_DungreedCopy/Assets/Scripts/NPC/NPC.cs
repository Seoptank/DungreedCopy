using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [Header("NPC�� DATA")]
    public string       name;
    public string[]     sentences;
    public string[]     visitedSentences;   // �湮���ο� ���� �����

    [SerializeField]
    private GameObject  keyObj;             // FŰ ������Ʈ
    private KeyCode     fKey = KeyCode.F;
    private bool        onKey;              // Ű�� ȭ��� ���̴��� Ȯ���ϴ� ���� 
    public bool         inputKey;           // Ű�� �������� Ȯ���ϱ� ���� ����
    public bool         visited;            // �湮����

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
