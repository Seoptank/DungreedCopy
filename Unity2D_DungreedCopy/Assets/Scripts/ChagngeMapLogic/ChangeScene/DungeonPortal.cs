using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonPortal : MonoBehaviour
{


    public bool                     eatPlayer = false;
    public string                   tranferMapName;   // 이동할 맵의 이름

    private PoolManager             poolManager;
    private PlayerController        player;
    private DungeonPortalController dungeonPortalController;

    private void Awake()
    {
        player                  = FindObjectOfType<PlayerController>();
        dungeonPortalController = FindObjectOfType<DungeonPortalController>();
    }
    public void Setup(PoolManager poolManager)
    {
        this.poolManager = poolManager;
        AudioManager.Instance.PlaySFX("OpenPortal");
    }

    public void ThePortalEatPlayer()
    {
        eatPlayer = true;
        AudioManager.Instance.PlaySFX("ClosePortal");

        PlayerController.instance.spriteRenderer.color = new Color(1, 1, 1, 0);
        PlayerController.instance.weaponRenderer.color = new Color(1, 1, 1, 0);
    }
    public void FalseToEatPlayer()
    {
        eatPlayer = false;

        player.curSceneName = tranferMapName;

        FadeEffectController.instance.OnFade(FadeState.FadeOut);

        // 던전에 들어가는 시간 기록
        PlayerDungeonData.instance.enterTime = Time.time;
        Debug.Log(PlayerDungeonData.instance.enterTime);

        StartCoroutine(ChangeScene());
    }
    private IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(FadeEffectController.instance.fadeTime);
        poolManager.DeactivePoolItem(gameObject);
        SceneManager.LoadScene(tranferMapName);
    }
}
