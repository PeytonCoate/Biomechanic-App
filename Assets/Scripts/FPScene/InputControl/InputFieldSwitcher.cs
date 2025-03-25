using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class InputFieldSwitcher : MonoBehaviour
{
    [SerializeField] private PlayerInput playerController = null;
    [SerializeField] private InputSystemUIInputModule inputModule = null;

    public void InputSystemSwitchTo()
    {
        playerController.SwitchCurrentActionMap("InputField");

        inputModule.point = InputActionReference.Create(playerController.currentActionMap["Point"]);
        inputModule.leftClick = InputActionReference.Create(playerController.currentActionMap["Click"]);
        inputModule.cancel = InputActionReference.Create(playerController.currentActionMap["CancelInput"]);
    }

    public void InputSystemSwitchBack()
    {
        playerController.SwitchCurrentActionMap("UI");
        inputModule.point = InputActionReference.Create(playerController.currentActionMap["Point"]);
        inputModule.leftClick = InputActionReference.Create(playerController.currentActionMap["Click"]);
        inputModule.cancel = InputActionReference.Create(playerController.currentActionMap["Back"]);
    }
}
