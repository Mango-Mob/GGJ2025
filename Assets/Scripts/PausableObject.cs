using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PausableObject : MonoBehaviour
{
    public static bool is_paused = false;

    public void TogglePause()
    {
        is_paused = !is_paused;
        
        if (is_paused)
            OnPause();
        else
            OnUnPause();
    }

    protected abstract void OnPause();
    protected abstract void OnUnPause();
}
