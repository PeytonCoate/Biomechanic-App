using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;



public class FileLoader : MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private TMP_Text ProgressAt;
    [SerializeField] private TMP_Text fileName;
    [SerializeField] private Toggle playToggle;
    [SerializeField] GameObject mainModel;

    private GameObject fileButton;
    private float updateIntervalSpeed = 100f;
    private float progress = 0f;
    private float updateInterval = 1f/100f;
    private string fullFileName = "";
    private string folderPath = "";
    private int rowCount;
    void Update()
    {
        MovementController movementController = mainModel.GetComponent<MovementController>();
        if (playToggle.isOn)
        {
            progress += Time.deltaTime;
            while (progress >= updateInterval)
            {
                progressBar.value++;
                progress -= updateInterval;
            }
        }
        if (rowCount != 0)
        {
            movementController.loadJoints();
            if (progressBar.value < progressBar.maxValue)
            {
                movementController.LoadCSVData(fullFileName, (int)progressBar.value);
            }

            progressBar.gameObject.SetActive(true);
            ProgressAt.text = progressBar.value.ToString() + "/" + (rowCount);
        }
        else
        {
            movementController.unloadJoints();
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

    public void LowerPlayBackSpeed()
    {
        if (updateIntervalSpeed > 10f)
        {
            updateIntervalSpeed -= 10f;
        }
        updateInterval = 1 / updateIntervalSpeed;
    }

    public void RaisePlayBackSpeed()
    {
        if (updateIntervalSpeed <= 200f)
        {
            updateIntervalSpeed += 10f;
        }
        updateInterval = 1 / updateIntervalSpeed;
    }

    public void GoBack()
    {
        progressBar.value -= 25f;
    }

    public void GoForward()
    {
        progressBar.value += 25f;
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
