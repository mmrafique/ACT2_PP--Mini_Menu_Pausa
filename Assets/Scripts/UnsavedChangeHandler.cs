using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UnsavedChangesHandler : MonoBehaviour
{
    public GameObject unsavedModal; // panel modal
    public Button yesButton;

    public AudioSettingsManager audioManager; 
    public Button noButton;
    public PauseMenu pauseMenu;

    bool hasUnsavedChanges = false;

    void Start()
    {
        unsavedModal.SetActive(false);
        yesButton.onClick.AddListener(OnYes);
        noButton.onClick.AddListener(OnNo);
    }

    public void SetUnsaved(bool b)
    {
        hasUnsavedChanges = b;
    }

    // Llamar desde botón X (close) del PausePanel
    public void TryClosePause()
    {
        if (hasUnsavedChanges)
        {
            unsavedModal.SetActive(true);
        }
        else
        {
            pauseMenu.TogglePause();
        }
    }

    void OnYes()
    {
        // descartar cambios y cerrar menú
        if (audioManager != null) audioManager.PlayUIClick();
        hasUnsavedChanges = false;
        unsavedModal.SetActive(false);
        pauseMenu.TogglePause();
    }

    void OnNo()
    {
        // solo cerrar modal, volver al menú
        if (audioManager != null) audioManager.PlayUIClick();
        unsavedModal.SetActive(false);
    }
}
