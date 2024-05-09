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

    public PlayerData  curPlayer = new PlayerData(); // 데이터 생성

    public string   path;       // 파일 저장 경로
    public int      curSlot;    // 현재 슬롯 번호    

    private void Awake()
    {
        #region 싱글톤
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

        path = Application.persistentDataPath + "/save";    // 경로 지정
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
