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
    public Slider ContrastSlider;

    public BubblePopper popper;
    private void Start()
    {
        InputManager.Instance.isCursorVisible = true;
        InputManager.Instance.cursorMode = CursorLockMode.Confined;

        MasterSlider.value = AudioManager.Instance.volumes[(int)AudioManager.VolumeChannel.MASTER];
        MusicSlider.value = AudioManager.Instance.volumes[(int)AudioManager.VolumeChannel.MUSIC];
        SoundEffectSlider.value = AudioManager.Instance.volumes[(int)AudioManager.VolumeChannel.SOUND_EFFECT];
        ContrastScript.BubbleAlpha = PlayerPrefs.GetFloat($"contrast");
        ContrastSlider.value = ContrastScript.BubbleAlpha;
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
            ContrastScript.BubbleAlpha = ContrastSlider.value;
        }
    }
    public void OnDestroy()
    {
        PlayerPrefs.SetFloat($"contrast", ContrastScript.BubbleAlpha);
    }
    public void TogglePause()
    {
        var pausables = FindObjectsOfType<PausableObject>();
        foreach (var pausable in pausables)
            pausable.TogglePause();
    }

    public void PlayGame()
    {
        LevelManager.Instance.LoadNewLevel("GameScene");
        GetComponent<SoloAudioAgent>().PlayWithRandomPitch();
    }

    public void ShowSettings()
    {
        current_state = State.Settings;
        GetComponent<SoloAudioAgent>().PlayWithRandomPitch();
        if (popper)
            popper.is_listening = false;
        TogglePause();
    }

    public void GoBackToMenu()
    {
        current_state = State.Menu;
        GetComponent<SoloAudioAgent>().PlayWithRandomPitch();
        if (popper)
            popper.is_listening = true;
        TogglePause();
    }
    public void OpenTeamPage()
    {
        Application.OpenURL("https://mangomob.itch.io/");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
