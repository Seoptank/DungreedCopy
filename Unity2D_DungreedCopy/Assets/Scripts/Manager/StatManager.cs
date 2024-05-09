using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Stats
{
    [HideInInspector]
    public float    HP;     // �÷��̾� ü��
    [HideInInspector]       
    public int      DC;     // �÷��̾� ��� ī��Ʈ
    [HideInInspector]
    public int      GOLD;   // �÷��̾ ������ �ִ� ��ȭ
    [HideInInspector]
    public int      LV;     // �÷��̾� ����
}

public abstract class StatManager : MonoBehaviour
{
    private Stats       stats;                // ĳ���� ����

    public float HP
    {
        set => stats.HP = Mathf.Clamp(value, 0, MaxHP);
        get => stats.HP;
    }
    public int DC
    {
        set => stats.DC = Mathf.Clamp(value, 0, MaxDC);
        get => stats.DC;
    }
    public int GOLD
    {
        set => stats.GOLD = Mathf.Clamp(value, 0, MaxGOLD);
        get => stats.GOLD;
    }
    public int LV
    {
        set => stats.LV = Mathf.Clamp(value, 0, MaxLV);
        get => stats.LV;
    }




    public abstract float       MaxHP { get; }              // �ִ� ü��
    public abstract int         MaxDC { get; }              // �ִ� ��� ī��Ʈ
    public abstract int         MaxGOLD { get; }            // �ִ� ��� ī��Ʈ
    public abstract int         MaxLV{ get; }               // �ִ� ����
    
    public void Setup()
    {
        HP      = MaxHP;
        DC      = MaxDC;
        GOLD    = 500;
        LV      = 1;
    }
}
