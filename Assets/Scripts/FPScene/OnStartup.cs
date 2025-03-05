using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnStartup : MonoBehaviour
{
    void Awake()//sets the target frame rate to 165
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 200;
    }

}
