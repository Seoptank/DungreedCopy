using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDiePiece : MonoBehaviour
{
    [SerializeField]
    private GameObject  explosionEffectPrefab;

    private void Awake()
    {
        CircleShot();
    }

    private void Update()
    {
        if(Time.timeScale <= 1)
        {
            Time.timeScale += Time.deltaTime;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    private void CircleShot()
    {
        for (int i = 0; i < 360; i += 36)
        {
            GameObject temp = Instantiate(explosionEffectPrefab);

            temp.transform.position = transform.position;

            temp.transform.rotation = Quaternion.Euler(0, 0, i);
        }
    }

}
