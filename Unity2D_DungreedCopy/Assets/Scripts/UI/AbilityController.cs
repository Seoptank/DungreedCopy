using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AbilityController : MonoBehaviour
{
    public static AbilityController instance;

    [Header("���� ����Ʈ")]
    private int maxPoint;   // �ִ� ����Ʈ
    [SerializeField]
    private int curPoint;   // ���� ����Ʈ
    [SerializeField]
    private TextMeshProUGUI textRemainPoint;            // ���� Point Text

    [Header("������ PointText ����")]
    [SerializeField]
    private TextMeshProUGUI textWrath;                  // �г� Text
    [SerializeField]
    private TextMeshProUGUI textSwiftness;              // �ż� Text
    [SerializeField]
    private TextMeshProUGUI textPatience;               // �γ� Text
    [SerializeField]
    private TextMeshProUGUI textArcane;                 // �ź� Text
    [SerializeField]
    private TextMeshProUGUI textGreed;                  // Ž�� Text

    [Header("������ ����Text ����")]
    [SerializeField]
    private TextMeshProUGUI textWrathDetail;            // �г� ���� Text
    [SerializeField]
    private TextMeshProUGUI textSwiftnessDetail;        // �ż� ���� Text
    [SerializeField]
    private TextMeshProUGUI textPatienceDetail;         // �γ� ���� Text
    [SerializeField]
    private TextMeshProUGUI textArcaneDetail;           // �ź� ���� Text
    [SerializeField]
    private TextMeshProUGUI textGreedDetail;            // Ž�� ���� Text

    [Header("1����Ʈ�� ��� Stat")]
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
    public int wrathPoint;                 // �г� Point
    [HideInInspector]
    public int swiftnessPoint;             // �ż� Point
    [HideInInspector]
    public int patiencePoint;              // �γ� Point
    [HideInInspector]
    public int arcanePoint;                // �ź� Point
    [HideInInspector]
    public int greedPoint;                 // Ž�� Point


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
        textRemainPoint.text = "���� ����Ʈ : " + curPoint;

        textWrath.text = wrathPoint.ToString();
        textSwiftness.text = swiftnessPoint.ToString();
        textPatience.text = patiencePoint.ToString();
        textArcane.text = arcanePoint.ToString();
        textGreed.text = greedPoint.ToString();

        textWrathDetail.text        = "<color=green>+" + wrathPoint.ToString() + "</color> ����";
        textSwiftnessDetail.text    = "<color=green>+" + (swiftnessPoint * 10).ToString() + "%" + " </color> ���� �ӵ�";
        textPatienceDetail.text     = "<color=green>+" + patiencePoint.ToString() + " </color> ����";
        textArcaneDetail.text       = "<color=green>+" + (arcanePoint * 5).ToString() + "%" + " </color> ũ��Ƽ��";
        textGreedDetail.text        = "<color=green>+" + (greedPoint * 5).ToString() + " </color> �ִ� ü��";
    }
    //==========================================================================================
    // YS: ��ư ��ȣ�ۿ� �Լ���
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
    // YS: �ִϸ��̼� �̺�Ʈ �Լ�
    //==========================================================================================
    public void DeactivateAbilityObject()
    {
        this.gameObject.SetActive(false);
    }
}