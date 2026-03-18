using TMPro;
using UnityEngine;

public class YourScore : MonoBehaviour
{
    public GameSettings gameSettings;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TMP_Text scoreText = GetComponent<TMP_Text>();
        scoreText.text = "Your Score: " + gameSettings.score;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
