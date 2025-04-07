using System.Collections;
using System.Collections.Generic;
using System;
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

    /* obsolete: caused undefined behavior
    public void ToggleStorage()
    {
        // Only respond to toggles being turned ON
        if (cloudToggle.isOn)
        {
            Debug.Log("Cloud Toggle turned on\n" + Environment.StackTrace);
            CloudRecordings();
        }
        else if (DesktopToggle.isOn)
        {
            Debug.Log("Desktop Toggle turned on\n" + Environment.StackTrace);
            DesktopRecordings();
        }
    }
    */

    public void OnCloudToggleChanged(bool value)
    {
        if (value) CloudRecordings();
    }
    public void OnDesktopToggleChanged(bool value)
    {
        if (value) DesktopRecordings();
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
