using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ScrollWheelController : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private InputActionReference scrollAction; 

    public float scrollSpeed = 0.1f;

    void Update()
    {
        HandleScrollInput();
    }

    private void HandleScrollInput()
    {
        // Read the scroll input (vector2)
        Vector2 scrollValue = scrollAction.action.ReadValue<Vector2>();

        // Check if there is vertical scroll input
        if (scrollValue.y != 0)
        {
            Debug.Log("scroll");

            // Update the ScrollRect's normalized position based on scroll value
            scrollRect.verticalNormalizedPosition += scrollValue.y * scrollSpeed * Time.deltaTime;

            // Clamp to make sure the value stays between 0 and 1
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition);
        }
    }
}
