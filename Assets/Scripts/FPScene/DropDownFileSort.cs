using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DropDownFileSort : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;

    public void changeFileSortType()
    {
        GameObject buttonControllerObject = GameObject.Find("ButtonController");
        FileManager files = buttonControllerObject.GetComponent<FileManager>();
        int selectedIndex = dropdown.value;
        files.ChangeSortType(selectedIndex);
    }
}
