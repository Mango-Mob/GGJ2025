using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : PausableObject
{
    public int min_angle = -30;
    public int max_angle = 60;
    public int steps = 10;

    public bool is_flipped = false;
    private Vector3[] cached_directions;

    public float safety_zone = 1.25f;
    public AnimationCurve time_till_next_bubble;
    public AnimationCurve min_density;
    public AnimationCurve max_density;
    public float time;
    public Animator dog_animator;

    public GameObject player;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
        cached_directions = GetDirections();
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (is_paused) return;

        time += Time.deltaTime;
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            if (Extentions.CircleVsCircle(player.transform.position, transform.position, safety_zone, 1))
                return;

            var list = GameManager.Instance.GetBubblesInRange(transform.position, safety_zone);
            if (list.Count > 0)
                return;

            Spawn();
        }
    }

    public void Spawn()
    {
        timer += time_till_next_bubble.Evaluate(time / 60.0f);

        var velocity = cached_directions[Random.Range(0, steps)] * Random.Range(2.5f, 3.0f);
        var density = Random.Range(min_density.Evaluate(time / 60.0f), min_density.Evaluate(time / 60.0f));
        var obj = GameManager.Instance.SpawnBubble(transform, transform.position, velocity, density);
        obj.GetComponent<Bubble>().PlaySpawnAnimation();
    }

    public void Flip()
    {
        is_flipped = !is_flipped;
        cached_directions = GetDirections();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.1f);
        Gizmos.DrawWireSphere(transform.position, safety_zone);

        foreach (var direction in GetDirections())
            Gizmos.DrawLine(transform.position, transform.position + direction * 10);
    }

    private Vector3[] GetDirections()
    {
        Vector3[] directions = new Vector3[steps];
        var min = is_flipped ? -max_angle : min_angle;
        var max = is_flipped ? -min_angle : max_angle;

        for (int i = 0; i < steps; i++)
        {
            int angle = min + ((max - min) / steps) * i;
            directions[i] = Quaternion.Euler(0, 0, angle) * new Vector3(0, 1, 0);
        }
        return directions;
    }

    protected override void OnPause()
    {
        
    }

    protected override void OnUnPause()
    {
        
    }
}
