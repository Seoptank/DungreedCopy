using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSwordDeactivateEffect : MonoBehaviour
{
    private BossPattern boss;
    private void Awake()
    {
        boss = FindObjectOfType<BossPattern>();    
    }
}
