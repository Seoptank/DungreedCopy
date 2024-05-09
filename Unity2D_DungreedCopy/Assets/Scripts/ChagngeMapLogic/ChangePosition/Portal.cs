using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("StrartPoint ����")]
    [SerializeField]
    private PortalStartPoint        portalStartPoint;

    private PlayerController        player;

    [Header("�ش� MarkCurMap ����")]
    [SerializeField]
    private MarkCurMap              markCurMap;
    public string                   dungeonMapMoveDir;      // ������: R, ����: L, �Ʒ�:D, ��:U

    [Header("���� �̵��� dungeon������Ʈ ����")]
    [SerializeField]
    private GameObject              nextDungeon;

    private void Awake()
    {
        player  = FindObjectOfType<PlayerController>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Player")
        {
            // �̵��� Ȱ��ȭ
            PlayerDungeonData.instance.isMoving = true;

            // MapUI�� �ش� ���� Ȱ��ȭ
            markCurMap.dungeonMapDir = dungeonMapMoveDir;

            // ���� ���� ������Ʈ Ȱ��ȭ
            nextDungeon.SetActive(true);

            // ���� �̴ϸ� ī�޶� ��Ȱ��ȭ
            MiniMapManager.instance.minimaps[player.curDungeonNum].SetActive(false);
            
            // ���� ���� �ѱ��
            player.curDungeonName   = nextDungeon.GetComponent<DungeonName>().dungeonName;
            player.curDungeonNum = nextDungeon.GetComponent<DungeonName>().dungeonNum;
            
            // ���� �̴ϸ� ī�޶� ��Ȱ��ȭ
            MiniMapManager.instance.minimaps[player.curDungeonNum].SetActive(true);
            
            // FadeOutȿ��
            FadeEffectController.instance.OnFade(FadeState.FadeOut);
            
            // �ڷ�ƾ Ȱ��ȭ 
            StartCoroutine(portalStartPoint.ChangePlayerPosition());
        }
    }
}
