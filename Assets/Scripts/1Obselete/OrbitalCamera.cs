using UnityEngine;

public class MouseOrbitCamera : MonoBehaviour
{
    public Transform cameraAnchor; // Assign the CameraAnchor in the Inspector
    public float rotationSpeed; // Speed of rotation 
    public float upperVerticalClampAngle; // Clamp for vertical rotation (to prevent flipping)
    
    public float lowerVerticalClampAngle;

    private float horizontalAngle = 0f; // Horizontal rotation angle (Y-axis)
    private float verticalAngle = 0f;   // Vertical rotation angle (X-axis)

    void Start()
    {
        // Initialize rotation angles based on the anchor's current rotation
        Vector3 anchorRotation = cameraAnchor.eulerAngles;
        horizontalAngle = anchorRotation.y;
        verticalAngle = anchorRotation.x;
    }

    void Update()
    {
        // Check if the right mouse button is held down
        if (Input.GetMouseButton(1)) { // 1 = Right Mouse Button
            // Get mouse input
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Adjust angles based on mouse movement and rotationSpeed
            horizontalAngle += mouseX * rotationSpeed * Time.deltaTime; // Apply Time.deltaTime to smooth out the rotation
            verticalAngle -= mouseY * rotationSpeed * Time.deltaTime;   // Apply Time.deltaTime here too

            // Clamp the vertical angle to prevent the camera from flipping
            verticalAngle = Mathf.Clamp(verticalAngle, -lowerVerticalClampAngle, upperVerticalClampAngle);

            // Apply the rotation to the camera anchor
            cameraAnchor.rotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0f);
        }
    }

    public void ResetAngles(Vector3 resetRotation) {
        horizontalAngle = resetRotation.y;
        verticalAngle = resetRotation.x;
    }
}
