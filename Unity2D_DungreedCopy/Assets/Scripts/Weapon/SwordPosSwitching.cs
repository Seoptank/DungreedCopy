using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPosSwitching : MonoBehaviour
{
    [SerializeField]
    private bool SwordPos;
    [SerializeField]
    private GameObject SwordUpPos;
    [SerializeField]
    private GameObject SwordDownPos;

    public void SwordPosition()
    {
        SwordPos = !SwordPos;
        SwordUpPos.SetActive(SwordPos);
        SwordDownPos.SetActive(!SwordPos);
    }
}
