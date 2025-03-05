using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class ToggleStorageType : MonoBehaviour
{

    [SerializeField] private Toggle cloudToggle;
    [SerializeField] private Toggle DesktopToggle;
    [SerializeField] private Button selectFolderButton;


    public void ToggleStorage()
    {
        if (cloudToggle.isOn)
        {
            CloudRecordings();
        }
        else if (DesktopToggle.isOn)
        {
            DesktopRecordings();
        }
    }

    private void CloudRecordings()
    {
        selectFolderButton.gameObject.SetActive(false);
        GameObject buttonControllerObject = GameObject.Find("ButtonController");
        FileManager files = buttonControllerObject.GetComponent<FileManager>();
        files.UnloadSelectedFolderFiles();
    }

    private void DesktopRecordings()
    {
        selectFolderButton.gameObject.SetActive(true);
        GameObject buttonControllerObject = GameObject.Find("ButtonController");
        FileManager files = buttonControllerObject.GetComponent<FileManager>();
        files.UnloadSelectedFolderFiles();
        files.switchToDesktopRecordings();
    }
}
