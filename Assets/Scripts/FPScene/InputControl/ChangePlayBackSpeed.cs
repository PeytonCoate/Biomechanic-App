using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ChangePlayBackSpeed : MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    

  

    public void SpeedUp(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (progressBar.gameObject.activeSelf)
        {
            GameObject buttonControllerObject = GameObject.Find("ButtonController");//buttoncontroller
            FileLoader fileLoader = buttonControllerObject.GetComponent<FileLoader>();
            float oldspeed = fileLoader.GetPlayBackSpeed();
            fileLoader.SetPlayBackSpeed(oldspeed + 25f);
        }
    }

    public void SlowDown(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (progressBar.gameObject.activeSelf)
        {
            GameObject buttonControllerObject = GameObject.Find("ButtonController");//buttoncontroller
            FileLoader fileLoader = buttonControllerObject.GetComponent<FileLoader>();
            float oldspeed = fileLoader.GetPlayBackSpeed();
            fileLoader.SetPlayBackSpeed(oldspeed - 25f);
        }
    }
    public void SkipBack(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        GameObject buttonControllerObject = GameObject.Find("ButtonController");//buttoncontroller
        FileLoader fileLoader = buttonControllerObject.GetComponent<FileLoader>();
        if (progressBar.gameObject.activeSelf)
        {
            fileLoader.GoBack();
        }
    }

    public void SkipForward(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        GameObject buttonControllerObject = GameObject.Find("ButtonController");//buttoncontroller
        FileLoader fileLoader = buttonControllerObject.GetComponent<FileLoader>();
        if (progressBar.gameObject.activeSelf)
        {
            fileLoader.GoForward();
        }
    }
}
