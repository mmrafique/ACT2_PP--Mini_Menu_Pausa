using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public AudioSettingsManager audioManager;

    public GameObject pausePanel; // asignar PausePanel
    public CanvasGroup canvasGroup; // CanvasGroup del pausePanel
    public float fadeDuration = 0.25f;
    bool isPaused = false;
    bool isFading = false;

    void Start()
    {
        pausePanel.SetActive(false);
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isFading) return;
        isPaused = !isPaused;
        if (isPaused) StartCoroutine(FadeIn());
        else StartCoroutine(FadeOut());
    }

    IEnumerator FadeIn()
    {
        isFading = true;
        pausePanel.SetActive(true);
        audioManager.RegisterButtonsForPanel(pausePanel);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        Time.timeScale = 0f; // pausa física
        isFading = false;
    }

    IEnumerator FadeOut()
    {
        isFading = true;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        pausePanel.SetActive(false);
        Time.timeScale = 1f; // reanuda física
        isFading = false;
    }

    // Llamar desde botón Resume
    public void OnResumeButton()
    {
        if (isPaused) TogglePause();
    }
}

