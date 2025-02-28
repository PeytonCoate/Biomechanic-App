using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OpenCloseRecordings : MonoBehaviour
{
    [SerializeField] private GameObject openButton;
    [SerializeField] private GameObject closeButton;
    private void OnOpenRecordings()
    {
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
