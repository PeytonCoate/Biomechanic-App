using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class UploadFileController : MonoBehaviour
{
    [SerializeField] private MenuController menuController;
    [SerializeField] private Page UploadFilePanel;

    [SerializeField] private TMP_InputField fileNameInput;
    [SerializeField] private TMP_Text fileDirectory;
    [SerializeField] private TMP_Dropdown profileDropdown;
    // Start is called before the first frame update
    void Start()
    {

    }

    
    public void OpenUploadFilePanel(string fileName, string directory)
    {
        menuController.PushPage(UploadFilePanel);
        fileNameInput.text = fileName;
        fileDirectory.text = $"Directory: {directory}";

        DirectoryInfo dir = new DirectoryInfo(directory);
        FileInfo fileInfo = new FileInfo(Path.Combine(dir.FullName, fileName));

        string FileDate = File.GetCreationTime(fileInfo.ToString()).ToString();

        Debug.Log(FileDate);
    }
}
