using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeyboardToggleFocus : MonoBehaviour
{

    [SerializeField] private CinemachineCamera cam; //so joints are only toggled in free look mode

    [SerializeField] private Toggle toggleHide;
    [SerializeField] private Toggle toggleFocus;
    public void Focus(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (cam.Priority < 1) return;

        if (toggleHide.isOn)
        {
            toggleFocus.isOn = true;
        }
        else
        {
            toggleHide.isOn = true;
        }
    }
}
