using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public GameObject m_player;
    public Camera m_activeCamera;
    public bool IsInCombat = false;
    public float time;
    public float SpawnDelay;
    System.Random random;

    public AnimationCurve DensityScaleCurve;
    public GameObject bubble_prefab;
    public int score;

    public GameObject[] spawners;
    protected override void Awake()
    {
        base.Awake();
        random = random = new System.Random((int)System.DateTime.Now.Ticks);
    }
    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_activeCamera = Camera.main;
        spawners = GameObject.FindGameObjectsWithTag("Spawner");
    }

    private void Update()
    {
        if (!m_player.GetComponent<PlayerController>().isDead)
            time += Time.deltaTime;

        if (InputManager.Instance.IsKeyUp(KeyType.P))
            TogglePause();
    }

    private void OnLevelWasLoaded(int level)
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_activeCamera = Camera.main;
    }

    public GameObject SpawnBubble(Transform parent, Vector3 positon, Vector3 velocity, float density)
    {
        var obj = Instantiate(bubble_prefab, parent);
        obj.transform.position = positon;
        var bubble = obj.GetComponent<Bubble>();

        bubble.density = density;
        bubble.SetVelocity(velocity);
        bubble.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f) * GameManager.Instance.DensityScaleCurve.Evaluate(bubble.density);

        return obj;
    }

    public void TogglePause()
    {
        var pausables = FindObjectsOfType<PausableObject>();
        foreach (var pausable in pausables)
            pausable.TogglePause();
    }

    public List<Bubble> GetBubblesInRange(Vector3 location, float range )
    {
        var list = new List<Bubble>();

        foreach (var spawner in spawners)
        {
            foreach (var bubble in spawner.GetComponentsInChildren<Bubble>())
            {
                if (Extentions.CircleVsCircle(bubble.transform.position, location, bubble.transform.transform.localScale.x, range ))
                    list.Add(bubble);
            }
        }

        return list;
    }
    public void WakeDogs()
    {
        foreach (var spawner in spawners)
        {
            var comp = spawner.GetComponent<Spawner>();
            comp.PlayDogoCinematic();
        }
    }
}