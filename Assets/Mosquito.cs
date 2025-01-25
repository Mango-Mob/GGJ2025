using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mosquito : PausableObject
{
    public float dist;
    public float min_speed, max_speed;
    public bool is_facing_right = true;

    public float min_delay, max_delay;
    public GameObject AlarmPrefab;
    private GameObject CreatedAlarm;
    private float timer = 0.0f;
    private float speed = 0.0f;

    private float right_edge, left_edge;
    // Start is called before the first frame update
    void Start()
    {
        left_edge = is_facing_right ? transform.position.x : (transform.position + transform.right * dist).x;
        right_edge = !is_facing_right ? transform.position.x : (transform.position + transform.right * dist).x;

        timer = Random.Range(min_delay, max_delay);
        speed = Random.Range(min_speed, max_speed);
    }

    // Update is called once per frame
    void Update()
    {
        if (is_paused)
            return;

        if (timer > 0.0f)
        {
            timer -= Time.deltaTime;
            GetComponentInChildren<Collider>().enabled = timer <= 0.0f;

            if (!CreatedAlarm && timer <= min_delay * 1.5f)
            {
                CreatedAlarm = GameObject.Instantiate(AlarmPrefab, transform);
                CreatedAlarm.transform.position = transform.position + (is_facing_right ? -transform.right : transform.right);
            }
            return;
        }
        else if (CreatedAlarm)
            Destroy(CreatedAlarm);

        var side = is_facing_right ? -1 : 1;
        transform.position += side * transform.right * speed * Time.deltaTime;

        var edge = transform.position + transform.right * dist;
        if (is_facing_right && transform.position.x <= right_edge)
        {
            is_facing_right = false;
            timer = Random.Range(min_delay, max_delay);
            speed = Random.Range(min_speed, max_speed);
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (!is_facing_right && transform.position.x >= left_edge )
        {
            is_facing_right = true;
            timer = Random.Range(min_delay, max_delay);
            speed = Random.Range(min_speed, max_speed);
            transform.localScale = new Vector3(1, 1, 1);
        }
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
