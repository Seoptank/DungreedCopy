using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BossState 
{
    None = -1,
    Idle = 0,
    HeadAttack,    
    HandsAttack,   
    SwordAttack,
    Die
}
public class BossPattern : MonoBehaviour
{
    public BossState   bossState;

    [Header("Die")]
    private PoolManager     explosionEffectPoolManager;
    [SerializeField]
    private float           slowFactor;
    [SerializeField]
    private float           fasterRate;
    [SerializeField]
    private Image           imageBossDieEffect;
    [SerializeField]
    private GameObject      explosionEffectPrefab;
    [SerializeField]
    private int             explosionEffectCount;       // 생성할 폭발이펙트 수
    [SerializeField]
    private GameObject      diePiecePrefab;
    [SerializeField]
    private Transform       camViewPos;
    public bool             isDie = false;

    [Header("HeadAttack")]
    [SerializeField]
    private GameObject      headBulletPrefab;
    [SerializeField]
    private int             angleInterval = -10;    // 양수 = 반시계 방향, 음수 = 시계 방향
    [SerializeField]
    private int             fireDirCount = 4;       // bullet이 나가는 방향의 갯수
    [SerializeField]
    private float           fireRateTime = 0.2f;    // bullet의 생성 시간 제어
    [HideInInspector]
    public  PoolManager     headAttackPoolManager;
    [SerializeField]
    private float           headAttackMinTime = 3.0f;
    [SerializeField]
    private float           headAttackMaxTime = 5.0f;
    [SerializeField]
    private float           headAttackTime = 0;
    [SerializeField]
    private bool            isHeadAttack;
    [SerializeField]
    private Transform       headAttackTransform;


    [Header("SwordAttack")]
    [SerializeField]
    private GameObject          bossSwordSpawnPrefab;
    [HideInInspector]
    public  PoolManager         bossSwordSpawnPoolManager;
    [SerializeField]
    private float               bossSwordSpawnDelayTime;
    [SerializeField]
    private Transform[]         spawnTransforms;

    [Header("HandsAttack")]
    [SerializeField]
    private GameObject          selectedHand;
    [SerializeField]
    private GameObject          leftHand;
    [SerializeField]
    private GameObject          rightHand;
    [SerializeField]
    private float               waitHandAttackTime;
    [SerializeField]
    private float               handsMoveTime;
    [SerializeField]
    private int                 count;
    [SerializeField]
    private int                 maxCount;
    [SerializeField]
    private int                 minCount;
    public  bool                isHandsAttack = false;

    private GameObject              player;
    private BossController          boss;
    private UIEffectManager         uiEffectManager;
    private MainCameraController    mainCam;
    private PlayerController        playerController;
    private BossHP                  bossHP;

    private void OnEnable()
    {
        AudioManager.Instance.PlayMusic("Boss");
        StartCoroutine(AutoChangeToIdle());
    }
    private IEnumerator AutoChangeToIdle()
    {
        yield return new WaitForSeconds(4f);
        ChangeBossState(BossState.Idle);    
    }
    private void OnDisable()
    {
        StopCoroutine(bossState.ToString());
        bossState = BossState.None;
    }
    private void Awake()
    {
        headAttackPoolManager       = new PoolManager(headBulletPrefab);
        bossSwordSpawnPoolManager   = new PoolManager(bossSwordSpawnPrefab);
        explosionEffectPoolManager  = new PoolManager(explosionEffectPrefab);

        player = GameObject.FindGameObjectWithTag("Player");

        boss = GetComponent<BossController>();
        bossHP = GetComponent<BossHP>();

        uiEffectManager     = FindObjectOfType<UIEffectManager>();
        mainCam             = FindObjectOfType<MainCameraController>();
        playerController    = FindObjectOfType<PlayerController>();

        imageBossDieEffect.gameObject.SetActive(false);
    }
    private void OnApplicationQuit()
    {
        headAttackPoolManager.DestroyObjcts();
        bossSwordSpawnPoolManager.DestroyObjcts();
        explosionEffectPoolManager.DestroyObjcts();
    }
    private void Update()
    {
        // HeadAttack을 랜덤 시간으로 돌리기 위한 조건
        if(isHeadAttack)
        {
            headAttackTime += Time.deltaTime;

            if(headAttackTime > Random.Range(headAttackMinTime,headAttackMaxTime))
            {
                isHeadAttack = false;

                if(!isHeadAttack)
                {
                    StartCoroutine(HeadAttackTimeReturnZero());
                }
            }
        }

        CheckDie();
    }

    private void CheckDie()
    {
        if(bossHP.curHP <= 0)
        {
            ChangeBossState(BossState.Die);
        }
    }

    private IEnumerator Idle()
    {
        yield return new WaitForSeconds(1f);

        //실제 사용할 코드
        StartCoroutine("AutoChangeBossAttack");
        
        while (true)
        {
            // "Idle"일때 하는 행동
            yield return null;
        }
    }

    private IEnumerator Die()
    {
        isDie =  true;
        AudioManager.Instance.OffMusic();
        imageBossDieEffect.gameObject.SetActive(true);
        imageBossDieEffect.color = Color.white;

        yield return StartCoroutine(uiEffectManager.UIFade(imageBossDieEffect, 1, 0));

        yield return new WaitForSeconds(1f);
        imageBossDieEffect.gameObject.SetActive(false);

        StartCoroutine(ChangeCamPos());
        PlayExplosionEffect(transform.position + new Vector3(0.5f, -1f, 0), Quaternion.identity, new Vector3(2f, 2f, 2f));
        AudioManager.Instance.PlaySFX("EnemyDie");

        yield return new WaitForSeconds(1f);

        playerController.isBossDie = true;
        Time.timeScale = slowFactor;

        for (int i = 0; i <= explosionEffectCount; i++)
        {
            yield return new WaitForSeconds(0.05f);
            Vector2 randomPos = new Vector2(Random.Range(transform.position.x - 4, transform.position.x + 4),
                                            Random.Range(transform.position.y - 4, transform.position.y + 4));

            PlayExplosionEffect(randomPos, Quaternion.identity,Vector3.one);
            mainCam.OnShakeCamByPos(0.05f, 0.1f);
            Time.timeScale += fasterRate;

            AudioManager.Instance.PlaySFX("EnemyDie");

            if (i >= explosionEffectCount)
            {
                SpawnDiePiece();
                break;
            }
        }

        Destroy(gameObject);
    }

    private IEnumerator ChangeCamPos()
    {
        while(true)
        {
            StartCoroutine(MainCameraController.instance.ChangeView(transform, 0.5f));
            yield return null;
        }
    }
    private void PlayExplosionEffect(Vector3 position, Quaternion rotation,Vector3 scale)
    {
        GameObject explosionEffect = explosionEffectPoolManager.ActivePoolItem();
        explosionEffect.transform.position = position;
        explosionEffect.transform.rotation = rotation;
        explosionEffect.GetComponent<EffectPool>().Setup(explosionEffectPoolManager);
    }

    private void SpawnDiePiece()
    {
        GameObject diePiece = Instantiate(diePiecePrefab, transform.position + new Vector3(1.45f, -1f, 0), Quaternion.identity);
        Destroy(gameObject);
    }

    private IEnumerator AutoChangeBossAttack()
    {
        yield return new WaitForSeconds(1);

        int count = Random.Range(0, 3);
        switch (count)
        {
            case 0:
                ChangeBossState(BossState.HandsAttack);
                break;
            case 1:
                ChangeBossState(BossState.HeadAttack);
                break;
            case 2:
                ChangeBossState(BossState.SwordAttack);
                break;
        }
    }

    private IEnumerator HandsAttack()
    {
        count = Random.Range(minCount, maxCount);

        while (count >= 0)
        {
             count--;

            int randomIndex = Random.Range(0, 2);

            if(randomIndex == 0)
            {
                selectedHand = leftHand;
            }
            else
            {
                selectedHand = rightHand;
            }

            Vector2 startPos = selectedHand.transform.position;
            Vector2 targetPos = new Vector2(selectedHand.transform.position.x, player.transform.position.y);

            float elapsedTime = 0f;

            while(elapsedTime < handsMoveTime)
            {
                selectedHand.transform.position = Vector2.Lerp(startPos, targetPos, elapsedTime / handsMoveTime);
                elapsedTime += Time.deltaTime;
             
                if(elapsedTime >= handsMoveTime)
                {
                    selectedHand.GetComponent<BossHands>().StartAttackAni();
                }
                yield return null;
            }
            yield return new WaitForSeconds(waitHandAttackTime);

        }

        ChangeBossState(BossState.Idle);
    }
    private IEnumerator HeadAttackTimeReturnZero()
    {
        yield return new WaitForSeconds(3f);
        headAttackTime = 0;

    }
    private IEnumerator SwordAttack()
    {
        for (int i = 0; i < spawnTransforms.Length; ++i)
        {
            yield return new WaitForSeconds(bossSwordSpawnDelayTime);
            GameObject bossSwordSpawn = bossSwordSpawnPoolManager.ActivePoolItem();
            bossSwordSpawn.transform.position = spawnTransforms[i].position;
            bossSwordSpawn.transform.rotation = transform.rotation;
            bossSwordSpawn.GetComponent<BossSwordSpawnEffect>().Setup(bossSwordSpawnPoolManager,this);
        }
        StartCoroutine("AutoChangeFromSwordToIdle");
    }
    private IEnumerator AutoChangeFromSwordToIdle()
    {
        yield return new WaitForSeconds(6);

        ChangeBossState(BossState.Idle);
    }
    private IEnumerator HeadAttack()
    {
        GameObject bossHead = GameObject.Find("BossHead");
        bossHead.GetComponent<Animator>().SetBool("IsHeadAttack", true);

        float currentAngle = 0f; // 현재 각도
        float angleIncrement = 360f / fireDirCount; // 각도 증가량

        isHeadAttack = true;

        while (isHeadAttack)
        {
            for (int i = 0; i < fireDirCount; ++i)
            {
                float angle = currentAngle + i * angleIncrement;
                Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

                GameObject tempObj = headAttackPoolManager.ActivePoolItem();
                tempObj.transform.right = direction;
                tempObj.transform.position = headAttackTransform.position;
                tempObj.GetComponent<BossHeadBullet>().Setup(headAttackPoolManager,this);
            }

            yield return new WaitForSeconds(fireRateTime);

            currentAngle += angleInterval;
            currentAngle %= 360; // 각도 정규화
        }

        bossHead.GetComponent<Animator>().SetBool("IsHeadAttack", false);
        ChangeBossState(BossState.Idle);
    }

    public void ChangeBossState(BossState newState)
    {

        if (bossState == newState) return;

        // 이전에 재생하던 상태 종료 
        StopCoroutine(bossState.ToString());

        // 상태 변경
        bossState = newState;
        
        // 새로운 상태 재생
        StartCoroutine(bossState.ToString());
    }
}
