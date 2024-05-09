using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Stats
{
    [HideInInspector]
    public float    HP;     // 플레이어 체력
    [HideInInspector]       
    public int      DC;     // 플레이어 대시 카운트
    [HideInInspector]
    public int      GOLD;   // 플레이어가 가지고 있는 재화
    [HideInInspector]
    public int      LV;     // 플레이어 레벨
}

public abstract class StatManager : MonoBehaviour
{
    private Stats       stats;                // 캐릭터 정보

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




    public abstract float       MaxHP { get; }              // 최대 체력
    public abstract int         MaxDC { get; }              // 최대 대시 카운트
    public abstract int         MaxGOLD { get; }            // 최대 대시 카운트
    public abstract int         MaxLV{ get; }               // 최대 레벨
    
    public void Setup()
    {
        HP      = MaxHP;
        DC      = MaxDC;
        GOLD    = 500;
        LV      = 1;
    }
}
