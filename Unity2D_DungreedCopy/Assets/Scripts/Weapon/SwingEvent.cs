using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingEvent : MonoBehaviour
{

    private PoolManager pool;

    public void Setup()
    {

    }
    public void DestroySwing()
    {
        Destroy(this.gameObject);
    }
}
