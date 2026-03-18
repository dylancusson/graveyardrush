using TMPro;
using UnityEngine;

public class SettingsPanel : MonoBehaviour
{
    public GameSettings gameSettings;
    public TMP_Text difficultyText;
    private bool isActive = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(isActive);
    }

    public void OnSettingsButton()
    {
        isActive = !isActive;
        gameObject.SetActive(isActive);
    }

    public void OnVolumeChanged(float volume)
    {
        gameSettings.musicVolume = volume;
        Debug.Log("Volume: " + gameSettings.musicVolume);
    }
    
    public void OnMouseSensChanged(float value)
    {
        gameSettings.mouseSensitivity = value;
        Debug.Log("Mouse Sens: " + gameSettings.mouseSensitivity);
    }
    
    public void OnDifficultyChanged(float difficulty)
    {
        gameSettings.difficultyLevel = difficulty;
        Debug.Log("Difficulty: " + gameSettings.difficultyLevel);
        switch (difficulty)
        {
            case 1:
                difficultyText.text = "Difficulty: Hard";
                break;
            case 2:
                difficultyText.text = "Difficulty: Normal";
                break;
            case 3:
                difficultyText.text = "Difficulty: Easy";
                break;
        }
    }
}
