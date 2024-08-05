  using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.XR.MRUtilityKit;


public class displayLabel : MonoBehaviour
{

    public Transform rayStartPoint;
    
    public float rayLength = 5;
    public MRUKAnchor.SceneLabels labelFilter;
    public TMPro.TextMeshPro debugText;

    public Material[] wallMaterials; // Array to store different materials
    private List<Renderer> currentEffectMeshRenderers = new List<Renderer>(); // List to store the renderers of the currently selected effect meshes
    private int currentMaterialIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame 
    void Update()
    {
        Ray ray = new Ray(rayStartPoint.position, rayStartPoint.forward);
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();

        if (room == null)
        {
            Debug.LogError("MRUKRoom is null. Ensure the room is correctly initialized.");
            //return;
        }

        bool hasHit = room.Raycast(ray, rayLength, LabelFilter.Included(labelFilter), out RaycastHit hit, out MRUKAnchor anchor);

        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            Vector3 hitPoint = hit.point;
            Vector3 hitNormal = hit.normal;

            string label = anchor.AnchorLabels[0];

            debugText.transform.position = hitPoint;
            debugText.transform.rotation = Quaternion.LookRotation(-hitNormal);

            debugText.text = "ANCHOR : " + label;

            //Debug.Log("Hit anchor: " + anchor.transform.name);

            // Use MRUKRoom.GetWallAnchors() to get all wall anchors
            List<MRUKAnchor> wallAnchors = room.GetWallAnchors();
            Debug.Log(wallAnchors[0]);
            Debug.Log(anchor.transform);

            foreach (MRUKAnchor wallAnchor in wallAnchors)
            {
                // Check if this wall anchor is the one we hit
                if (wallAnchor.transform == anchor.transform)
                {
                    //Debug.Log("Matching wall anchor found: " + wallAnchor.transform.name);

                    // Find all child objects with "EffectMesh" in their names
                    Renderer[] meshRenderers = wallAnchor.transform.GetComponentsInChildren<Renderer>();


                    foreach (Renderer meshRenderer in meshRenderers)
                    {
                        Material newMaterial = new Material(wallMaterials[currentMaterialIndex]); // Assuming yellow material is at index 0
                        meshRenderer.material = newMaterial;
                        currentEffectMeshRenderers.Add(meshRenderer); // Store the current effect mesh renderer
                        //if (meshRenderer.gameObject.name.Contains("EffectMesh"))
                        //{
                        //    Material newMaterial = new Material(wallMaterials[currentMaterialIndex]); // Assuming yellow material is at index 0
                        //    meshRenderer.material = newMaterial;
                        //    currentEffectMeshRenderers.Add(meshRenderer); // Store the current effect mesh renderer

                        //    Debug.Log("Material changed for: " + meshRenderer.gameObject.name);
                        //} else if (meshRenderer.gameObject.name.Contains("TileWall") || meshRenderer.gameObject.name.Contains("Cube"))
                        //{
                        //    Material newMaterial = new Material(wallMaterials[currentMaterialIndex]); // Assuming yellow material is at index 0
                        //    meshRenderer.material = newMaterial;
                        //    currentEffectMeshRenderers.Add(meshRenderer); // Store the current effect mesh renderer
                        //}
                    }
                }
            }

            currentMaterialIndex = (currentMaterialIndex + 1) % wallMaterials.Length;

        }
        else
        {
            Debug.Log("No hit detected.");
        }

    }

    // Method to apply a material to the currently selected effect meshes
    public void ApplyMaterial(int materialIndex)
    {
        if (materialIndex < wallMaterials.Length)
        {
            Material newMaterial = new Material(wallMaterials[materialIndex]);
            foreach (Renderer renderer in currentEffectMeshRenderers)
            {
                renderer.material = newMaterial;
            }
        }
        else
        {
            Debug.LogError("Material index out of range or no current effect meshes selected");
        }
    }
}
