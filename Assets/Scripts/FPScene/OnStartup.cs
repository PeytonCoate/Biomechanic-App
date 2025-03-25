using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnStartup : MonoBehaviour
{
    [SerializeField] private CanvasGroup settingsAlpha;
    [SerializeField] private CanvasGroup loginAlpha;
    [SerializeField] private CanvasGroup saveExerciseAlpha;
    void Awake()//sets the target frame rate to 165
    {
        settingsAlpha.alpha = 0;
        loginAlpha.alpha = 0;
        saveExerciseAlpha.alpha = 0;
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 200;

        //string defaultPath = PlayerPrefs.GetString("DefaultPath", GetOutputDirectory());

    }

    string GetOutputDirectory()
    {
        string registryKey = @"Software\MyUnityGame";
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryKey))
        {
            return key?.GetValue("OutputDirectory", "C:\\DefaultGameOutput") as string;
        }
    }

}
