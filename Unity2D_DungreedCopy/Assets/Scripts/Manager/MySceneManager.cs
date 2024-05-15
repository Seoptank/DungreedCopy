using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    public static MySceneManager instance;

    public string curSceneName;

    private GameObject player;
    private GameObject mainCanvas;

    private void Awake()
    {
        #region ΩÃ±€≈Ê
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
        #endregion

        CheckScene();

        player      = GameObject.Find("Player");
        mainCanvas  = GameObject.Find("MainCanvas");
    }

    private void Update()
    {
        if(curSceneName != SceneManager.GetActiveScene().name)
        {
            CheckScene();
        }

        CurSceneIsStartScene();
    }

    private void CheckScene()
    {
        curSceneName = SceneManager.GetActiveScene().name;
    }

    public bool CurSceneIsStartScene()
    {
        if (curSceneName == "StartScene")
        {
            player.transform.position = Vector3.zero;
            player.SetActive(false);
            mainCanvas.SetActive(false);
            return true;
        }
        else
        {
            player.SetActive(true);
            mainCanvas.SetActive(true);
            return false;
        }
    }   
}
