using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassingManager : MonoBehaviour
{
    public void OnPassing(int layer1,int layer2)
    {
        Physics2D.IgnoreLayerCollision(layer1, layer2, true);
    }

    public void OffPassing(int layer1, int layer2)
    {
        Physics2D.IgnoreLayerCollision(layer1, layer2, false);
    }
}
