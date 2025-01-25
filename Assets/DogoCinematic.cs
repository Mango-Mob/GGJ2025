using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogoCinematic : MonoBehaviour
{
    public Material[] DogTextures;
    public int index;
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial = DogTextures[index];
    }

    public void SwitchTexture()
    {
        index++;
        if (index >= DogTextures.Length)
            index = 0;

        GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial = DogTextures[index];
    }
}
