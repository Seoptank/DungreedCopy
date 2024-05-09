using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkelSwing : MonoBehaviour
{
    public BoxCollider2D attackBoxCollider;
    public void EnableAttackCollider()
    {
        attackBoxCollider.enabled = true;
        AudioManager.Instance.PlaySFX("Bite");
    }
    public void DisableAttackCollider()
    {
        attackBoxCollider.enabled = false;
    }
}