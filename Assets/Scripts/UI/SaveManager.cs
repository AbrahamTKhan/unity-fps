using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Audio;

[Serializable]
class PlayerData //Serializable class with the player's data
{
    public float oneEasy;
    public float oneMed;
    public float oneHard;
    public float twoEasy;
    public float twoMed;
    public float twoHard;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    public int difficulty;

    public float volumeVal;
    public float sensVal;
    public int isFullscreen;

    public float[,] bestTimes = new float[2,3];
    public float oneEasy;
    public float oneMed;
    public float oneHard;
    public float twoEasy;
    public float twoMed;
    public float twoHard;

    public AudioMixer mixer;
    private void Awake()
    {
        if (instance == null) //Retains this object in further scenes
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SetVolume(float value) //Updates the game's volume
    {
        volumeVal = value;
        mixer.SetFloat("MasterVolume", Mathf.Log10(volumeVal) * 20);
    }

    public void SetSensitivity(float value) //Updates the game's sensitivity
    {
        sensVal = value;
    }

    public float GetSensitivity()
    {
        return sensVal;
    }

    public void SetFullscreen(int value)
    {
        isFullscreen = value;
    }

    public void SetScore(int level, int difficulty, float time) //Checks if the submitted score is good enough to be saved
    {
        if (time < bestTimes[level, difficulty] || bestTimes[level, difficulty] == 0)
        {
            if (level == 0 && difficulty == 0) { oneEasy = time; bestTimes[level, difficulty] = time; } //Updates value & array if new personal best is set
            else if (level == 0 && difficulty == 1) { oneMed = time; bestTimes[level, difficulty] = time; }
            else if (level == 0 && difficulty == 2) { oneHard = time; bestTimes[level, difficulty] = time; }
            else if (level == 1 && difficulty == 0) { twoEasy = time; bestTimes[level, difficulty] = time; }
            else if (level == 1 && difficulty == 1) { twoMed = time; bestTimes[level, difficulty] = time; }
            else if (level == 1 && difficulty == 2) { twoHard = time; bestTimes[level, difficulty] = time; }
        }
        
    }
    public float[,] GetScores()
    {
        return bestTimes;
    }

    public void Save() //Saves the values to the file
    {
        Debug.Log("Save");
        string filename = Application.persistentDataPath + "/playInfo.dat";
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(filename, FileMode.OpenOrCreate); //Creates stream to file

        PlayerData pd = new PlayerData(); //Adds each value to new data object for storage

        PlayerPrefs.SetFloat("Volume",volumeVal);
        PlayerPrefs.SetFloat("Sensitivity", sensVal);
        PlayerPrefs.SetInt("Screen", isFullscreen);

        pd.oneEasy = oneEasy;
        pd.oneMed = oneMed;
        pd.oneHard = oneHard;
        pd.twoEasy = twoEasy;
        pd.twoMed = twoMed;
        pd.twoHard = twoHard;

        bf.Serialize(file, pd); //Writes object to file
        file.Close();
    }

    public void Load(Slider volSlider, Slider sensSlider, Toggle fullscreenToggle)
    {
        Debug.Log("Load");
        string filename = Application.persistentDataPath + "/playInfo.dat";
        if (File.Exists(filename))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(filename, FileMode.Open); //Opens stream to read from file

            PlayerData pd = (PlayerData) bf.Deserialize(file); //Creates object from file
            file.Close();

            if (PlayerPrefs.HasKey("Volume"))
            {
                volumeVal = PlayerPrefs.GetFloat("Volume"); //Assigns values from file object to script
            }
            else
            {
                volumeVal = 1;
            }
            volSlider.value = volumeVal;

            if (PlayerPrefs.HasKey("Sensitivity"))
            {
                sensVal = PlayerPrefs.GetFloat("Sensitivity");
            }
            else
            {
                sensVal = 2.5f;
            }
            sensSlider.value = sensVal;

            if (PlayerPrefs.HasKey("Screen"))
            {
                isFullscreen = PlayerPrefs.GetInt("Screen");
            }
            else
            {
                isFullscreen = 1;
            }
            
            fullscreenToggle.isOn = isFullscreen == 1 ? true : false;

            oneEasy = bestTimes[0, 0] = pd.oneEasy;
            oneMed = bestTimes[0, 1] = pd.oneMed;
            oneHard = bestTimes[0, 2] = pd.oneHard;
            twoEasy = bestTimes[1, 0] = pd.twoEasy;
            twoMed = bestTimes[1, 1] = pd.twoMed;
            twoHard = bestTimes[1, 2] = pd.twoHard;

            mixer.SetFloat("MasterVolume", Mathf.Log10(volumeVal) * 20);
        }
    }

    public void SetDifficulty(int value)
    {
        difficulty = value;
    }
    public int GetDifficulty()
    {
        return difficulty;
    }
}
