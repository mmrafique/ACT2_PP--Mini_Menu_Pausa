    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    public class ResolutionSettings : MonoBehaviour
    {
        public TMP_Dropdown resolutionDropdown;
        public Toggle fullscreenToggle;
        public Button applyButton;
        public Button defaultButton;

    public UnsavedChangesHandler unsavedHandler;
        public AudioSettingsManager audioManager;
    List<Resolution> uniqueResolutions = new List<Resolution>();
        int selectedIndex = 0;

        const string RES_W_PREF = "pref_res_width";
        const string RES_H_PREF = "pref_res_height";
        const string RES_FULLSCREEN = "pref_fullscreen";

        void Start()
        {
            PopulateResolutions();
            fullscreenToggle.isOn = PlayerPrefs.GetInt(RES_FULLSCREEN, Screen.fullScreen ? 1 : 0) == 1;

            int savedW = PlayerPrefs.GetInt(RES_W_PREF, Screen.currentResolution.width);
            int savedH = PlayerPrefs.GetInt(RES_H_PREF, Screen.currentResolution.height);

            for (int i = 0; i < uniqueResolutions.Count; i++)
            {
                if (uniqueResolutions[i].width == savedW && uniqueResolutions[i].height == savedH)
                {
                    resolutionDropdown.value = i;
                    selectedIndex = i;
                    break;
                }
            }

            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggleChanged);

            if (applyButton != null) applyButton.onClick.AddListener(OnApply);
            if (defaultButton != null) defaultButton.onClick.AddListener(OnDefault);
        }

        void PopulateResolutions()
        {
            resolutionDropdown.ClearOptions();
            Resolution[] all = Screen.resolutions;

            var map = new Dictionary<string, Resolution>();
            foreach (var r in all)
            {
                string key = r.width + "x" + r.height;
                if (!map.ContainsKey(key) || r.refreshRate > map[key].refreshRate)
                {
                    map[key] = r;
                }
            }

            uniqueResolutions.Clear();
            foreach (var kv in map) uniqueResolutions.Add(kv.Value);

            uniqueResolutions.Sort((a,b) =>
            {
                int cmp = b.width.CompareTo(a.width);
                if (cmp == 0) return b.height.CompareTo(a.height);
                return cmp;
            });

            List<string> options = new List<string>();
            int defaultIndex = 0;
            for (int i = 0; i < uniqueResolutions.Count; i++)
            {
                var r = uniqueResolutions[i];
                options.Add(r.width + " x " + r.height + " @" + r.refreshRate + "Hz");
                if (r.width == Screen.currentResolution.width && r.height == Screen.currentResolution.height) defaultIndex = i;
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = defaultIndex;
            resolutionDropdown.RefreshShownValue();
        }

        void OnResolutionChanged(int idx)
        {
            selectedIndex = idx;
            ApplyResolution(uniqueResolutions[selectedIndex]);
            if (unsavedHandler != null) unsavedHandler.SetUnsaved(true);
            if (audioManager != null) audioManager.PlayUIClick();
        }

        void OnFullscreenToggleChanged(bool isOn)
        {
            Screen.fullScreen = isOn;
            if (unsavedHandler != null) unsavedHandler.SetUnsaved(true);
            if (audioManager != null) audioManager.PlayUIClick();
        }

        void ApplyResolution(Resolution r)
        {
            Screen.SetResolution(r.width, r.height, Screen.fullScreen, r.refreshRate);
        }

        public void OnApply()
        {
            var r = uniqueResolutions[selectedIndex];
            PlayerPrefs.SetInt(RES_W_PREF, r.width);
            PlayerPrefs.SetInt(RES_H_PREF, r.height);
            PlayerPrefs.SetInt(RES_FULLSCREEN, Screen.fullScreen ? 1 : 0);
            PlayerPrefs.Save();
            if (unsavedHandler != null) unsavedHandler.SetUnsaved(false);
        }

        public void OnDefault()
        {
            int defW = 1280;
            int defH = 720;

            int idx = uniqueResolutions.FindIndex(r => r.width == defW && r.height == defH);
            if (idx == -1)
            {
                idx = uniqueResolutions.FindIndex(r => r.width == Screen.currentResolution.width && r.height == Screen.currentResolution.height);
            }
            if (idx != -1)
            {
                resolutionDropdown.value = idx;
                ApplyResolution(uniqueResolutions[idx]);
            }
            fullscreenToggle.isOn = true;
            Screen.fullScreen = true;
            if (unsavedHandler != null) unsavedHandler.SetUnsaved(true);
        }
    }
