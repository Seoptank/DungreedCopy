using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerData
{
    public int playerLV;
    public int playerCoin;
} 

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    public PlayerData  curPlayer = new PlayerData(); // ������ ����

    public string   path;       // ���� ���� ���
    public int      curSlot;    // ���� ���� ��ȣ    

    private void Awake()
    {
        #region �̱���
        if (instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(instance.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        #endregion

        path = Application.persistentDataPath + "/save";    // ��� ����
        print(path);
    }

    public void SaveData()
    {
        string jsonData = JsonUtility.ToJson(curPlayer);
        File.WriteAllText(path + curSlot.ToString(), jsonData);
    }

    public void LoadData()
    {
        string jsonData = File.ReadAllText(path + curSlot.ToString());
        curPlayer = JsonUtility.FromJson<PlayerData>(jsonData);
    }

    public void DataClear()
    {
        curSlot = -1;
        curPlayer = new PlayerData();
    }
}
