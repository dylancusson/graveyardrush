using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScreenController : MonoBehaviour
{
    public GameSettings gameSettings;
    public Image fadeImage;
    public float fadeSpeed = 1.5f;
    private static StartScreenController _instance;
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Messenger.AddListener(GameEvent.GAME_OVER, OnGameOver);
        }
        else if (_instance != this)
        {
            // If we reach here, this is a duplicate. 
            // We set it inactive immediately so buttons can't try to use it
            gameObject.SetActive(false); 
            Destroy(gameObject);
        }
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindFadeImage();
        StartCoroutine(FadeIn());
    }
    
    private void FindFadeImage()
    {
        // Look for the image tagged 'FadeCanvas' if the reference is missing
        if (fadeImage == null)
        {
            GameObject found = GameObject.FindWithTag("FadeCanvas");
            if (found != null) fadeImage = found.GetComponent<Image>();
        }
    }
    
    private void OnDestroy()
    {
        // Clean up the messenger to prevent errors when quitting the game
        if (_instance == this)
        {
            Messenger.RemoveListener(GameEvent.GAME_OVER, OnGameOver);
        }
    }

    IEnumerator FadeIn()
    {
        float alpha = 1;
        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }
    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    IEnumerator FadeOutAndLoad(string sceneName)
    {
        float alpha = 0;

        // Loop until the image is fully opaque
        while (alpha < 1)
        {
            alpha += Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null; // Wait for the next frame
        }

        // Load the actual scene
        SceneManager.LoadScene(sceneName);
    }
    
    public static void OnPlayButton()
    {
        if (_instance != null)
        {
            _instance.FadeToScene("Level1");
        }
    }

    public void OnGameOver()
    {
        FadeToScene("GameOver");
    }
}
