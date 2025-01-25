using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblePopper : MonoBehaviour
{
    public bool is_listening = true;
    // Update is called once per frame
    void Update()
    {
        if (!is_listening)
            return;

        if (InputManager.Instance.GetMousePress(MouseButton.LEFT))
        {
            RaycastHit hit;
            Ray ray = GetComponent<Camera>().ScreenPointToRay(InputManager.Instance.GetMousePositionInScreen());
            if(Physics.Raycast(ray, out hit)) 
            {
                if (hit.collider.tag == "Bubble") 
                    hit.collider.GetComponent<MenuBubble>().Pop(true);
            }
        }
    }
}
