using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class OpenCloseRecordings : MonoBehaviour
{
    [SerializeField] private GameObject openButton;
    [SerializeField] private GameObject closeButton;
    public void OpenRecordings(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        Button button1 = openButton.GetComponent<Button>();
        Button button2 = closeButton.GetComponent<Button>();
        if (openButton.activeSelf && button1.interactable)
        {
            button1.onClick.Invoke();
            return;
        }

        if (closeButton.activeSelf && button2.interactable)
        {
            button2.onClick.Invoke();
            return;
        }
    }
}
