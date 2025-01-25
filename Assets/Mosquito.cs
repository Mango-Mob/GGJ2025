using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mosquito : PausableObject
{
    public float dist;
    public float speed;
    public bool is_facing_right = true;

    private float right_edge, left_edge;
    // Start is called before the first frame update
    void Start()
    {
        left_edge = is_facing_right ? transform.position.x : (transform.position + transform.right * dist).x;
        right_edge = !is_facing_right ? transform.position.x : (transform.position + transform.right * dist).x;
    }

    // Update is called once per frame
    void Update()
    {
        if (is_paused)
            return;

        var side = is_facing_right ? -1 : 1;
        transform.position += side * transform.right * speed * Time.deltaTime;

        var edge = transform.position + transform.right * dist;
        if (is_facing_right && transform.position.x <= right_edge)
            is_facing_right = false;
        else if (!is_facing_right && transform.position.x >= left_edge )
             is_facing_right = true;

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * dist);
        Gizmos.DrawSphere(transform.position + transform.right * dist, 0.25f);
    }

    protected override void OnPause()
    {
        
    }

    protected override void OnUnPause()
    {
        
    }
}
