using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorDungeon : MonoBehaviour
{
    [SerializeField]
    private GameObject[]    doors;                                     // ������ �ִ� ��
    [SerializeField]
    private bool            exsistTel;      // Tel�� ���� dungeon�� �����ϴ��� ����                     
    [SerializeField]
    private GameObject      curTel;         // ����  dungeon�� Tel
    public int              enemiesCount;   // ���� ���� ��

    [SerializeField]
    private Transform fairyPos;
    [SerializeField]
    private Transform treasureBoxPos;
    [SerializeField]
    private GameObject fairyPrefab;
    [SerializeField]
    private GameObject treasureBoxPrefab;
    private PoolManager treasurePool;

    private void Awake()
    {
        StartCoroutine(OpenTheDoor());

        if(treasureBoxPrefab!=null)
            treasurePool = new PoolManager(treasureBoxPrefab);
    }
    private void ActivateTreasureBox()
    {
        if(treasureBoxPrefab!=null)
        {
            GameObject treasureBox = treasurePool.ActivePoolItem();
            treasureBox.transform.position = transform.position;
            treasureBox.transform.rotation = Quaternion.identity;
            treasureBox.GetComponent<CreateTresureBox>().Setup(treasurePool);
        }
    }
    private void ActivateFairy()
    {
        if(fairyPrefab != null)
        {
            GameObject fairy = Instantiate(fairyPrefab, fairyPos.position, fairyPos.rotation, fairyPos.parent);
        }
    }

    private void OnEnable()
    {
        // UI���Ѱ�
        PlayerDungeonData.instance.isFighting = true;

        // Ȱ��ȭ ���ڸ��� �� ��ױ�
        CloseTheDoor();
    
        // dungeon�� Teleport�� �����ϸ�
        if (exsistTel)
        {
            curTel = this.gameObject.GetComponent<TeleportDungeon>().teleport;
            curTel.SetActive(false);
        }
        else
        {   
            if(curTel != null) curTel.SetActive(true);
        }
    }
    private void CloseTheDoor()
    {
        for (int i = 0; i < doors.Length; ++i)
        {
            doors[i].SetActive(true);

            // �� �ݴ� �̹���
            doors[i].GetComponent<Animator>().SetTrigger("CloseTheDoor");

            // �ݶ��̴� Ȱ��ȭ
            doors[i].GetComponent<BoxCollider2D>().enabled = true;
        }
        PlayerDungeonData.instance.isFighting = true;
    }

    private IEnumerator OpenTheDoor()
    {
        while (true)
        {
            yield return new WaitUntil(() => enemiesCount == 0); // enemiesCount�� 0�� �� ������ ���

            int randomNumber = UnityEngine.Random.Range(0, 100);
            if (randomNumber <= 24 && treasureBoxPos != null)
            {
                ActivateTreasureBox();
            }
            if (randomNumber <= 49 && fairyPos != null)
            {
                ActivateFairy();
            }

            if(curTel!= null)
            {
                curTel.SetActive(true);
            }

            for (int i = 0; i < doors.Length; ++i)
            {
                // �� ���� �̹���
                doors[i].GetComponent<Animator>().SetTrigger("OpenTheDoor");

                // �ݶ��̴� ��Ȱ��ȭ
                doors[i].GetComponent<BoxCollider2D>().enabled = false;
                PlayerDungeonData.instance.isFighting = false;
            }

            yield break;
        }
    }
}
