using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public GameSettings gameSettings;
    public TMP_Text scoreText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        gameSettings.score = 0;
        Messenger.AddListener(GameEvent.ENEMY_DEAD, AddPoints);
    }

    private void AddPoints()
    {
        gameSettings.score += (110 - 10 * gameSettings.difficultyLevel);
        scoreText.text = "Score: " + gameSettings.score;
    }
}
