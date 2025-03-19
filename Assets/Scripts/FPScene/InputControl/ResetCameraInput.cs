using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class ResetCameraInput : MonoBehaviour
{
    [SerializeField] private GameObject ButtonController;
    [SerializeField] private CinemachineCamera freeLookCam;

    public void ResetCamera(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (freeLookCam.Priority.Value > 1)
        {
            ResetCamera reset = ButtonController.GetComponent<ResetCamera>();
            reset.ResetCameraPosition();
        }
    }
}
