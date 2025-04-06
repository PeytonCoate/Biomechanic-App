using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SFB;
using System.Threading;
using System.IO;
using System;
using System.Windows.Forms;
using Button = UnityEngine.UI.Button;
using System.Threading.Tasks;
using TMPro;
using System.Linq;
using UnityEngine.InputSystem.Controls;
using UnityEditor;

public class FileManager : MonoBehaviour
{

    [SerializeField]
    private GameObject RecordingButtonLocalTemplate;
    [SerializeField]
    private Transform RecordingItems;
    [SerializeField]
    private Canvas PageControl;

    [SerializeField] private Toggle desktopToggle;
    [SerializeField] private Toggle cloudToggle;

    private string mainPath = "C:\\Users\\DVXR1\\Documents\\STRIDE\\Biomechanic-App\\Assets\\Resources"; //Change this to whatever directory your recording CSV files are in
    private int sortType = 0;

    public async void AsyncSelectFolder()
    {
        await FolderSelect();

        UnloadSelectedFolderFiles();
        LoadSelectedFolderFiles(mainPath);
        GameObject buttonControllerObject = GameObject.Find("ButtonController");//buttoncontroller 
        SetScroll setScroll = buttonControllerObject.GetComponent<SetScroll>();
        setScroll.SetScrollArea();
    }


    private async Task FolderSelect() //async in order to keep main program running
    {
        await Task.Run(() =>
        {
            var path = StandaloneFileBrowser.OpenFolderPanel("Open File", mainPath, false);
            if (path != null && path.Length > 0)
            {
                mainPath = path[0];
            }

        });

    }

    public void SwitchDirectory(string directory)
    {
        mainPath = directory;

        if(GameObject.Find("DesktopStorageToggle").GetComponent<Toggle>().isOn)
        {
            UnloadSelectedFolderFiles();
            LoadSelectedFolderFiles(mainPath);
        }
    }

    public void switchToDesktopRecordings()
    {
        LoadSelectedFolderFiles(mainPath);
        GameObject buttonControllerObject = GameObject.Find("ButtonController");//buttoncontroller 
        SetScroll setScroll = buttonControllerObject.GetComponent<SetScroll>();
        setScroll.SetScrollArea();
    }

    public void switchToCloudRecordings()
    {

    }

    public void LoadSelectedFolderFiles(string mainPath)
    {
        DirectoryInfo dir = new DirectoryInfo(mainPath);
        FileInfo[] files = dir.GetFiles("*.csv");

        //for changed sort type in sorting dropdown;
        if (sortType == 1)
        {
            files = files.OrderBy(f => f.CreationTime).ToArray();
        }
        else if (sortType == 2)
        {
            files = files.OrderBy(f => File.ReadLines(f.FullName).Count()).ToArray();
        }
        else
        {
            files = files.OrderBy(f => f.Name).ToArray();
        }

        foreach (FileInfo file in files) //reads in files from folder selected.
        {
            GameObject fileButton = Instantiate(RecordingButtonLocalTemplate, RecordingItems, false) as GameObject; //instantiates a button for each marked file.
            TMP_Text[] textComponents = fileButton.GetComponentsInChildren<TMP_Text>();
            string FileDate = File.GetCreationTime(file.ToString()).ToString();
            if (textComponents.Length >= 3)
            {
                textComponents[0].text = file.Name;
                textComponents[1].text = mainPath;
                textComponents[2].text = FileDate;
            }//sets the text components in the button to the name, path, and date respectively

            Button buttonComponent = fileButton.GetComponent<Button>();//used to initiate onClick

            GameObject buttonControllerObject = GameObject.Find("ButtonController");//buttoncontroller 
            ToggleModelJoint disableFocus = buttonControllerObject.GetComponent<ToggleModelJoint>();
            ResetCamera resetcamera = buttonControllerObject.GetComponent<ResetCamera>();
            FileLoader fileLoader = buttonControllerObject.GetComponent<FileLoader>();

            GameObject canvas = GameObject.Find("Canvas");//canvas
            MenuController menuController = canvas.GetComponent<MenuController>();

            GameObject deleteButton = GameObject.Find("DeleteRecordingButton");
            InteractableButton interactableButton = deleteButton.GetComponent<InteractableButton>();

            //pops itself and any additional non-sepecial pages, pops the primary page (start), and resets the model for a new file. After, sends file name and information to file loader.
            if (buttonComponent != null)
            {
                buttonComponent.onClick.AddListener(menuController.PopAllPages);
                buttonComponent.onClick.AddListener(menuController.PopPrimaryPage);
                buttonComponent.onClick.AddListener(resetcamera.ResetCameraPosition);
                buttonComponent.onClick.AddListener(disableFocus.DisableFocusMode);
                buttonComponent.onClick.AddListener(disableFocus.TurnOffSectionToggle);
                buttonComponent.onClick.AddListener(() => fileLoader.LoadFile(mainPath, file.Name, file.FullName, fileButton));
                buttonComponent.onClick.AddListener(fileLoader.PauseFile);
                buttonComponent.onClick.AddListener(interactableButton.InteractButtonOn);
            }


            Button uploadButton = fileButton.transform.Find("UploadButton").GetComponent<Button>();
             
            if (uploadButton != null)
            {
                uploadButton.onClick.AddListener(menuController.PopPage);
                uploadButton.onClick.AddListener(() => buttonControllerObject.GetComponent<UploadFileController>().OpenUploadFilePanel(file.Name, mainPath));
            }
        }
    }

    public void UnloadSelectedFolderFiles()
    {

        GameObject recordingButtons = GameObject.Find("RecordingItems");
        while (recordingButtons.transform.childCount > 0)
        {
            DestroyImmediate(recordingButtons.transform.GetChild(0).gameObject);
        }
    }

    public void ChangeSortType(int sort)
    {
        sortType = sort;
        UnloadSelectedFolderFiles();

        if (desktopToggle.isOn)
        {
            LoadSelectedFolderFiles(mainPath);
        }

        if (cloudToggle.isOn)
        {
            NetworkManager networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
            networkManager.changeSortType(sortType);
            networkManager.LoadUserExercises();
        }

    }

}
