using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHands : MonoBehaviour
{
    [SerializeField]
    private GameObject laserPrefab;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void StartAttackAni()
    {
        anim.SetBool("IsAttack", true);
    }
    public void StopAttackAni()
    {
        anim.SetBool("IsAttack", false);
    }
    public void ActivateLaser()
    {
        laserPrefab.SetActive(true);
        AudioManager.Instance.PlaySFX("BossAtk");
    }
    public void DeactivateLaser()
    {
        laserPrefab.SetActive(false);
    }
}
