using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    private void Update()
    {
        if(NPCManager.instance.meetKablovinaInDungeon)
        {
            this.gameObject.SetActive(true);
        }
        else this.gameObject.SetActive(false);
    }
}
