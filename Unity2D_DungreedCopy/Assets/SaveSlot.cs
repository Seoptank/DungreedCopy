using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class SaveSlot : MonoBehaviour
{
    //private int          slotNum;
    [SerializeField]
    private Sprite[]            sprites;                    // 0번 => origin, 1번 => selected 
    private bool[]              saveFile = new bool[3];     // Save파일 존재 유무 저장
    private TextMeshProUGUI[]   textLevel;
    private Image[]             images;

    private void Awake()
    {
        // 슬롯별로 데이터 유무 판단
        for (int i = 0; i < saveFile.Length; ++i)
        {
            // 데이터가 존재 
            if(File.Exists(DataManager.instance.path + $"{i}")) 
            {
                saveFile[i] = true;

                DataManager.instance.curSlot = i;   // 해당 슬롯에 번호 저장
                DataManager.instance.LoadData();    // 해당 슬롯 데이터 불러옴
                textLevel[i].text = "LV " + DataManager.instance.curPlayer.playerLV;
            }
            else 
                textLevel[i].text = "비어있음";
        }
    }

    public void Slot(int num)
    {
        DataManager.instance.curSlot = num;

        // 현재 Slot 데이터 존재하면
        if(saveFile[num])
        {
            DataManager.instance.LoadData();    // 데이터 로드

            // 게임 씬 이동 함수 실행
            ChangeScaneInGame();
        }
        else
        {

        }
    }

    private void ChangeScaneInGame()
    {
        // 현재 슬롯에 데이터 없으면?
        if (!saveFile[DataManager.instance.curSlot])
        {
            DataManager.instance.SaveData(); // 현재 정보를 저장함.

            // 씬 이동
        }

    }
}
