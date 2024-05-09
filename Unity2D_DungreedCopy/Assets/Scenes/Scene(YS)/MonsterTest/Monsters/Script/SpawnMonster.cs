using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMonster : MonoBehaviour
{
    [SerializeField]
    private GameObject  prefabSpawn;
    [SerializeField]
    private int         monNum;
    private Transform   transformParent;
    // 0: BigBat
    // 1: Banshee
    // 2: LittleGhost
    // 3: RedBigBat
    // 4: RedBat
    // 5: BigWhiteSkel
    // 6: Minotaur
    // 7: BowSkel
    // 8: SwordSkel
    // 9: SkelDog

    private PoolManager spawnPool;

    private void Awake()
    {
        transformParent = transform.parent;
        spawnPool = new PoolManager(prefabSpawn);
    }

    private void OnEnable()
    {
        AvtivateSpawnPrefab(monNum);
    }

    private void OnApplicationQuit()
    {
        spawnPool.DestroyObjcts();
    }

    private void AvtivateSpawnPrefab(int num)
    {
        GameObject spawn = spawnPool.ActivePoolItem();
        if(transformParent != null) spawn.transform.parent = transformParent;
        spawn.transform.position = transform.position;
        spawn.transform.rotation = transform.rotation;
        spawn.GetComponent<MonsterFactory>().Setup(monNum, spawnPool,transformParent);
    }
}
