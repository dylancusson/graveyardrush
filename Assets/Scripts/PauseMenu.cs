using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] Button _pauseButton;
    [SerializeField] TMP_Text _pauseTitle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Messenger.AddListener(GameEvent.GAME_PAUSED, OnPause);
        Messenger.AddListener(GameEvent.GAME_UNPAUSED, OnUnPause);
    }

    void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.GAME_PAUSED, OnPause);
        Messenger.RemoveListener(GameEvent.GAME_UNPAUSED, OnUnPause);
    }

    void OnPause()
    {
        _pauseButton.gameObject.SetActive(true);
        _pauseButton.interactable = true;
        _pauseTitle.gameObject.SetActive(true);
    }

    void OnUnPause()
    {
        _pauseButton.gameObject.SetActive(false);
        _pauseButton.interactable = false;
        _pauseTitle.gameObject.SetActive(false);
    }
}
