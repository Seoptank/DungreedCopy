using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalStartPoint : MonoBehaviour
{
    private  string         startingMapName;

    private PlayerController        player;
    private FadeEffectController    fade;
    private MainCameraController    mainCam;
    private MapController           map;

    private void Awake()
    {
        player      = FindObjectOfType<PlayerController>();
        fade        = FindObjectOfType<FadeEffectController>();
        mainCam     = FindObjectOfType<MainCameraController>();
        map         = FindObjectOfType<MapController>();
        
    }
    public IEnumerator ChangePlayerPosition()
    {
        startingMapName = player.curDungeonName;

        yield return new WaitForSeconds(fade.fadeTime);
            
        if (startingMapName == player.curDungeonName)
        {
            // 플레이어 위치 이동
            player.transform.position = this.transform.position;


            // 카메라 위치 이동
            mainCam.transform.position = new Vector3(this.transform.position.x,
                                                     this.transform.position.y,
                                                     mainCam.transform.position.z);
            if (!map.dungeonNames.Contains(startingMapName))
            {
                map.dungeonNames.Add(startingMapName);
                Debug.Log(startingMapName + "이 리스트에 추가됐습니다.");
            }

            // 페이드 효과
            fade.OnFade(FadeState.FadeIn);

            // 이동중 비활성화
            PlayerController.instance.dontMovePlayer = false;
            PlayerDungeonData.instance.isMoving = false;

            GameObject targetObject = GameObject.Find(startingMapName);

            if (targetObject != null)
            {
                BoxCollider2D targetBound = targetObject.GetComponent<BoxCollider2D>();

                // 바운드 재설정
                mainCam.SetBound(targetBound);
            }
            else
            {
                Debug.LogWarning("Target object with the specified name not found.");
            }

        }
    }
}
