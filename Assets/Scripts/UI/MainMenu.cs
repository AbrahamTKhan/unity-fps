using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    private int level;
    private SaveManager saveManager;
    private FaderScript fader;
    public Slider volumeSlider;
    public Slider sensitivitySlider;
    public Toggle fullscreenToggle;
    private int difficulty;
    public Text easyUI;
    public Text mediumUI;
    public Text hardUI;
    private void Start()
    {
        fader = GameObject.FindGameObjectWithTag("Fader").GetComponent<FaderScript>();
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        saveManager = GameObject.FindGameObjectWithTag("SaveManager").GetComponent<SaveManager>();
        saveManager.Load(volumeSlider, sensitivitySlider, fullscreenToggle);
    }

    public void SetLevel(int value)
    {
        level = value;
    }

    public void PlayLevel() //Starts the fade corouttine when a new level is loaded
    {
        fader.StartCoroutine(fader.FadeOnce(level));
    }

    public void SetScores() //Sets the personal best scores on the menu
    {
        float[,] scores = saveManager.GetScores();
        if (scores[level-2, 0] > 0) //If stored time <= 0 = never completed
        {
            easyUI.text = "Best Time: " + scores[level-2, 0];
        }
        else
        {
            easyUI.text = "Best Time: Not Completed";
        }
        if (scores[level-2, 1] > 0)
        {
            mediumUI.text = "Best Time: " + scores[level - 2, 1];
        }
        else
        {
            mediumUI.text = "Best Time: Not Completed";
        }
        if (scores[level-2, 2] > 0)
        {
            hardUI.text = "Best Time: " + scores[level - 2, 2];
        }
        else
        {
            hardUI.text = "Best Time: Not Completed";
        }
    }

    public void CallSave()
    {
        saveManager.Save();
    }

    public void SetVolume()
    {
        saveManager.SetVolume(volumeSlider.value);
    }

    public void SetSensitivity()
    {
        saveManager.SetSensitivity(sensitivitySlider.value);
    }

    public void SetFullscreen()
    {
        int value = fullscreenToggle.isOn ? 1 : 0;
        saveManager.SetFullscreen(value);
    }

    public void SetDifficulty(int value)
    {
        difficulty = value;
        saveManager.SetDifficulty(value);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void UpdateScreenMode() //Updates the screen setting in the options menu
    {
        if (fullscreenToggle.isOn)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }

    public void SetTimeScale()
    {
        Time.timeScale = 1;
    }
}
