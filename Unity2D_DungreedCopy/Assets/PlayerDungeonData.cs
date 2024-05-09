using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerDungeonData : MonoBehaviour
{
    static public PlayerDungeonData instance;
    
    public int      countKill = 0;
    public float    enterTime;              // 던전 시작시간 기록
    public float    deathTime;              // 플레이어 죽은 시간 기록
    public float    totalTime;              // 플레이어가 살아남은 시간
    public bool     isMoving = false;       // 플레이어가 Pos간 이동을 하고있는지
    public bool     isFighting = false;     // 플레이어가 전투를 하고있는지
    public float    totalEXP;               // 던전 내에서 받은 최종 EXP

    private Rigidbody2D     rigid;
    private Movement2D      movement;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            rigid       = GetComponent<Rigidbody2D>();
            movement    = GetComponent<Movement2D>();
        }
        else
        {
            Destroy(this);
        }
    }

    private void FixedUpdate()
    {
        if (isMoving && !movement.isGrounded)
            rigid.velocity = new Vector3(0, rigid.velocity.y);
        else if (isMoving && movement.isGrounded)
            rigid.velocity = Vector3.zero;
    }

    public void ResetDungeonData()
    {
        countKill = 0;
        enterTime = 0f;
        deathTime = 0f;
        totalTime = 0f;
        totalEXP = 0;
    }
    public void TimeChangeToText(TextMeshProUGUI textUI)
    {
        textUI.text = FormatTime(totalTime);
    }

    private string FormatTime(float seconds)
    {
        int h = (int)(seconds / 3600);
        int m = (int)(seconds % 3600) / 60;
        int s = (int)(seconds % 60);

        return string.Format("{0:D2}h {1:D2}m {2:D2}s", h, m, s);
    }
}
