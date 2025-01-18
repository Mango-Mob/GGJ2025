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
    public String time_desplay;
    public float SpawnDelay;
    System.Random random;

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
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "MenuScene")
            return;
    }

    private void OnLevelWasLoaded(int level)
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_activeCamera = Camera.main;
    }
}
