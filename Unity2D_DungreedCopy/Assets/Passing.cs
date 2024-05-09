using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passing : MonoBehaviour
{
    int normalPlatform, passingPlatform;
 
    private void Start()
    {
        normalPlatform  = LayerMask.NameToLayer("Platform");
        passingPlatform = LayerMask.NameToLayer("PassingPlatform");    
    }

    public void OnPassing(int layer1, int layer2)
    {
        this.gameObject.layer = passingPlatform;
        Physics2D.IgnoreLayerCollision(layer1, layer2);
    }

    public void OffPassing(int layer1, int layer2)
    {
        this.gameObject.layer = normalPlatform;
        Physics2D.IgnoreLayerCollision(layer1, layer2 , false);
    }

    public IEnumerator PassingRoutain(int layer1, int layer2,float routainTime)
    {
        OnPassing(layer1, layer2);
        yield return new WaitForSeconds(routainTime);
        OffPassing(layer1, layer2);
    }
}
