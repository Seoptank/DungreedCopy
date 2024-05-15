using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ClickCheck : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pd = new PointerEventData(EventSystem.current);
            pd.position = Input.mousePosition;
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pd, raycastResults);

            if (raycastResults.Count > 0 )
            {
                Debug.Log("Click UI Elenmnt: " + raycastResults[0].gameObject.name);
                Debug.Log("Tag : " + raycastResults[0].gameObject.tag);
                Debug.Log("Layer : " + LayerMask.LayerToName(raycastResults[0].gameObject.layer));
            }
            else
            {
                Debug.Log("Nothing");
            }
        }
    }
}
