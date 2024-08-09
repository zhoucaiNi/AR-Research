using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  // For TextMeshPro functionality
using Meta.XR.MRUtilityKit;  // Assuming correct namespace for MRUK functionality

[System.Serializable]
public class MaterialTilingSettings
{
    public Vector2 tileDimensions; // Dimensions of the tile in meters (width x height)
}

public class displayLabel : MonoBehaviour
{
    public Transform rayStartPoint;
    public float rayLength = 5;
    public MRUKAnchor.SceneLabels labelFilter;
    public TextMeshPro debugText;
    public Material[] wallMaterials; // Array to store different materials
    public MaterialTilingSettings[] materialSettings; // Array to store tiling settings for each material
    //public GameObject passthroughLayerObject; // This should have the OVRPassthroughLayer component if using Oculus Passthrough

    private List<Renderer> currentEffectMeshRenderers = new List<Renderer>();
    private int currentWallIndex = -1;

    void Update()
    {
        Ray ray = new Ray(rayStartPoint.position, rayStartPoint.forward);
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();

        if (room == null)
        {
            Debug.LogError("MRUKRoom is null. Ensure the room is correctly initialized.");
            return;
        }

        Debug.Log("Performing raycast...");
        bool hasHit = room.Raycast(ray, rayLength, LabelFilter.Included(labelFilter), out RaycastHit hit, out MRUKAnchor anchor);

        if (hasHit)
        {
            Debug.Log("Raycast hit detected.");
            if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
            {
                Vector3 hitPoint = hit.point;
                Vector3 hitNormal = hit.normal;
                string label = anchor.AnchorLabels[0]; // Assuming 'Label' is a public property

                debugText.transform.position = hitPoint;
                debugText.transform.rotation = Quaternion.LookRotation(-hitNormal);
                debugText.text = "ANCHOR: " + label;

                currentWallIndex = anchor.GetInstanceID(); // Use the instance ID to track the current wall
                Debug.Log($"Wall selected with instance ID: {currentWallIndex}");
            }
        }
        else
        {
            Debug.Log("No hit detected.");
        }
    }

    public void ApplyMaterial(int materialIndex)
    {
        Debug.Log($"ApplyMaterial called with index: {materialIndex}");

        if (currentWallIndex == -1)
        {
            Debug.LogError("No wall selected.");
            return;
        }

        MRUKAnchor anchor = FindAnchorByInstanceID(currentWallIndex);
        if (anchor == null)
        {
            Debug.LogError("Anchor not found.");
            return;
        }

        Renderer[] meshRenderers = anchor.gameObject.GetComponentsInChildren<Renderer>();
        if (meshRenderers.Length > 0)
        {
            currentEffectMeshRenderers.Clear(); // Clear the list before adding new renderers
            foreach (Renderer renderer in meshRenderers)
            {
                if (materialIndex < wallMaterials.Length)
                {
                    renderer.material = wallMaterials[materialIndex];
                    currentEffectMeshRenderers.Add(renderer);
                    SetTileSize(renderer, materialIndex);
                }
            }
        }
        else
        {
            Debug.Log("No renderers found on the anchor object.");
        }
    }

    private void SetTileSize(Renderer renderer, int materialIndex)
    {
        Vector3 wallSize = renderer.bounds.size;
        MaterialTilingSettings settings = materialSettings[materialIndex];

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

        renderer.material.SetFloat("_XTiling", tiling.x);
        renderer.material.SetFloat("_YTiling", tiling.y);
        Debug.Log($"Adjusted Tiling: {tiling} on renderer with material index: {materialIndex}");
    }





    private MRUKAnchor FindAnchorByInstanceID(int instanceID)
    {
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        if (room == null)
        {
            Debug.LogError("MRUKRoom is null.");
            return null;
        }

        foreach (var anchor in room.Anchors)
        {
            if (anchor.GetInstanceID() == instanceID)
            {
                return anchor;
            }
        }

        Debug.LogError("Anchor with the specified instance ID not found.");
        return null;
    }

    public void RotateTexture(float angleDegrees)
    {
        if (currentEffectMeshRenderers.Count > 0)
        {
            foreach (Renderer renderer in currentEffectMeshRenderers)
            {
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
                    float xTiling = mat.GetFloat("_XTiling");
                    float yTiling = mat.GetFloat("_YTiling");
                    mat.SetFloat("_XTiling", yTiling);
                    mat.SetFloat("_YTiling", xTiling);
                }

                Debug.Log($"Rotated texture on renderer with material: {mat.name} by {angleDegrees} degrees. New rotation: {newRotation}");
            }
        }
        else
        {
            Debug.LogError("No walls selected for texture rotation.");
        }
    }
}
