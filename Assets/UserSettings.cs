using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using SFB;
using System.Threading.Tasks;
using UnityEngine.UI;

public class UserSettings : MonoBehaviour
{
    private float sensitivityValue;
    private float playBackSpeed;
    private int skipIntervalIndex;
    private int sortModeIndex;
    private string defaultPath;
    // Start is called before the first frame update
    void Start()
    {
        sensitivityValue = PlayerPrefs.GetFloat("Sensitivity", 0.5f);
        GameObject sensitivitySliderObject = GameObject.Find("MouseSensitivitySlider");
        Slider sensSlider = sensitivitySliderObject.GetComponent<Slider>();
        sensSlider.value = sensitivityValue;

        playBackSpeed = PlayerPrefs.GetFloat("PlayBackSpeed", 1f);
        GameObject playBackSpeedSliderObject = GameObject.Find("PlayBackSpeedSlider");
        Slider playBackSpeedSlider = playBackSpeedSliderObject.GetComponent<Slider>();
        playBackSpeedSlider.value = playBackSpeed;

        skipIntervalIndex = PlayerPrefs.GetInt("SkipIntervalIndex");
        GameObject.Find("SkipIntervalDropdown").GetComponent<TMP_Dropdown>().value = skipIntervalIndex;

        sortModeIndex = PlayerPrefs.GetInt("SortModeIndex");
        GameObject.Find("DefaultSortModeDropDown").GetComponent<TMP_Dropdown>().value = sortModeIndex;

        
        defaultPath = PlayerPrefs.GetString("DefaultPath");
        if (defaultPath != "")
        {
            GameObject.Find("DefaultDirectoryName").GetComponent<TMP_Text>().text = defaultPath;
            GameObject.Find("ButtonController").GetComponent<FileManager>().SwitchDirectory(defaultPath);
            //GameObject.Find("ButtonController").GetComponent<RecordFileManager>().SwitchDirectory(defaultPath);
        }
        else
        {
            GameObject.Find("DefaultDirectoryName").GetComponent<TMP_Text>().text = "D:\\IR\\EC\\TO\\RY";
        }
        

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

    public void SetSkipInterval(TMP_Dropdown dropdown)
    {
        skipIntervalIndex = dropdown.value;
        PlayerPrefs.SetInt("SkipIntervalIndex", skipIntervalIndex);

        float skipIntervalValue = float.Parse(dropdown.options[skipIntervalIndex].text);
        GameObject.Find("ButtonController").GetComponent<FileLoader>().SetSkipIntervalSpeed(skipIntervalValue);
    }

    public void SetDefaultSortMode(TMP_Dropdown dropdown)
    {
        sortModeIndex = dropdown.value;
        PlayerPrefs.SetInt("SortModeIndex", sortModeIndex);
        GameObject.Find("SortByDropdown").GetComponent<TMP_Dropdown>().value = sortModeIndex;
    }

    public async void SetDefaultDirectory()
    {
        await DefaultFolderSelect();
        PlayerPrefs.SetString("DefaultPath", defaultPath);
        GameObject.Find("DefaultDirectoryName").GetComponent<TMP_Text>().text = defaultPath;
        GameObject.Find("ButtonController").GetComponent<FileManager>().SwitchDirectory(defaultPath);
        //GameObject.Find("ButtonController").GetComponent<RecordFileManager>().SwitchDirectory(defaultPath);
    }

    private async Task DefaultFolderSelect() //async in order to keep main program running
    {
        await Task.Run(() =>
        {
            var path = StandaloneFileBrowser.OpenFolderPanel("Open File", defaultPath, false);
            if (path != null && path.Length > 0)
            {
                defaultPath = path[0];
                
            }
        });

    }
}
