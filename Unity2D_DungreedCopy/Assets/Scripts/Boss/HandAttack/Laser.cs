using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private GameObject laserPrefab;
    [SerializeField]
    private float       dis;
    private float       angle;
    
    public void DeactivateLaser()
    {
        laserPrefab.SetActive(false);
    }


}
