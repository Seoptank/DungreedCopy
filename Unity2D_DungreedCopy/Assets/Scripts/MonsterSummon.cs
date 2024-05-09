using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSummon : MonoBehaviour
{
    public GameObject Monster;
    void Start()
    {

    }

    void Update()
    {
        
    }

    public void Summon()
    {
        GameObject MonsterInstance = Instantiate(Monster, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
