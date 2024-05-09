using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField]
    private Slider      slider;
    [SerializeField]
    private Transform   target;


    private Camera cam;

    public void UpdateHPBar(float curValue, float maxValue)
    {
        slider.value = curValue / maxValue;
    }
}
