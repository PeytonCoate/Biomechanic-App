using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class ToggleStorageType : MonoBehaviour
{

    [SerializeField] private Toggle cloudToggle;
    [SerializeField] private Toggle DesktopToggle;
    [SerializeField] private Button selectFolderButton;
    [SerializeField] private TMP_Dropdown selectUserDropdown;


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
        selectUserDropdown.gameObject.SetActive(true);
        FileManager files = GameObject.Find("ButtonController").GetComponent<FileManager>();
        files.UnloadSelectedFolderFiles();

        NetworkManager networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        networkManager.LoadUserExercises();
    }

    private void DesktopRecordings()
    {
        FileManager files = GameObject.Find("ButtonController").GetComponent<FileManager>();
        files.UnloadSelectedFolderFiles();
        files.switchToDesktopRecordings();
        selectFolderButton.gameObject.SetActive(true);
        selectUserDropdown.gameObject.SetActive(false);
    }
}
