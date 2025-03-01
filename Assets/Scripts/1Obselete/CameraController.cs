using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform pathCenter; // The center point of the circular path
    public float radius = 10f; // Radius of the circular path
    public float rotationSpeed = 100f; // Speed at which the camera moves along the spline

    private float angle = 0f; // Initial angle

    void Update()
    {
        // Handle keyboard input
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            angle += rotationSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            angle -= rotationSpeed * Time.deltaTime;
        }

        // Ensure angle is within 0 to 360 degrees
        angle %= 360f;
        if (angle < 0) angle += 360f;

        // Convert angle to radians
        float radians = angle * Mathf.Deg2Rad;

        // Calculate position on the circle
        Vector3 position = new Vector3(Mathf.Cos(radians) * radius, 0, Mathf.Sin(radians) * radius);
        transform.position = pathCenter.position + position;

        // Make the camera look at the center
        transform.LookAt(pathCenter.position);
    }
}
