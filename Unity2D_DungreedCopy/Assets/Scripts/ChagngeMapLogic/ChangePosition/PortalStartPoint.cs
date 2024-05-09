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
            // �÷��̾� ��ġ �̵�
            player.transform.position = this.transform.position;


            // ī�޶� ��ġ �̵�
            mainCam.transform.position = new Vector3(this.transform.position.x,
                                                     this.transform.position.y,
                                                     mainCam.transform.position.z);
            if (!map.dungeonNames.Contains(startingMapName))
            {
                map.dungeonNames.Add(startingMapName);
                Debug.Log(startingMapName + "�� ����Ʈ�� �߰��ƽ��ϴ�.");
            }

            // ���̵� ȿ��
            fade.OnFade(FadeState.FadeIn);

            // �̵��� ��Ȱ��ȭ
            PlayerController.instance.dontMovePlayer = false;
            PlayerDungeonData.instance.isMoving = false;

            GameObject targetObject = GameObject.Find(startingMapName);

            if (targetObject != null)
            {
                BoxCollider2D targetBound = targetObject.GetComponent<BoxCollider2D>();

                // �ٿ�� �缳��
                mainCam.SetBound(targetBound);
            }
            else
            {
                Debug.LogWarning("Target object with the specified name not found.");
            }

        }
    }
}
