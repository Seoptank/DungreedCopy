using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AbilityController : MonoBehaviour
{
    public static AbilityController instance;

    [Header("통합 포인트")]
    private int maxPoint;   // 최대 포인트
    [SerializeField]
    private int curPoint;   // 현재 포인트
    [SerializeField]
    private TextMeshProUGUI textRemainPoint;            // 남은 Point Text

    [Header("각각의 PointText 변수")]
    [SerializeField]
    private TextMeshProUGUI textWrath;                  // 분노 Text
    [SerializeField]
    private TextMeshProUGUI textSwiftness;              // 신속 Text
    [SerializeField]
    private TextMeshProUGUI textPatience;               // 인내 Text
    [SerializeField]
    private TextMeshProUGUI textArcane;                 // 신비 Text
    [SerializeField]
    private TextMeshProUGUI textGreed;                  // 탐욕 Text

    [Header("각각의 내용Text 변수")]
    [SerializeField]
    private TextMeshProUGUI textWrathDetail;            // 분노 내용 Text
    [SerializeField]
    private TextMeshProUGUI textSwiftnessDetail;        // 신속 내용 Text
    [SerializeField]
    private TextMeshProUGUI textPatienceDetail;         // 인내 내용 Text
    [SerializeField]
    private TextMeshProUGUI textArcaneDetail;           // 신비 내용 Text
    [SerializeField]
    private TextMeshProUGUI textGreedDetail;            // 탐욕 내용 Text

    [Header("1포인트당 얻는 Stat")]
    [SerializeField]
    private int     grantedATK;
    [SerializeField]
    private int     grantedDEF;
    [SerializeField]
    private float   grantedATS;
    [SerializeField]
    private float   grantedCRI;
    [SerializeField]
    private int     grantedHP;

    [HideInInspector]
    public int wrathPoint;                 // 분노 Point
    [HideInInspector]
    public int swiftnessPoint;             // 신속 Point
    [HideInInspector]
    public int patiencePoint;              // 인내 Point
    [HideInInspector]
    public int arcanePoint;                // 신비 Point
    [HideInInspector]
    public int greedPoint;                 // 탐욕 Point


    private Animator ani;
    private NPC      npc;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ani = GetComponent<Animator>();
        npc = FindObjectOfType<NPC>();

        maxPoint = PlayerStats.instance.LV;
        curPoint = maxPoint;
    }
    private void Update()
    {
        UpdateTextPoints();

        maxPoint = PlayerStats.instance.LV;
    }

    private void UpdateTextPoints()
    {
        textRemainPoint.text = "남은 포인트 : " + curPoint;

        textWrath.text = wrathPoint.ToString();
        textSwiftness.text = swiftnessPoint.ToString();
        textPatience.text = patiencePoint.ToString();
        textArcane.text = arcanePoint.ToString();
        textGreed.text = greedPoint.ToString();

        textWrathDetail.text        = "<color=green>+" + wrathPoint.ToString() + "</color> 위력";
        textSwiftnessDetail.text    = "<color=green>+" + (swiftnessPoint * 10).ToString() + "%" + " </color> 공격 속도";
        textPatienceDetail.text     = "<color=green>+" + patiencePoint.ToString() + " </color> 방어력";
        textArcaneDetail.text       = "<color=green>+" + (arcanePoint * 5).ToString() + "%" + " </color> 크리티컬";
        textGreedDetail.text        = "<color=green>+" + (greedPoint * 5).ToString() + " </color> 최대 체력";
    }
    //==========================================================================================
    // YS: 버튼 상호작용 함수들
    //==========================================================================================
    public void WrathPointUP()
    {
        if (curPoint > 0)
        {
            curPoint--;
            wrathPoint++;

            PlayerStats.instance.AddATK(grantedATK);
        }
    }
    public void SwiftnessPointUP()
    {
        if (curPoint > 0)
        {
            curPoint--;
            swiftnessPoint++;

            PlayerStats.instance.AddATS(grantedATS);
        }
    }
    public void PatiencePointUP()
    {
        if (curPoint > 0)
        {
            curPoint--;
            patiencePoint++;

            PlayerStats.instance.AddDEF(grantedDEF);
        }
    }
    public void ArcanePointUP()
    {
        if (curPoint > 0)
        {
            curPoint--;
            arcanePoint++;

            PlayerStats.instance.AddCRI(grantedCRI);
        }
    }
    public void GreedPointUP()
    {
        if (curPoint > 0)
        {
            curPoint--;
            greedPoint++;

            PlayerStats.instance.AddMaxHP(grantedHP);
        }
    }

    public void ExitAbillityUI()
    {
        ani.Play("AbilityHide");
        npc.inputKey = false;
        PlayerController.instance.dontMovePlayer = false;
    }

    public void ResetAllAbility()
    {
        wrathPoint      = 0;
        swiftnessPoint  = 0;
        patiencePoint   = 0;
        arcanePoint     = 0;
        greedPoint      = 0;
        curPoint = maxPoint;

        PlayerStats.instance.ResetAllStat();
    }
    //==========================================================================================
    // YS: 애니메이션 이벤트 함수
    //==========================================================================================
    public void DeactivateAbilityObject()
    {
        this.gameObject.SetActive(false);
    }
}