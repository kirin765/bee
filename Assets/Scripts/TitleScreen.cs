using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    [Header("Behavior")]
    [SerializeField] private bool pauseTimeOnShow = true;

    [Header("Optional References")]
    [SerializeField] private Canvas existingCanvas;
    [SerializeField] private GameObject rootOverride;
    [SerializeField] private Button startButtonOverride;

    private void Awake()
    {
        // BuildIfNeeded();
        Show();
    }

    private void Show()
    {
        if (rootOverride != null) rootOverride.SetActive(true);
        if (pauseTimeOnShow) Time.timeScale = 0f;
    }

    public void HandleStart()
    {
        if (pauseTimeOnShow) Time.timeScale = 1f;
        if (rootOverride != null) rootOverride.SetActive(false);
    }
}
