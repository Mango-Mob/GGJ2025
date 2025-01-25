using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblePopper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance.GetMousePress(MouseButton.LEFT))
        {
            RaycastHit hit;
            Ray ray = GetComponent<Camera>().ScreenPointToRay(InputManager.Instance.GetMousePositionInScreen());
            if(Physics.Raycast(ray, out hit)) 
            {
                if (hit.collider.tag == "Bubble") 
                    hit.collider.GetComponent<MenuBubble>().Pop();
            }
        }
    }
}
