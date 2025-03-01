using UnityEngine;
using UnityEngine.UI;

public class ResetCameraButton : MonoBehaviour
{
    public Button button; // Reference to the UI Button
    public Transform cameraAnchor; // Reference to the camera's parent (cameraAnchor)
    public MouseOrbitCamera mouseOrbitCamera; // Reference to the MouseOrbitCamera script

    private Vector3 initialAnchorPosition; // Store the initial position of the anchor
    private Quaternion initialAnchorRotation; // Store the initial rotation of the anchor

    void Start()
    {

        if (cameraAnchor != null) {
            initialAnchorPosition = cameraAnchor.position;
            initialAnchorRotation = cameraAnchor.rotation;
        }
        else {
            Debug.LogWarning("Camera Anchor reference not assigned.");
        }

        if (button != null) {
            button.onClick.AddListener(OnButtonClick);
        }
        else {
            Debug.LogWarning("Button reference not assigned.");
        }
    }

    void OnButtonClick() {

        if (cameraAnchor != null) {
            cameraAnchor.position = initialAnchorPosition;
            cameraAnchor.rotation = initialAnchorRotation;

            if (mouseOrbitCamera != null) {
                mouseOrbitCamera.ResetAngles(initialAnchorRotation.eulerAngles);
            }
            else {
                Debug.LogWarning("MouseOrbitCamera script reference not assigned.");
            }
        }
        else {
            Debug.LogWarning("Camera Anchor is missing!");
        }
    }
}
