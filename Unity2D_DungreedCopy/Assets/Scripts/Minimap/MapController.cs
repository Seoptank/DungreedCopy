using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour
{

    [Header("���� ��")]
    [SerializeField]
    private GameObject[] dungeonMaps;
    public List<string> dungeonNames;

    [Header("�̴� ��")]
    [SerializeField]
    private GameObject  miniMap;

    [Header("Teleport")]
    public GameObject startTeleport;
    public GameObject targetTeleport;

    public GameObject MapUI;
    public bool MapOn = false;

    private void Awake()
    {
        miniMap = transform.GetChild(0).gameObject;
    }

    void Update()
    {
        DontActivateDungeonMap();
        UpdateDungeonMapUI();
        MapUI.SetActive(MapOn);
        miniMap.SetActive(!PlayerController.instance.dontMovePlayer);
        MinimapControl();
    }
    private void DontActivateDungeonMap()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (PlayerDungeonData.instance.isFighting)
            {
                UIManager.instance.StartCoroutine("OnNotificationTxt");
                return;
            }
            MapOn = true;
            PlayerController.instance.dontMovePlayer = true;
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            if (startTeleport == null)
            {
                MapOn = false;
                PlayerController.instance.dontMovePlayer = false;
            }
            else
            {
                if (!startTeleport.GetComponent<TeleportController>().inputKey)
                {
                    MapOn = false;
                    PlayerController.instance.dontMovePlayer = false;
                }
            }
        }
    }

    public void OffDungeonMap()
    {
        MapOn = false;
        PlayerController.instance.dontMovePlayer = false;
        startTeleport.GetComponent<TeleportController>().inputKey = false;
    }

    private void UpdateDungeonMapUI()
    {
        for (int i = 0; i < dungeonMaps.Length; ++i)
        {
            if (dungeonNames.Contains(dungeonMaps[i].name))
            {
                dungeonMaps[i].SetActive(true);
            }
            else
            {
                dungeonMaps[i].SetActive(false);
            }

        }
    }

    void MinimapControl()
    {
        if (PlayerController.instance.dontMovePlayer || PlayerController.instance.curDungeonName == "BossRoom" || PlayerController.instance.curDungeonName == "NextStage")
        {
            miniMap.SetActive(false);
        }
        else if (!PlayerController.instance.dontMovePlayer)
        {
            miniMap.SetActive(!PlayerController.instance.dontMovePlayer);
        }
    }

    public IEnumerator ChangePosPlayer()
    {

        // Ÿ���� �Ǵ� �ڷ���Ʈ�� ��ġ �޾ƿ���
        Transform targetTelPos = targetTeleport.GetComponent<TeleportDungeon>().teleport.transform;
        // Ÿ���� �Ǵ� ������ �̸� 
        DungeonName targetDungeonName = targetTeleport.GetComponent<DungeonName>();



        yield return new WaitForSeconds(FadeEffectController.instance.fadeTime);

        startTeleport.GetComponent<TeleportController>().inputKey = false;
        startTeleport.GetComponent<Animator>().SetBool("EatPlayer", false);

        // ���� ���� �缳��
        PlayerController.instance.curDungeonName = targetDungeonName.dungeonName;
        PlayerController.instance.curDungeonNum = targetDungeonName.dungeonNum;

        if (PlayerController.instance.curDungeonName == targetDungeonName.dungeonName)
        {
            // ���̵� �� ȿ�� ����
            FadeEffectController.instance.OnFade(FadeState.FadeIn);

            // �÷��̾� �̵�
            PlayerController.instance.transform.position = targetTelPos.position;

            // �÷��̾� �̵� ����
            PlayerController.instance.dontMovePlayer = false;

            // a �ʱ�ȭ
            PlayerController.instance.spriteRenderer.color = new Color(1, 1, 1, 1);
            PlayerController.instance.weaponRenderer.color = new Color(1, 1, 1, 1);

            MainCameraController.instance.transform.position = new Vector3(targetTelPos.position.x,
                                                                           targetTelPos.position.y,
                                                                           MainCameraController.instance.transform.position.z);
            GameObject targetObject = GameObject.Find(PlayerController.instance.curDungeonName);

            if (targetObject != null)
            {
                BoxCollider2D targetBound = targetObject.GetComponent<BoxCollider2D>();

                // �ٿ�� �缳��
                MainCameraController.instance.SetBound(targetBound);
            }
            else
            {
                Debug.LogWarning("Target object with the specified name not found.");
            }
        }


    }


}
