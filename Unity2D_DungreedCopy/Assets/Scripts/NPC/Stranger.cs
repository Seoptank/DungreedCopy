using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stranger : MonoBehaviour
{
    private void Update()
    {
        if (NPCManager.instance.meetKablovinaInDungeon)
        {
            StartCoroutine(DisapearStranger());
        }
        else return;
    }

    private IEnumerator DisapearStranger()
    {
        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);
    }
}
