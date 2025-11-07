using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    public UnsavedChangesHandler unsavedHandler;

    [Header("UI")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Text musicValueText;
    public Text sfxValueText;

    public Button playSfxButton;   // test SFX

    float tempMusic = 1f;
    float tempSfx = 1f;

    const string MUSIC_PREF = "pref_music_volume";
    const string SFX_PREF = "pref_sfx_volume";

    bool musicPaused = false;

    void Start()
    {
        tempMusic = PlayerPrefs.GetFloat(MUSIC_PREF, 0.8f);
        tempSfx = PlayerPrefs.GetFloat(SFX_PREF, 0.8f);

        musicSlider.value = tempMusic;
        sfxSlider.value = tempSfx;
        UpdateMusicVolumeUI(tempMusic);
        UpdateSfxVolumeUI(tempSfx);

        musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);

        playSfxButton.onClick.AddListener(PlaySfxTest);

        ApplyAudioVolumes(tempMusic, tempSfx);

        if (musicSource.clip != null)
        {
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    // SOLO usar para TODOS los botones del menú, EXCEPTO playMusic / playSfx
    public void RegisterButtonsForPanel(GameObject panel)
    {
        Button[] b = panel.GetComponentsInChildren<Button>(true);
        foreach (Button bt in b)
        {
            if (bt != playSfxButton)
                bt.onClick.AddListener(PlayUIClick);
        }
    }

    void OnMusicSliderChanged(float v)
    {
        tempMusic = v;
        UpdateMusicVolumeUI(v);
        ApplyMusicVolume(v);
        if (unsavedHandler) unsavedHandler.SetUnsaved(true);
    }

    void OnSfxSliderChanged(float v)
    {
        tempSfx = v;
        UpdateSfxVolumeUI(v);
        ApplySfxVolume(v);
        if (unsavedHandler) unsavedHandler.SetUnsaved(true);
    }

    void UpdateMusicVolumeUI(float v) => musicValueText.text = Mathf.RoundToInt(v * 100) + "%";
    void UpdateSfxVolumeUI(float v) => sfxValueText.text = Mathf.RoundToInt(v * 100) + "%";

    void ApplyMusicVolume(float v) { musicSource.volume = v; }
    void ApplySfxVolume(float v) { sfxSource.volume = v; }

    void ApplyAudioVolumes(float music, float sfx)
    {
        ApplyMusicVolume(music);
        ApplySfxVolume(sfx);
    }

    public void PlaySfxTest()
    {
        sfxSource.PlayOneShot(sfxSource.clip);
    }

    public void PlayUIClick()
    {
        sfxSource.PlayOneShot(sfxSource.clip);
    }

    public void OnApply()
    {
        PlayerPrefs.SetFloat(MUSIC_PREF, tempMusic);
        PlayerPrefs.SetFloat(SFX_PREF, tempSfx);
        PlayerPrefs.Save();
        if (unsavedHandler) unsavedHandler.SetUnsaved(false);
    }

    public void OnDefault()
    {
        tempMusic = 0.8f;
        tempSfx = 0.8f;
        musicSlider.value = tempMusic;
        sfxSlider.value = tempSfx;
        ApplyAudioVolumes(tempMusic, tempSfx);
        if (unsavedHandler) unsavedHandler.SetUnsaved(true);
    }
}