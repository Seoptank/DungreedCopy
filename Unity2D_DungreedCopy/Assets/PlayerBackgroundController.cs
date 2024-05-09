using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBackgroundController : MonoBehaviour
{
    public static PlayerBackgroundController instance;

    [SerializeField]
    private GameObject      background;
    private PoolManager     backgroundPoolManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            backgroundPoolManager = new PoolManager(background);
        }
        else
        {
            Destroy(this);
        }

    }



}
