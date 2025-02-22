using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ChangePlayBackSpeed : MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    

  

    private void OnUp()
    {
        if (progressBar.gameObject.activeSelf)
        {
            GameObject buttonControllerObject = GameObject.Find("ButtonController");//buttoncontroller
            FileLoader fileLoader = buttonControllerObject.GetComponent<FileLoader>();
            fileLoader.RaisePlayBackSpeed();
        }
    }

    private void OnDown()
    {
        if (progressBar.gameObject.activeSelf)
        {
            GameObject buttonControllerObject = GameObject.Find("ButtonController");//buttoncontroller
            FileLoader fileLoader = buttonControllerObject.GetComponent<FileLoader>();
            fileLoader.LowerPlayBackSpeed();
        }
    }
    private void OnLeft()
    {
        GameObject buttonControllerObject = GameObject.Find("ButtonController");//buttoncontroller
        FileLoader fileLoader = buttonControllerObject.GetComponent<FileLoader>();
        if (progressBar.gameObject.activeSelf)
        {
            fileLoader.GoBack();
        }
    }

    private void OnRight()
    {
        GameObject buttonControllerObject = GameObject.Find("ButtonController");//buttoncontroller
        FileLoader fileLoader = buttonControllerObject.GetComponent<FileLoader>();
        if (progressBar.gameObject.activeSelf)
        {
            fileLoader.GoForward();
        }
    }
}
