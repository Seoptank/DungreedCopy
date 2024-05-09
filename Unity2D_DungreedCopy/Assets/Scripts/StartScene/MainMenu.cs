using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnClickNewGame()
    {
        FadeEffectController.instance.OnFade(FadeState.FadeInOut);

        StartCoroutine(ChangeScene());

    }

    private IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(FadeEffectController.instance.fadeTime);
        SceneManager.LoadScene("Scene(Yuseop)");
    }

    public void OnclickOption()
    {

    }

    public void OnClickExit()
    {
        SceneManager.LoadScene("StartScene");        
    }

    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
