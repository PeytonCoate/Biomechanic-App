using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ToggleJointSection : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private CinemachineCamera cam; //so joints are only toggled in free look mode

    [SerializeField] private Toggle toggleSection1;
    [SerializeField] private Toggle toggleSection2;
    [SerializeField] private Toggle toggleSection3;

    public void ToggleHipJoints(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (cam.Priority > 1)
        {
            toggleSection1.isOn = !toggleSection1.isOn;
        }
    }

    public void ToggleKneeJoints(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (cam.Priority > 1)
        {
            toggleSection2.isOn = !toggleSection2.isOn;
        }
    }

    public void ToggleAnkleJoints(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (cam.Priority > 1)
        {
            toggleSection3.isOn = !toggleSection3.isOn;
        }
    }
}
