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
        Vector2 scrollValue = scrollAction.action.ReadValue<Vector2>();

        //vertical scroll
        if (scrollValue.y != 0)
        {

            //Update normalized position
            scrollRect.verticalNormalizedPosition += scrollValue.y * scrollSpeed * Time.deltaTime;

            //clapm
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition);
        }
    }
}
