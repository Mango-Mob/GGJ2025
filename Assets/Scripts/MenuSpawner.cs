using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSpawner : PausableObject
{
    public int min_angle = -30;
    public int max_angle = 60;
    public int steps = 10;

    public bool is_flipped = false;
    private Vector3[] cached_directions;

    public float safety_zone = 1.25f;
    public AnimationCurve time_till_next_bubble;
    public AnimationCurve DensityScaleCurve;
    public float time;
    public GameObject bubblePrefab;


    private float timer;
    // Start is called before the first frame update
    void Start()
    {
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
            var list = GetBubblesInRange(transform.position, safety_zone);
            if (list.Count > 0)
                return;

            Spawn();
        }
    }
    public List<Bubble> GetBubblesInRange(Vector3 location, float range)
    {
        var list = new List<Bubble>();

        foreach (var spawner in MenuBubble.global_list)
        {
            foreach (var bubble in spawner.GetComponentsInChildren<Bubble>())
            {
                if (Extentions.CircleVsCircle(bubble.transform.position, location, bubble.transform.transform.localScale.x, range))
                    list.Add(bubble);
            }
        }

        return list;
    }

    public void Spawn()
    {
        timer += time_till_next_bubble.Evaluate(time / 60.0f);

        var velocity = cached_directions[Random.Range(0, steps)] * Random.Range(2.5f, 3.0f);
        var density = Random.Range(0.0f, 1.0f);
        var obj = SpawnBubble(transform, transform.position, velocity, density);
        obj.GetComponent<MenuBubble>().PlaySpawnAnimation();
    }

    public GameObject SpawnBubble(Transform parent, Vector3 positon, Vector3 velocity, float density)
    {
        var obj = Instantiate(bubblePrefab, parent);
        obj.transform.position = positon;
        var bubble = obj.GetComponent<MenuBubble>();

        bubble.density = density;
        bubble.SetVelocity(velocity);
        bubble.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f) * DensityScaleCurve.Evaluate(bubble.density);

        return obj;
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
