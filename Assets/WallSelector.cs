using UnityEngine;

[System.Serializable]
public class MaterialTilingSettings
{
    public Material material;
    public Vector2 tileDimensions; // Dimensions of the tile in meters (width x height)
}

public class WallSelector : MonoBehaviour
{
    public GameObject[] wallPrefabs; // Array to store wall prefabs
    public Transform[] spawnPoints; // Array to store spawn points for each wall prefab
    public MaterialTilingSettings[] materialSettings; // Settings for materials and their tile dimensions

    private GameObject[] currentWalls; // Array to store instantiated wall objects
    private Renderer[] currentWallRenderers; // Array to store renderers of the instantiated walls
    private int currentWallIndex = -1; // Index of the currently selected wall

    const int defaultMaterialIndex = 0;  //Define a default Material index

    void Start()
    {
        InitializeWalls();
    }

    void InitializeWalls()
    {
        currentWalls = new GameObject[wallPrefabs.Length];
        currentWallRenderers = new Renderer[wallPrefabs.Length];

        for (int i = 0; i < wallPrefabs.Length; i++)
        {
            currentWalls[i] = Instantiate(wallPrefabs[i], spawnPoints[i].position, spawnPoints[i].rotation);
            currentWallRenderers[i] = currentWalls[i].GetComponent<Renderer>();
            currentWallRenderers[i].material = new Material(materialSettings[i].material);
            SetTileSize(i, defaultMaterialIndex);
        }
    }

    public void SelectWall(int index)
    {
        if (index >= 0 && index < wallPrefabs.Length)
        {
            currentWallIndex = index;
        }
        else
        {
            Debug.LogError("Index out of range for wallPrefabs");
        }
    }

    public void ApplyMaterial(int materialIndex)
    {
        if (currentWallIndex >= 0 && currentWallIndex < currentWallRenderers.Length && materialIndex < materialSettings.Length)
        {
            Renderer renderer = currentWallRenderers[currentWallIndex];
            renderer.material = new Material(materialSettings[materialIndex].material);
            SetTileSize(currentWallIndex, materialIndex);  // update including materialIndex
            Debug.Log($"Material applied: {renderer.material.name}");
            Debug.Log($"Texture scale: {renderer.material.GetTextureScale("_MainTex")}");
        }
        else
        {
            Debug.LogError("Material index out of range or no current wall selected");
        }
    }

    private void SetTileSize(int wallIndex, int materialIndex) // include materialIndex 
    {
        Renderer renderer = currentWallRenderers[wallIndex];
        Vector3 wallSize = renderer.bounds.size;

        MaterialTilingSettings settings = materialSettings[materialIndex]; // use materialIndex instead of wallIndex
        Debug.Log($"Wall {wallIndex} size: {wallSize}");
        Debug.Log($"Tile dimensions for material {materialIndex}: {settings.tileDimensions}");

        Vector2 tiling;
        if (wallSize.y >= wallSize.x && wallSize.z >= wallSize.x)
        {
            tiling = new Vector2(
                wallSize.y / settings.tileDimensions.x,
                wallSize.z / settings.tileDimensions.y
            );
        }
        else if (wallSize.x >= wallSize.y && wallSize.z >= wallSize.y)
        {
            tiling = new Vector2(
                wallSize.x / settings.tileDimensions.x,
                wallSize.z / settings.tileDimensions.y
            );
        }
        else
        {
            tiling = new Vector2(
                wallSize.x / settings.tileDimensions.x,
                wallSize.y / settings.tileDimensions.y
            );
        }

        renderer.material.SetTextureScale("_MainTex", tiling);
        Debug.Log($"Adjusted Tiling: {tiling} on wall index: {wallIndex} with material index: {materialIndex}");
    }

    public void RotateTexture(float angleDegrees)
    {
        if (currentWallIndex >= 0 && currentWallIndex < currentWallRenderers.Length)
        {
            Renderer renderer = currentWallRenderers[currentWallIndex];
            Material mat = renderer.material;

            // Get the current rotation value from the material
            float currentRotation = mat.HasProperty("_Rotation") ? mat.GetFloat("_Rotation") : 0;

            // Calculate the new rotation angle
            float newRotation = (currentRotation + angleDegrees) % 360;
            mat.SetFloat("_Rotation", newRotation);

            // Adjust the texture scale based on the rotation
            if (Mathf.Abs(newRotation % 360) == 90 || Mathf.Abs(newRotation % 360) == 270)
            {
                // Perpendicular rotation - Swap the texture scale
                Vector2 currentScale = mat.GetTextureScale("_MainTex");
                mat.SetTextureScale("_MainTex", new Vector2(currentScale.y, currentScale.x));
            }
            // For 180 or 360 degrees rotation, no change in tiling is needed
            else if (Mathf.Abs(newRotation % 360) == 180 || Mathf.Abs(newRotation % 360) == 0)
            {
                // No change in texture scale is needed
            }

            Debug.Log($"Rotated texture on wall index: {currentWallIndex} by {angleDegrees} degrees. New rotation: {newRotation}");
        }
        else
        {
            Debug.LogError("No wall selected or index out of range for texture rotation");
        }
    }
}

