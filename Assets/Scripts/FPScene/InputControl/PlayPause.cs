using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayPause : MonoBehaviour
{
    [SerializeField] private Toggle playToggle;
    [SerializeField] private Image playImage;
    [SerializeField] private Image pauseImage;
    public void Pause(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        GameObject buttonControllerObject = GameObject.Find("ButtonController");//buttoncontroller 
        FileLoader fileLoader = buttonControllerObject.GetComponent<FileLoader>();
        fileLoader.togglePlayButton();
    }

    public void ToggleSpriteChange()
    {
        if (playToggle.isOn)
        {
            Color offColor = playImage.color;
            offColor.a = Mathf.Clamp01(0);
            playImage.color = offColor;
        }
        else
        {
            Color onColor = playImage.color;
            onColor.a = Mathf.Clamp01(1);
            playImage.color = onColor;
        }
    }
}
