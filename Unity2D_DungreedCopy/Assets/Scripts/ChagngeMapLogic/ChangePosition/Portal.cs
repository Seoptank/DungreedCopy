using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("StrartPoint 삽입")]
    [SerializeField]
    private PortalStartPoint        portalStartPoint;

    private PlayerController        player;

    [Header("해당 MarkCurMap 삽입")]
    [SerializeField]
    private MarkCurMap              markCurMap;
    public string                   dungeonMapMoveDir;      // 오른쪽: R, 왼쪽: L, 아래:D, 위:U

    [Header("다음 이동할 dungeon오브젝트 삽입")]
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
            // 이동중 활성화
            PlayerDungeonData.instance.isMoving = true;

            // MapUI의 해당 방향 활성화
            markCurMap.dungeonMapDir = dungeonMapMoveDir;

            // 다음 던전 오브젝트 활성화
            nextDungeon.SetActive(true);

            // 현재 미니맵 카메라 비활성화
            MiniMapManager.instance.minimaps[player.curDungeonNum].SetActive(false);
            
            // 던전 정보 넘기기
            player.curDungeonName   = nextDungeon.GetComponent<DungeonName>().dungeonName;
            player.curDungeonNum = nextDungeon.GetComponent<DungeonName>().dungeonNum;
            
            // 다음 미니맵 카메라 비활성화
            MiniMapManager.instance.minimaps[player.curDungeonNum].SetActive(true);
            
            // FadeOut효과
            FadeEffectController.instance.OnFade(FadeState.FadeOut);
            
            // 코루틴 활성화 
            StartCoroutine(portalStartPoint.ChangePlayerPosition());
        }
    }
}
