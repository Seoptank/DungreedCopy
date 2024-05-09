using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneStartPoint : MonoBehaviour
{
    public string                   startPoint;

    [SerializeField]
    private string                  dungeonName;

    private PlayerController        player;
    private MainCameraController    mainCam;
    private FadeEffectController    fade;
    [SerializeField]
    private MapController           map;

    private void Awake()
    {
        player  = FindObjectOfType<PlayerController>();
        mainCam = FindObjectOfType<MainCameraController>();
        fade    = FindObjectOfType<FadeEffectController>();
    }

    private void Start()
    {
        if (startPoint == player.curSceneName)
        {
            AudioManager.Instance.PlayMusic("Dungeon");

            fade.OnFade(FadeState.FadeIn);
            player.curDungeonName      = dungeonName;

            PlayerController.instance.dontMovePlayer = false;

            PlayerController.instance.spriteRenderer.color = new Color(1, 1, 1, 1);
            PlayerController.instance.weaponRenderer.color = new Color(1, 1, 1, 1);

            mainCam.transform.position = new Vector3(transform.position.x,
                                                     transform.position.y,
                                                     mainCam.transform.position.z);
            player.transform.position = this.transform.position;
            
            if(player.curDungeonName == dungeonName)
            {
                if(!map.dungeonNames.Contains(dungeonName))
                {
                    map.dungeonNames.Add(dungeonName);
                    Debug.Log(dungeonName + "이 리스트에 추가됐습니다.");
                }
            }


            GameObject targetObject = GameObject.Find(dungeonName);
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
