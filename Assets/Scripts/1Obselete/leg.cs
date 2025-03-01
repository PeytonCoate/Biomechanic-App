using UnityEngine;

public class ThighSegmentController : MonoBehaviour
{
    public Transform hipJoint;
    public Transform kneeJoint;

    void Update()
    {
        // Calculate the midpoint between hip and knee joints
        Vector3 midPoint = (hipJoint.position + kneeJoint.position) / 2f;
        
        // Position the thigh segment at the midpoint
        transform.position = midPoint;
        
        // Calculate the direction from hip to knee
        Vector3 direction = kneeJoint.position - hipJoint.position;
        
        // Calculate the angle and rotate the thigh segment
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 90);
    }
}
