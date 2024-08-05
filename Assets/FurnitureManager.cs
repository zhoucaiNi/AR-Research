using UnityEngine;
using UnityEngine.UI;

public class FurnitureManager : MonoBehaviour
{
    public Button[] toggleButtons; // Assign the UI Buttons in the Inspector
    public GameObject[] furnitureItems; // Assign the furniture items in the Inspector
    private Vector3[] originalPositions; // Array to store the original positions of the furniture items
    private Quaternion[] originalRotations; // Array to store the original rotations of the furniture items

    void Start()
    {
        // Initialize the original positions and rotations arrays
        originalPositions = new Vector3[furnitureItems.Length];
        originalRotations = new Quaternion[furnitureItems.Length];

        // Set all furniture items to be initially off and store their original positions and rotations
        for (int i = 0; i < furnitureItems.Length; i++)
        {
            if (furnitureItems[i] != null)
            {
                originalPositions[i] = furnitureItems[i].transform.position;
                originalRotations[i] = furnitureItems[i].transform.rotation;
                furnitureItems[i].SetActive(false);
            }
        }

        // Add listeners to the buttons
        for (int i = 0; i < toggleButtons.Length; i++)
        {
            int index = i; // Capture the current index
            toggleButtons[i].onClick.AddListener(() => ToggleVisibility(index));
        }
    }

    void ToggleVisibility(int index)
    {
        if (furnitureItems[index] != null)
        {
            bool isActive = furnitureItems[index].activeSelf;
            if (isActive)
            {
                // If the item is currently active, disable it
                furnitureItems[index].SetActive(false);
            }
            else
            {
                // If the item is currently inactive, enable it and reset to the original position and rotation
                furnitureItems[index].SetActive(true);
                furnitureItems[index].transform.position = originalPositions[index];
                furnitureItems[index].transform.rotation = originalRotations[index]; // Reset to original rotation
            }
        }
    }
}
