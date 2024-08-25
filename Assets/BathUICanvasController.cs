using UnityEngine;
using UnityEngine.UI;

public class BathUICanvasController : MonoBehaviour
{
    public Button[] buttons; // Assign the UI Buttons in the Inspector

    void Start()
    {
        // Add listeners to the buttons
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => OnButtonClick(button));
        }
    }

    void OnButtonClick(Button button)
    {
        Debug.Log($"Button {button.name} clicked.");
        // Add your button click handling logic here
    }
}
