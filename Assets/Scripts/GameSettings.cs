using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/GameSettings")]
public class GameSettings : ScriptableObject
{
    public float musicVolume;
    public float difficultyLevel;
    public float mouseSensitivity;
    public float highScore;
    public float score = 0;
}
