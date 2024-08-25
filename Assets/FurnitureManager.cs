using UnityEngine;
using UnityEngine.UI;

public class FurnitureManager : MonoBehaviour
{
    public Button[] toggleButtons; // Assign the UI Buttons in the Inspector
    public GameObject[] furnitureItems; // Assign the furniture items in the Inspector
    private Vector3[] originalPositions; // Array to store the original positions of the furniture items
    private Quaternion[] originalRotations; // Array to store the original rotations of the furniture items
    private string label; // Store the label

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

    public void SetLabel(string label)
    {
        this.label = label;
    }

    void ToggleVisibility(int index)
    {
        if (furnitureItems[index] != null)
        {
            bool isActive = furnitureItems[index].activeSelf;
            if (isActive)
            {
                furnitureItems[index].SetActive(false);
            }
            else
            {
                furnitureItems[index].SetActive(true);

                Vector3 spawnPosition = Vector3.zero;
                if (label == "Lighting")
                {
                    spawnPosition.y = 0.7f;
                }
                else
                {
                    spawnPosition.y = originalPositions[index].y;
                }
                furnitureItems[index].transform.position = spawnPosition;

                furnitureItems[index].transform.rotation = GetAlignedRotation();
                AlignToNearestOrthogonalAngle(furnitureItems[index].GetComponent<Renderer>());

                Debug.Log($"Furniture item {index} activated at position: {spawnPosition}");
            }
        }
    }


    Quaternion GetAlignedRotation()
    {
        // Randomly select one of the orthogonal angles
        int[] angles = { 0, 90, 180, 270 };
        int randomIndex = Random.Range(0, angles.Length);
        return Quaternion.Euler(0, angles[randomIndex], 0);
    }

    public void AlignToNearestOrthogonalAngle(Renderer renderer)
    {
        if (renderer != null)
        {
            Vector3 eulerAngles = renderer.transform.rotation.eulerAngles;
            eulerAngles.x = Mathf.Round(eulerAngles.x / 90) * 90;
            eulerAngles.y = Mathf.Round(eulerAngles.y / 90) * 90;
            eulerAngles.z = Mathf.Round(eulerAngles.z / 90) * 90;
            renderer.transform.rotation = Quaternion.Euler(eulerAngles);
            Debug.Log($"Aligned renderer to nearest orthogonal angle: {eulerAngles}");
        }
        else
        {
            Debug.LogError("No Renderer component found.");
        }
    }
}
