using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// YS
public class DungeonPortalController : MonoBehaviour
{
    [SerializeField]
    private GameObject              dungeonPortalPrefab;

    private PoolManager             dungeonPortalPoolMnager;
    private PlayerController        player;

    private void Awake()
    {
        dungeonPortalPoolMnager = new PoolManager(dungeonPortalPrefab);
        
        player = FindObjectOfType<PlayerController>();
    }
    private void OnApplicationQuit()
    {
        dungeonPortalPoolMnager.DestroyObjcts();
    }

    private void ActiveDungeonPortal()
    {
        GameObject dungeonPortal = dungeonPortalPoolMnager.ActivePoolItem();
        dungeonPortal.transform.position = new Vector3(player.transform.position.x,
                                                       transform.position.y+5f);
        dungeonPortal.transform.rotation = transform.rotation;
        dungeonPortal.GetComponent<DungeonPortal>().Setup(dungeonPortalPoolMnager);
    }

    // 플레이어와 충돌시 MemoryPool로 DungeonPortal 불러오기
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Player")
        {
            ActiveDungeonPortal();
            PlayerController.instance.dontMovePlayer = true;
        }
    }
}
