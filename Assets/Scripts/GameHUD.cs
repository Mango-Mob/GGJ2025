using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    public static int finalScore = 0;
    public static float finalTime = 0;
    public TMP_Text Score;
    public TMP_Text Timer;

    private int score;
    private float time;

    void OnStart()
    {
        finalScore = 0;
        finalTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        score = GameManager.Instance.score;
        Score.text = score.ToString("00000");
        time += Time.deltaTime;

        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);

        Timer.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    private void OnDestroy()
    {
        finalScore = score;
        finalTime = time;
    }
}
