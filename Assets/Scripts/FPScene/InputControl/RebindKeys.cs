using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindKeys : MonoBehaviour
{
    [SerializeField] private InputActionAsset _actionAsset;
    [SerializeField] private PlayerInput playerController = null;

    private GameObject startRebindObject = null;
    private GameObject waitingForInputObject = null;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;


    private void Start()
    {
        string rebinds = PlayerPrefs.GetString("rebinds", string.Empty);

        if (string.IsNullOrEmpty(rebinds)) return;

        playerController.actions.LoadBindingOverridesFromJson(rebinds);
    }

    private void SaveBinds()
    {
        string rebinds = playerController.actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }
    public void StartRebind(GameObject buttonPressed)
    {
        startRebindObject =  buttonPressed.transform.GetChild(2).gameObject;
        waitingForInputObject = buttonPressed.transform.GetChild(3).gameObject;

        string actionName = buttonPressed.transform.GetChild(1).GetComponent<TMP_Text>().text;

        InputAction actionToRebind = _actionAsset.FindAction(actionName);

        startRebindObject.SetActive(false);
        waitingForInputObject.SetActive(true);

        playerController.SwitchCurrentActionMap("Rebind");


        rebindingOperation = actionToRebind.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => RebindComplete(actionToRebind, buttonPressed))
            .Start();
    
        
    }

    private void RebindComplete(InputAction actionToRebind, GameObject buttonPressed)
    {
        int bindingIndex = actionToRebind.GetBindingIndexForControl(actionToRebind.controls[0]);
        
        buttonPressed.transform.GetChild(0).GetComponent<TMP_Text>().text = InputControlPath.ToHumanReadableString(
            actionToRebind.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);

        rebindingOperation.Dispose();

        startRebindObject.SetActive(true);
        waitingForInputObject.SetActive(false);

        playerController.SwitchCurrentActionMap("UI");

        SaveBinds();
    }
}
