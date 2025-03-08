using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;
using static UnityEngine.Rendering.DebugUI;



public class FileLoader : MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text ProgressAt;
    [SerializeField] private TMP_Text fileName;
    [SerializeField] private Toggle playToggle;
    [SerializeField] GameObject mainModel;

    private GameObject fileButton;
    private float skipIntervalValue;
    private float updateIntervalSpeed = 100f;
    private float progress = 0f;
    private string fullFileName = "";
    private string folderPath = "";
    private int rowCount;
    void Update()
    {
        MovementController movementController = mainModel.GetComponent<MovementController>();
        if (playToggle.isOn)
        {
            progress += Time.deltaTime;
            while (progress >= 1/updateIntervalSpeed)
            {
                progressBar.value++;
                progress -= 1/updateIntervalSpeed;
            }
        }
        if (rowCount != 0)
        {
            movementController.loadJoints();
            if (progressBar.value < progressBar.maxValue)
            {
                movementController.LoadCSVData(fullFileName, (int)progressBar.value);
            }
            inputField.gameObject.SetActive(true);
            progressBar.gameObject.SetActive(true);
            ProgressAt.text = progressBar.value.ToString() + "/" + (rowCount);
        }
        else
        {
            movementController.unloadJoints();
            inputField.gameObject.SetActive(false);
            progressBar.gameObject.SetActive(false);
            ProgressAt.text = "";
        }
        if (progressBar.value == progressBar.maxValue)
        {
            PauseFile();
        }
    }
    public void LoadFile(string mainPath, string fileToOpen, string fileFullname, GameObject filebutton)
    {
        folderPath = mainPath;
        fullFileName = fileFullname;
        fileButton = filebutton;
        fileName.text = fileToOpen;
        rowCount = File.ReadAllLines(fileFullname).Length;

        progressBar.maxValue = rowCount;
        progressBar.value = 0;

        ProgressAt.text = progressBar.value.ToString() + "/" + rowCount;
    }
    public void EnableProgressBar()
    {
        progressBar.enabled = true;
    }
    public void DisableProgressBar()
    {
        progressBar.enabled = false;
    }
    public void PlayFile()
    {
        if (progressBar.value == progressBar.maxValue)
        {
            progressBar.value = 1;
        }
        playToggle.isOn = true;
    }
    public void PauseFile()
    {
        playToggle.isOn = false;
    }

    public void togglePlayButton()
    {
        if (!progressBar.enabled) return;
        if (playToggle.isOn)
        {
            PauseFile();
        }
        else
        {
            PlayFile();
        }
    }

    public float GetPlayBackSpeed()
    {
        return updateIntervalSpeed;
    }
    public void SetPlayBackSpeed(float interval)
    {
        if (interval < 25f)
        {
            updateIntervalSpeed = 25f;
            return;
        }
        if (interval > 200f)
        {
            updateIntervalSpeed = 200f;
            return;
        }
        updateIntervalSpeed = interval;
    }


    public void SetSkipIntervalSpeed(float skipInterval)
    {
        skipIntervalValue = skipInterval;
    }

    public void GoBack()
    {
        progressBar.value -= skipIntervalValue;
    }

    public void GoForward()
    {
        progressBar.value += skipIntervalValue;
    }

    public void DeleteFile()
    {
        string filePath = fullFileName;
        if (fullFileName != "" && File.Exists(filePath))
        {
            File.Delete(filePath);
            if (File.Exists(filePath + ".meta"))
            {
                File.Delete(filePath + ".meta");
            }
            GameObject buttonControllerObject = GameObject.Find("ButtonController");
            FileManager files = buttonControllerObject.GetComponent<FileManager>();
            fileName.text = "";
            files.UnloadSelectedFolderFiles();
            files.LoadSelectedFolderFiles(folderPath);
            rowCount = 0;
            Debug.Log("File deleted successfully: " + filePath);
        }
        else
        {
            Debug.LogWarning("File not found: " + filePath);
        }
    }
}
