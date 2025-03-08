using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class UserSettings : MonoBehaviour
{
    private float sensitivityValue;
    private float playBackSpeed;
    // Start is called before the first frame update
    void Start()
    {
        sensitivityValue = PlayerPrefs.GetFloat("Sensitivity");
        GameObject sensitivitySliderObject = GameObject.Find("MouseSensitivitySlider");
        Slider sensSlider = sensitivitySliderObject.GetComponent<Slider>();
        sensSlider.value = sensitivityValue;

        playBackSpeed = PlayerPrefs.GetFloat("PlayBackSpeed");
        GameObject playBackSpeedSliderObject = GameObject.Find("PlayBackSpeedSlider");
        Slider playBackSpeedSlider = playBackSpeedSliderObject.GetComponent<Slider>();
        playBackSpeedSlider.value = playBackSpeed;
    }

    
    public void SetSensitivity(Slider sliderSens)
    {
        sensitivityValue = sliderSens.value;
        PlayerPrefs.SetFloat("Sensitivity", sensitivityValue);
        SetAxisSensitivity();
    }
    private void SetAxisSensitivity()
    {
        GameObject freelookCamera = GameObject.Find("FreeLook Camera");
        CinemachineInputAxisController controller = freelookCamera.GetComponent<CinemachineInputAxisController>();

        foreach (var c in controller.Controllers)
        {
            if (c.Name == "Look Orbit X")
            {
                c.Input.Gain = sensitivityValue / 2;
            }
            if (c.Name == "Look Orbit Y")
            {
                c.Input.Gain = (sensitivityValue / 6) * -1;
            }
        }
    }

    public void SetPlayBackSpeed(Slider sliderSpeed)
    {
        playBackSpeed = sliderSpeed.value;
        PlayerPrefs.SetFloat("PlayBackSpeed", playBackSpeed);
        GameObject buttonControllerObject = GameObject.Find("ButtonController");//buttoncontroller
        FileLoader fileLoader = buttonControllerObject.GetComponent<FileLoader>();
        fileLoader.SetPlayBackSpeed(playBackSpeed * 100);
    }


}
