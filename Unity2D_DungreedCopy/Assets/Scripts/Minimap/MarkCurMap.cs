using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarkCurMap : MonoBehaviour
{
    [SerializeField]
    public string       curmapName;
    [SerializeField]
    private Image       imageBlinkedEffect;
    [SerializeField]
    private float       blinkedTime = 0.5f;
    [SerializeField]
    private float       time = 0;
    [SerializeField]
    private GameObject[]  teleportUIs;


    private Color   color;
    [Header("던전맵 방향 오브젝트")]
    [SerializeField]
    private GameObject  Right;
    [SerializeField]
    private GameObject  Left;
    [SerializeField]
    private GameObject  Up;
    [SerializeField]
    private GameObject  Down;
    
    public string   dungeonMapDir;

    private PlayerController    player;
    private MapController       mapController;

    private void Awake()
    {
        player          = FindObjectOfType<PlayerController>();
        mapController   = FindObjectOfType<MapController>();
    }
    private void Start()
    {
        curmapName  = this.gameObject.name;
        color       = imageBlinkedEffect.color;
    }

    private void Update()
    {
        if(curmapName == player.curDungeonName)
        {
            UpdateMarkCurMap();
        }
        else
        {
            color.a = 0;
            imageBlinkedEffect.color = color;
        }

        UpdateMarkMapDir();
        CheckCurMapHaveTeleport();
    }

    private void CheckCurMapHaveTeleport()
    {
        DungeonName curMapObj = GameObject.Find($"Dungeons/{curmapName}")?.GetComponent<DungeonName>();
        // 해당 맵에 텔포트 있으면?
        if (curMapObj.haveTeleport)
        {
            // 현재 맵이 플레이어가 있는 맵일때
            if(curmapName == PlayerController.instance.curDungeonName)
            {
                teleportUIs[0].gameObject.SetActive(false);
                teleportUIs[1].gameObject.SetActive(true);
            }
            // 현재 맵이 플레이어가 없으면
            else
            {
                teleportUIs[0].gameObject.SetActive(true);
                teleportUIs[1].gameObject.SetActive(false);
            }
        }
        // 해당 맵에 텔포트 없으면?
        else
        {
            teleportUIs[0].gameObject.SetActive(false);
            teleportUIs[1].gameObject.SetActive(false);
        }
    }

    private void UpdateMarkCurMap()
    {
        time += Time.deltaTime;
        
        if(time < blinkedTime)
        {
            color.a = 0;
            imageBlinkedEffect.color = color;
        }
        else
        {
            color.a = 1;
            imageBlinkedEffect.color = color;

            if (time > (2 * blinkedTime))
            {
                time = 0;
            }
        }
    }

    private void UpdateMarkMapDir()
    {
        switch (dungeonMapDir)
        {
            case "R":
                Right.SetActive(true);
                break;
            case "L":
                Left.SetActive(true);
                break;
            case "U":
                Up.SetActive(true);
                break;
            case "D":
                Down.SetActive(true);
                break;
            case "N":
                break;  
        }
    }
}
