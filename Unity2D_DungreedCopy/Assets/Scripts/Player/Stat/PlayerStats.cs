using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HPEvent : UnityEngine.Events.UnityEvent<float, float> { }
public class DCEvect : UnityEngine.Events.UnityEvent<int, int> { }
public class PlayerStats : StatManager
{
    public static PlayerStats instance;

    [HideInInspector]
    public HPEvent onHPEvent = new HPEvent();
    [HideInInspector]
    public DCEvect onDCEvent = new DCEvect();

    private PlayerController playerController;

    public float tempMaxHP = 100;
    public float curEXP;
    public float targetEXP;
    [SerializeField]
    public int      gold;

    private int     originATK = 2;    // ���� ������ �ִ� ���ݷ�
    private int     originDEF = 2;    // ���� ������ �ִ� ����
    private int     originATS = 0;    // ������ ������ �ִ� ����
    private float   originCRI = 0.1f; // ũ��Ƽ�� Ȯ��(1�� 100%)

    [Header("�÷��̾� ����")]
    public int      ATK;            // ���ݷ�
    public int      DEF;            // ���� 
    public float    ATS;            // ����(1�ʿ� n��)
    public float    CRI;            // ũ��Ƽ��(1�� 100%)
    public int      DashATK = 10;   // ��� ���ݷ�

    [Header("���� ����")]
    public int WP_MINATK; 
    public int WP_MAXATK; 
    public int WP_DEF; 
    public float WP_ATS; 
    public float WP_CRI;
    

    public void ResetAllStat()
    {
        ATK = originATK;
        DEF = originDEF;
        ATS = originATS;
        CRI = originCRI;
        tempMaxHP = 100;
        WP_MINATK = 10;
        WP_MAXATK = 15;
        WP_ATS = 3;
        DashATK = 10;
        HP = MaxHP;
        DC = MaxDC;
    }

    private void Awake()
    {
        base.Setup();
        playerController = GetComponent<PlayerController>();
    }
    [HideInInspector]
    public float            RecoverTimeDC = 1.5f;
    [HideInInspector]
    public float            timer; 

    public override float MaxHP
    {
        get
        {
            return tempMaxHP;
        }
    }
    public override int     MaxDC => 3;
    public override int     MaxGOLD => 999999;
    public override int     MaxLV => 50;

    private void Start()
    {
        instance = this;

        StartCoroutine("RecoveryDC");
        timer = 0;
    }

    private void Update()
    {
        RecoveryDC();
        LevelUP();
    }

    public void UseDC()
    {
        DC = Mathf.Max(DC - 1, 0);
    }

    private void RecoveryDC()
    {
        if(DC < MaxDC && DashRecoveryTimerExpired())
        {
            DC = Mathf.Min(DC + 1, MaxDC);
            DashRecoveryTimerExpired();
            if(timer >= RecoverTimeDC)
            {
                timer = 0;
            }
        }
    }

    public void TakeGold(int value)
    {
        GOLD += value;
    }

    public bool DecreaseHP(float monAtt)
    {
        float preHP = HP;
        HP = HP - monAtt > 0 ? HP - monAtt : 0;

        onHPEvent.Invoke(preHP, HP);

        if(HP <= 0)
        {
            return true;
        }
        return false;
    }

    public void IncreaseHP(float heal)
    {
        float preHP = HP;

        HP = HP + heal > MaxHP ? MaxHP : HP + heal;

        onHPEvent.Invoke(preHP, HP);
    }
    private bool DashRecoveryTimerExpired()
    {
        timer += Time.deltaTime;
        return timer >= RecoverTimeDC;
    }

    public void AddMaxHP(int grantedHP)
    {
        tempMaxHP += grantedHP;
        HP = MaxHP;
    }

    public void AddEXP(float grantedEXP)
    {
        curEXP += grantedEXP;

       
    }

    private void LevelUP()
    {
        if (curEXP >= targetEXP)
        {
            LV++;

            curEXP = curEXP - targetEXP;

            targetEXP += (targetEXP * 0.5f);
        }
        else return;
    }

    public void AddATK(int grantedAtk)
    {
        ATK += grantedAtk;
    }
    public void AddDEF(int grantedDef)
    {
        DEF += grantedDef;
    }
    public void AddCRI(float grantedCri)
    {
        CRI += grantedCri;
    }
    public void AddATS(float grantedATS)
    {
        ATS += grantedATS;
    }

    private void TotalATK()
    {
        // ���ݽ� ���� ������ ����
        // ������ = ��������.����(�ּҰ� + Player_ATK  ~  �ִ밪 + Player_ATK ) 
    }
}
