using UnityEngine;
using UnityEngine.UI; // Import the UI namespace to access UI elements like Button

public class PlaybackButton : MonoBehaviour
{
    public Button button; // Reference to the UI Button

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the button reference is set
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick); // Register the button click event
        }
        else
        {
            Debug.LogWarning("Button reference not assigned.");
        }
    }

    // This function is called when the button is clicked
    void OnButtonClick()
    {
        Debug.Log("Playback Clicked"); // Log the message when the button is clicked
    }
}