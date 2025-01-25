using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class EndScene : MonoBehaviour
{
    public TMP_Text Score;
    public TMP_Text Timer;

    // Start is called before the first frame update
    void Start()
    {
        Score.text = GameHUD.finalScore.ToString("00000");
        int minutes = (int)(GameHUD.finalTime / 60);
        int seconds = (int)(GameHUD.finalTime % 60);

        Timer.text = minutes.ToString("00") + ":" + seconds.ToString("00");
        InputManager.Instance.isCursorVisible = true;
        InputManager.Instance.cursorMode = CursorLockMode.Confined;
    }

    public void PlayAgain()
    {
        LevelManager.Instance.LoadNewLevel("GameScene");
    }

    public void GoToMenu()
    {
        LevelManager.Instance.LoadNewLevel("MenuScene");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
