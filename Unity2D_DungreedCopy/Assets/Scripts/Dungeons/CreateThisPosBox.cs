using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateThisPosBox : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;

    private void Awake()
    {
        Vector2 instancePos = new Vector3(transform.position.x, transform.position.y + 1f);

        Instantiate(prefab, instancePos, transform.rotation, GameObject.Find("Boxes").transform);
        Destroy(this.gameObject);
    }
}
