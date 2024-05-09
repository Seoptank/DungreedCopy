using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTresureBox : MonoBehaviour
{
    [SerializeField]
    private GameObject prefabTresureBox;

    private PoolManager tresureBoxPoolManager;
    private PoolManager poolManager;

    public void Setup(PoolManager pool)
    {
        this.poolManager = pool;
        tresureBoxPoolManager = new PoolManager(prefabTresureBox);
    }
    private void OnApplicationQuit()
    {
        tresureBoxPoolManager.DestroyObjcts();
    }

    private void Deactivate()
    {
        poolManager.DeactivePoolItem(this.gameObject);
    }

    public void CreateBox()
    {
        GameObject box = tresureBoxPoolManager.ActivePoolItem();
        box.transform.position = transform.position;
        box.transform.rotation = transform.rotation;
        box.GetComponent<BoxPool>().Setup(tresureBoxPoolManager);
        Deactivate();
        //Destroy(this.gameObject);
    }
}
