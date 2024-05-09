using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorDungeon : MonoBehaviour
{
    [SerializeField]
    private GameObject[]    doors;                                     // 던전에 있는 문
    [SerializeField]
    private bool            exsistTel;      // Tel이 현재 dungeon에 존재하는지 여부                     
    [SerializeField]
    private GameObject      curTel;         // 현재  dungeon의 Tel
    public int              enemiesCount;   // 남은 적의 수

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
        // UI못켜게
        PlayerDungeonData.instance.isFighting = true;

        // 활성화 되자마자 문 잠그기
        CloseTheDoor();
    
        // dungeon에 Teleport가 존재하면
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

            // 문 닫는 이미지
            doors[i].GetComponent<Animator>().SetTrigger("CloseTheDoor");

            // 콜라이더 활성화
            doors[i].GetComponent<BoxCollider2D>().enabled = true;
        }
        PlayerDungeonData.instance.isFighting = true;
    }

    private IEnumerator OpenTheDoor()
    {
        while (true)
        {
            yield return new WaitUntil(() => enemiesCount == 0); // enemiesCount가 0이 될 때까지 대기

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
                // 문 여는 이미지
                doors[i].GetComponent<Animator>().SetTrigger("OpenTheDoor");

                // 콜라이더 비활성화
                doors[i].GetComponent<BoxCollider2D>().enabled = false;
                PlayerDungeonData.instance.isFighting = false;
            }

            yield break;
        }
    }
}
