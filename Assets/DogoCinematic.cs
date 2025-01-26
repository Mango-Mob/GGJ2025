using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogoCinematic : MonoBehaviour
{
    public Material[] DogTextures;
    public int index;
    public Transform nose_bone;

    public MultiAudioAgent barks;
    public MultiAudioAgent taps;
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

    public void DestroyLocalBubbles()
    {
        foreach (var bubble in GameManager.Instance.GetBubblesInRange(nose_bone.position, 3))
        {
            bubble.GetComponent<Bubble>().Pop(false);
        }
    }

    public void Bark()
    {
        barks.PlayRandom(false, Random.Range(0.75f, 1.25f));
    }

    public void Footstep()
    {
        taps.PlayRandom(false, Random.Range(0.85f, 1.15f));
    }
}
