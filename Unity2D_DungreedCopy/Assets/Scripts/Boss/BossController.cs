using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer[]    spritesBoss;
    [SerializeField]
    private SpriteRenderer      spriteBossBack;     
    [SerializeField]
    private float               camSmoothMoveTime;
    [SerializeField]
    private Transform           camViewPos;
    public bool                 isAbleToAttack = false;

    private UIEffectManager         uiEffectManager;
    private MainCameraController    mainCam;
    private PlayerController        player;
    private void Awake()
    {
        mainCam             = FindObjectOfType<MainCameraController>();
        uiEffectManager     = FindObjectOfType<UIEffectManager>();
        player              = FindObjectOfType<PlayerController>();
    }

    private void Start()
    {
        player.playerMeetsBoss = true;
        PlayerDungeonData.instance.isFighting = true;

        StartCoroutine(FadeInBossSprite());

    }

    public void OnDie()
    {
        for (int i = 0; i < spritesBoss.Length; ++i)
        {
            spritesBoss[i].color = new Color(1, 1, 1, 0);
        }
    }

    private IEnumerator FadeInBossSprite()
    {
        StartCoroutine(mainCam.ChangeView(camViewPos, camSmoothMoveTime));
        StartCoroutine(GameObject.FindObjectOfType<UIBossIntroduce>().OnIntroduceBoss(0, 1));

        StartCoroutine(uiEffectManager.UIFade(spriteBossBack, 0, 1));

        for (int i = 0; i < spritesBoss.Length; ++i)
        {
            StartCoroutine(uiEffectManager.UIFade(spritesBoss[i], 0, 1));
            yield return new WaitForSeconds(1f);

            if(i == spritesBoss.Length-1)
            {
                AudioManager.Instance.PlaySFX("Boss");
                yield return new WaitForSeconds(2);
                player.playerMeetsBoss = false;
                isAbleToAttack = true;
                PlayerDungeonData.instance.isFighting = false;
                StartCoroutine(GameObject.FindObjectOfType<UIBossIntroduce>().OffIntroduceBoss(1, 0));
            }
        }
    }
}
