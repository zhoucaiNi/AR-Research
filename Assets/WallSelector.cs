using System.Collections.Generic;
using UnityEngine;

// testing
public class WallSelector : MonoBehaviour
{
    public GameObject[] wallPrefabs; // Array to store wall prefabs
    public Transform[] spawnPoints; // Array to store spawn points for each wall prefab
    public Material[] wallMaterials; // Array to store different materials

    private GameObject[] currentWalls; // Array to store instantiated wall objects
    private Renderer[] currentWallRenderers; // Array to store renderers of the instantiated walls
    private int currentWallIndex = -1; // Currently selected wall index

    // Dictionary to store current materials for each wall
    private Dictionary<int, Material> currentMaterials;

    void Start()
    {
        // Initialize the dictionary and arrays
        currentMaterials = new Dictionary<int, Material>();
        currentWalls = new GameObject[wallPrefabs.Length];
        currentWallRenderers = new Renderer[wallPrefabs.Length];

        // Set initial materials for each wall to white
        Material initialMaterial = new Material(wallMaterials[3]); // Assuming the white material is at index 2

        for (int i = 0; i < wallPrefabs.Length; i++)
        {
            // Instantiate walls and set initial materials to white
            currentWalls[i] = Instantiate(wallPrefabs[i], spawnPoints[i].position, spawnPoints[i].rotation);
            currentWallRenderers[i] = currentWalls[i].GetComponent<Renderer>();

            // Use unique instances of the white material for each wall
            currentMaterials[i] = new Material(initialMaterial);
            currentWallRenderers[i].material = currentMaterials[i];
        }
    }

    // Method to select a wall
    public void SelectWall(int index)
    {
        if (index < wallPrefabs.Length)
        {
            currentWallIndex = index;
        }
        else
        {
            Debug.LogError("Index out of range for wallPrefabs");
        }
    }

    // Method to apply a material to the currently selected wall
    public void ApplyMaterial(int materialIndex)
    {
        if (currentWallIndex >= 0 && currentWallIndex < currentWallRenderers.Length && materialIndex < wallMaterials.Length)
        {
            Material newMaterial = new Material(wallMaterials[materialIndex]);
            currentWallRenderers[currentWallIndex].material = newMaterial;
            currentMaterials[currentWallIndex] = newMaterial;
        }
        else
        {
            Debug.LogError("Material index out of range or no current wall selected");
        }
    }


}
