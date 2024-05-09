using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TransformToBossDungeon : MonoBehaviour
{

    [SerializeField]
    private GameObject              spriteKey_F;
    [SerializeField]
    private string                  transferBossStageName;
    [SerializeField]
    private bool                    isActiveTransferKey = false;

    private KeyCode                 transferBossKey = KeyCode.F;

    private PlayerController        player;
    private FadeEffectController    fade;
    private MapController           map;
    public string[] sentences;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        fade   = FindObjectOfType<FadeEffectController>();
        map    = FindObjectOfType<MapController>();
    }
    private void Update()
    {
        spriteKey_F.SetActive(isActiveTransferKey);

        if (isActiveTransferKey)
        {
            if(Input.GetKeyDown(transferBossKey))
            {
                if(player.curSceneName == "Original")
                {
                    // 변경할 씬 이름으로 변경
                    player.curSceneName = transferBossStageName;

                    // dungeonName List정리
                    map.dungeonNames.Clear();

                    // 페이드아웃 효과
                    fade.OnFade(FadeState.FadeOut);

                    StartCoroutine(TranferBossStage());
                }
                else if(player.curSceneName == "SkelBoss")
                {
                    // 변경할 씬 이름으로 변경
                    player.curSceneName = transferBossStageName;

                    // dungeonName List정리
                    map.dungeonNames.Clear();

                    // 페이드아웃 효과
                    fade.OnFade(FadeState.FadeOut);

                    StartCoroutine(TranferEnding());
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Player")
        {
            isActiveTransferKey = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            isActiveTransferKey = false;
            //spriteKey_F.SetActive(isActiveTransferKey);
        }
    }
    private IEnumerator TranferBossStage()
    {
        yield return new WaitForSeconds(fade.fadeTime);
        SceneManager.LoadScene(transferBossStageName);
    }
    private IEnumerator TranferEnding()
    {
        yield return new WaitForSeconds(fade.fadeTime);
        DialogueManager.instance.OnEnding(sentences);
        yield return new WaitForSeconds(10f);
        DialogueManager.instance.endingDialogue.text = "";
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
