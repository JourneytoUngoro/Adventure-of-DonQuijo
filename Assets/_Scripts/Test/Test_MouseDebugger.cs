using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Test_MouseDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 클릭 시
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            Debug.Log("=== UI Raycast Results ===");
            foreach (var result in results)
            {
                Debug.Log(result.gameObject.name);
            }
        }
    }
}
