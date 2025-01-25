using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    enum State { Menu, Settings} 
    
    State current_state = State.Menu;

    public GameObject MenuTab;
    public GameObject SettingsTab;

    public Slider MasterSlider;
    public Slider MusicSlider;
    public Slider SoundEffectSlider;

    private void Start()
    {
        InputManager.Instance.isCursorVisible = true;
        InputManager.Instance.cursorMode = CursorLockMode.Confined;

        MasterSlider.value = AudioManager.Instance.volumes[(int)AudioManager.VolumeChannel.MASTER];
        MusicSlider.value = AudioManager.Instance.volumes[(int)AudioManager.VolumeChannel.MUSIC];
        SoundEffectSlider.value = AudioManager.Instance.volumes[(int)AudioManager.VolumeChannel.SOUND_EFFECT];
    }
    // Update is called once per frame
    void Update()
    {
        MenuTab.SetActive(current_state == State.Menu);
        SettingsTab.SetActive(current_state == State.Settings);

        if(current_state == State.Settings)
        {
            AudioManager.Instance.volumes[(int)AudioManager.VolumeChannel.MASTER] = MasterSlider.value;
            AudioManager.Instance.volumes[(int)AudioManager.VolumeChannel.MUSIC] = MusicSlider.value;
            AudioManager.Instance.volumes[(int)AudioManager.VolumeChannel.SOUND_EFFECT] = SoundEffectSlider.value;
        }
    }

    public void PlayGame()
    {
        LevelManager.Instance.LoadNewLevel("GameScene");
    }

    public void ShowSettings()
    {
        current_state = State.Settings;
    }

    public void GoBackToMenu()
    {
        current_state = State.Menu;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
