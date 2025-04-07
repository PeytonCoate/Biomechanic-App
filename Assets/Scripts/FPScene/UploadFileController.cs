using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class UploadFileController : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;

    [SerializeField] private MenuController menuController;
    [SerializeField] private Page UploadFilePanel;

    [SerializeField] private TMP_InputField fileNameInput;
    [SerializeField] private TMP_Text fileDirectory;
    [SerializeField] private TMP_Dropdown profileDropdown;

    private string fileTitle;
    private string fileFullName;
    private string fileDate;

    // Start is called before the first frame update
    void Start()
    {

    }

    
    public void OpenUploadFilePanel(string fileName, string directory)
    {
        menuController.PushPage(UploadFilePanel);
        fileNameInput.text = fileName;
        fileDirectory.text = $"Directory: {directory}";

        fileTitle = fileName;

        DirectoryInfo dir = new DirectoryInfo(directory);

        fileFullName = Path.Combine(dir.FullName, fileName);

        FileInfo fileInfo = new FileInfo(fileFullName);

        fileDate = File.GetCreationTime(fileInfo.ToString()).ToString();

        Debug.Log(fileDate);
    }

    public void Upload()
    {
        byte[] fileData = File.ReadAllBytes(fileFullName);

        if (fileTitle.Contains("_"))
        {
            int index = fileTitle.IndexOf('_');
            fileTitle = fileTitle.Substring(0, index);
        }

        networkManager.TryUpload(fileData, fileTitle);

    }
}
