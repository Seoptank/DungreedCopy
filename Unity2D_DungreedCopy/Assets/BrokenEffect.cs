using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenEffect : MonoBehaviour
{
    public GameObject[] debrisPrefabs; // ���� ������Ʈ �����յ� �迭
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

        // �μ��� ������Ʈ ��Ȱ��ȭ
        gameObject.SetActive(false);

        // �� ���� ������Ʈ�� �����Ͽ� Ȱ��ȭ
        foreach (GameObject debrisPrefab in debrisPrefabs)
        {
            GameObject debris = Instantiate(debrisPrefab, transform.position, transform.rotation);
            debris.SetActive(true);
        }
    }
}
