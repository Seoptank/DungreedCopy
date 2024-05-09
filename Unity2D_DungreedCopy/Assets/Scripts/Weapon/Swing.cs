using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing : MonoBehaviour
{
    private Transform player;

    public Transform SwingPos;
    [SerializeField]
    private  GameObject SwingObj;

    public EquipWeapon equipWeapon;
    [SerializeField]
    private SwordPosSwitching sword;
    [SerializeField]
    private bool swingWeapon;

    private PoolManager swingPoolManager;

    private SpearMove spearMove;
    void Awake()
    {
        equipWeapon = GetComponent<EquipWeapon>();

        player = GameObject.Find("Player").transform;

        spearMove = FindObjectOfType<SpearMove>();

        swingPoolManager = new PoolManager(SwingObj);
    }

    private void OnApplicationQuit()
    {
        swingPoolManager.DestroyObjcts();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerController player = PlayerController.instance;
        PlayerDungeonData data  = PlayerDungeonData.instance;

        UpdateSight();

        if(player.canAttack && !player.dontMovePlayer && !player.isDie && !data.isMoving)
        {
            SwingSword();
        }
    }

    void UpdateSight()
    {
        // 마우스 위치를 가져와서 월드 좌표로 변환
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 플레이어 위치에서 마우스 위치까지의 방향 벡터
        Vector3 directionToMouse = mousePosition - player.position;

        // 오브젝트를 마우스 방향으로 z축 회전
        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 마우스 x좌표를 읽어 scale값을 -1로 바꾸어 이미지 반전
        Vector2 scale = transform.localScale;
        if (directionToMouse.x < 0)
        {
            scale.y = -1;
        }
        else
        {
            scale.y = 1;
        }
        transform.localScale = scale;
    }

    void SwingSword()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameObject swing = swingPoolManager.ActivePoolItem();
            if(swing != null)
            {
                swing.transform.position = SwingPos.position;
                swing.transform.rotation = transform.rotation;
                swing.GetComponent<EffectPool>().Setup(swingPoolManager);
                
                if (swingWeapon)
                    sword.SwordPosition();
                else
                    spearMove.AttackMove();

                AudioManager.Instance.PlaySFX("Swing");
                StartCoroutine(PlayerController.instance.AbleToAttack());
            }
        }
    }
}
