using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainCameraController : MonoBehaviour
{
    static public MainCameraController instance;

    private float           shakeTime;
    private float           shakeIntensity;
    private Vector3         originPos;
    private Vector3         originRot;

    [SerializeField]
    private Transform       player;
    [SerializeField]
    private float           smooting = 0.2f;

    public BoxCollider2D    bound;

    // YS: 박스 콜라이더 영역의 최소/ 최대 x,y,z값을 지닐 변수
    private Vector3         minBound;
    private Vector3         maxBound;

    // YS: 카페라의 반너비, 반높이 값을 지닐 변수
    private float           halfWidth;
    private float           halfHeight;

    // YS: 카메라의 반높이 값의 속성을 이용하기 위한 변수
    private Camera          halfHeightCam;

    private PlayerController playerController;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;

            playerController = FindObjectOfType<PlayerController>();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        halfHeightCam = GetComponent<Camera>();
        
        minBound = bound.bounds.min;
        maxBound = bound.bounds.max;

        // YS: 반너비 구하는 공식 = 반높이 * Screen.width / Screen.height(Screen.식은 해상도를 나타냄)
        halfHeight = halfHeightCam.orthographicSize;
        halfWidth = halfHeight * Screen.width / Screen.height;
    }
    private void FixedUpdate()
    {
        if(MySceneManager.instance.CurSceneIsStartScene())
        {
            transform.position = new Vector3(0, 0, transform.position.z);
        }

        if(playerController.playerMeetsBoss == false && playerController.isBossDie == false)
        {
            ChasePlayer();
        }

        float clampedX = Mathf.Clamp(this.transform.position.x, minBound.x + halfWidth, maxBound.x - halfWidth);
        float clampedY = Mathf.Clamp(this.transform.position.y, minBound.y + halfHeight, maxBound.y - halfHeight);

        this.transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    public void SetBound(BoxCollider2D newBound)
    {
        bound = newBound;
        minBound = bound.bounds.min;
        maxBound = bound.bounds.max;
    }

    public IEnumerator ChangeView(Transform changePos, float camMoveTime)
    {
        float elapsedTime = 0f;
        Vector3 targetPos = new Vector3(changePos.position.x, changePos.position.y, changePos.position.z - 10);
       
        while(elapsedTime< camMoveTime)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, elapsedTime / camMoveTime);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

    }
    public void OnShakeCamByPos(float shakeTime = 1.0f, float shakeIntensity = 0.1f)
    {
        this.shakeTime = shakeTime;
        this.shakeIntensity = shakeIntensity;

        StopCoroutine("ShakePos");
        StartCoroutine("ShakePos");
    }
    public void OnShakeCamByRot(float shakeTime = 1.0f, float shakeIntensity = 0.1f)
    {
        this.shakeTime = shakeTime;
        this.shakeIntensity = shakeIntensity;

        StopCoroutine("ShakeRot");
        StartCoroutine("ShakeRot");
    }

    private IEnumerator ShakePos()
    {
        Vector3 StartPos = transform.position;

        while (shakeTime > 0.0f)
        {
            transform.position = StartPos + Random.insideUnitSphere * shakeIntensity;

            shakeTime -= Time.deltaTime;

            yield return null;
        }

        transform.position = StartPos;
    }

    private IEnumerator ShakeRot()
    {
        Vector3 startRot = transform.eulerAngles;

        float power = 10.0f;

        while(shakeTime > 0.0f)
        {
            float x = 0;
            float y = 0;
            float z = Random.Range(-1f,1f);

            transform.rotation = Quaternion.Euler(startRot + new Vector3(x, y, z) * shakeIntensity * power);

            shakeTime -= Time.deltaTime;

            yield return null;
        }

        transform.rotation = Quaternion.Euler(startRot);
    }
    public void ChasePlayer()
    {
        Vector3 targetPos = new Vector3(player.position.x, player.position.y, this.transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, smooting);
    }

    
}
