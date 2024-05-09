using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenEffect : MonoBehaviour
{
    public GameObject[] debrisPrefabs; // 파편 오브젝트 프리팹들 배열
    private bool isBroken = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isBroken)
        {
            BreakObject();
        }
    }

    void BreakObject()
    {
        isBroken = true;

        // 부서진 오브젝트 비활성화
        gameObject.SetActive(false);

        // 각 파편 오브젝트를 생성하여 활성화
        foreach (GameObject debrisPrefab in debrisPrefabs)
        {
            GameObject debris = Instantiate(debrisPrefab, transform.position, transform.rotation);
            debris.SetActive(true);
        }
    }
}
