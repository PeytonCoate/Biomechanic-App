using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Rendering;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class FreeLookRightClickControl : MonoBehaviour
{
    [SerializeField]
    private CinemachineInputAxisController cineCam;
    [SerializeField]
    private CinemachineCamera cam;
    [SerializeField]
    private CinemachineOrbitalFollow orbitalFollow; //For WASD

    [SerializeField]
    private float horizontalSpeed = 200f; // Speed for horizontal rotation
    [SerializeField]
    private float verticalSpeed = 50f; // Speed for vertical rotation
    [SerializeField]
    private float smoothTime = 0.1f;
    [SerializeField]
    private float decelerationTime = 0.1f;


    private Vector2 input = Vector2.zero;
    private Vector2 currentVelocity = Vector2.zero;


    void Update()
    {

        if (Mouse.current.rightButton.isPressed && cam.Priority.Value > 1)
        {
            StopRecentering();
            cineCam.enabled = true;
        }
        else
        {
            cineCam.enabled = false;
        }


        float targetInputX = 0f;
        float targetInputY = 0f;

        if (Keyboard.current.wKey.isPressed && cam.Priority.Value > 1)
        {
            StopRecentering();
            targetInputY = 1f;
        }
        if (Keyboard.current.sKey.isPressed && cam.Priority.Value > 1)
        {
            StopRecentering();
            targetInputY = -1f;
        }
        if (Keyboard.current.dKey.isPressed && cam.Priority.Value > 1)
        {
            StopRecentering();
            targetInputX = 1f;
        }
        if (Keyboard.current.aKey.isPressed && cam.Priority.Value > 1)
        {
            StopRecentering();
            targetInputX = -1f;
        }

        float appliedSmoothTime = (targetInputX == 0 && targetInputY == 0) ? decelerationTime : smoothTime;

        input.x = Mathf.SmoothDamp(input.x, targetInputX, ref currentVelocity.x, appliedSmoothTime);
        input.y = Mathf.SmoothDamp(input.y, targetInputY, ref currentVelocity.y, appliedSmoothTime);

        if (input != Vector2.zero)
        {
            orbitalFollow.HorizontalAxis.Value += input.x * horizontalSpeed * Time.deltaTime;
            orbitalFollow.VerticalAxis.Value -= input.y * verticalSpeed * Time.deltaTime;

        }

        if (input.magnitude > 0.001f) // Only move if input is significant
        {
            orbitalFollow.HorizontalAxis.Value += input.x * horizontalSpeed * Time.deltaTime;
            float newVerticalValue = orbitalFollow.VerticalAxis.Value - input.y * verticalSpeed * Time.deltaTime;
            orbitalFollow.VerticalAxis.Value = Mathf.Clamp(newVerticalValue, -10f, 45f);
        }
    }

    public void StopRecentering()
    {
        orbitalFollow.HorizontalAxis.Recentering.Enabled = false;
        orbitalFollow.VerticalAxis.Recentering.Enabled = false;
    }
}
