using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack: MonoBehaviour
{
    [Header("ȭ�� ������ ���� ������")]
    [SerializeField]
    private GameObject  arrowPrefab;        // ������ ȭ�� ������
    private float       arrowSpeed = 50f;  // ȭ���� �ӵ�
    private Transform   arrowSpawn;

    private PoolManager arrowpoolManager;

    public bool isSmoke;

    private SpearMove spearMove;

    [SerializeField]
    private Animator smokePos, shotPos;

    public string sfxName;
    private void Awake()
    {
        arrowpoolManager = new PoolManager(arrowPrefab);
        arrowSpawn = transform.GetChild(0).GetComponent<Transform>();
        spearMove = FindObjectOfType<SpearMove>();
    }

    private void OnApplicationQuit()
    {
        arrowpoolManager.DestroyObjcts();
    }
    void Update()
    {

        PlayerController player = PlayerController.instance;

        if (Input.GetKeyDown(KeyCode.Mouse0)&& player.canAttack && !player.dontMovePlayer && !player.isDie)
        {
            Fire();
            StartCoroutine(PlayerController.instance.AbleToAttack());
            AudioManager.Instance.PlaySFX(sfxName);
            if(isSmoke == true)
            {
                spearMove.AttackRotate();
                smokePos.SetTrigger("OnSmoke");
                shotPos.SetTrigger("OnShot");
            }            
        }
    }
    void Fire()
    {
        GameObject arrow = arrowpoolManager.ActivePoolItem();
        if (arrow != null)
        {
            arrow.transform.position = arrowSpawn.position;
            arrow.transform.rotation = transform.rotation;
            Rigidbody2D rigidbody = arrow.GetComponent<Rigidbody2D>();
            rigidbody.velocity = transform.right * arrowSpeed;
            arrow.GetComponent<Arrow>().Setup(arrowpoolManager);
        }
        else
        {
            Debug.LogWarning("Failed to get arrow from the object pool.");
        }
    }
}
