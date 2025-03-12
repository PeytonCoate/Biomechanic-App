using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnStartup : MonoBehaviour
{
    [SerializeField] private CanvasGroup settingsAlpha;
    [SerializeField] private CanvasGroup loginAlpha;
    void Awake()//sets the target frame rate to 165
    {
        settingsAlpha.alpha = 0;
        loginAlpha.alpha = 0;
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 200;
    }


}
