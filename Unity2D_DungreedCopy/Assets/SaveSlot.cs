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
    private Sprite[]            sprites;                    // 0�� => origin, 1�� => selected 
    private bool[]              saveFile = new bool[3];     // Save���� ���� ���� ����
    private TextMeshProUGUI[]   textLevel;
    private Image[]             images;

    private void Awake()
    {
        // ���Ժ��� ������ ���� �Ǵ�
        for (int i = 0; i < saveFile.Length; ++i)
        {
            // �����Ͱ� ���� 
            if(File.Exists(DataManager.instance.path + $"{i}")) 
            {
                saveFile[i] = true;

                DataManager.instance.curSlot = i;   // �ش� ���Կ� ��ȣ ����
                DataManager.instance.LoadData();    // �ش� ���� ������ �ҷ���
                textLevel[i].text = "LV " + DataManager.instance.curPlayer.playerLV;
            }
            else 
                textLevel[i].text = "�������";
        }
    }

    public void Slot(int num)
    {
        DataManager.instance.curSlot = num;

        // ���� Slot ������ �����ϸ�
        if(saveFile[num])
        {
            DataManager.instance.LoadData();    // ������ �ε�

            // ���� �� �̵� �Լ� ����
            ChangeScaneInGame();
        }
        else
        {

        }
    }

    private void ChangeScaneInGame()
    {
        // ���� ���Կ� ������ ������?
        if (!saveFile[DataManager.instance.curSlot])
        {
            DataManager.instance.SaveData(); // ���� ������ ������.

            // �� �̵�
        }

    }
}
