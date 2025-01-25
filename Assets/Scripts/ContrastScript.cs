using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContrastScript : MonoBehaviour
{
    public static float BubbleAlpha = 0.0f;

    private SpriteRenderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        Color col = renderer.color;
        col.a = BubbleAlpha;
        renderer.color = col;
    }

    // Update is called once per frame
    void Update()
    {
        Color col = renderer.color;
        col.a = BubbleAlpha;
        renderer.color = col;
    }
}
