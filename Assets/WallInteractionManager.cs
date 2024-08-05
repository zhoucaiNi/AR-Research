using UnityEngine;

public class WallInteractionManager : MonoBehaviour
{
    public OVRInput.Controller leftController = OVRInput.Controller.LTouch;
    public OVRInput.Controller rightController = OVRInput.Controller.RTouch;
    public Material highlightMaterial;
    public GameObject pokeCanvas; // Reference to the existing UI panel

    private Renderer lastHitRenderer;
    private Material lastOriginalMaterial;
    private int currentWallIndex = -1;
    private bool isHighlighting = false;

    void Update()
    {
        HandleControllerRaycast(leftController);
        HandleControllerRaycast(rightController);
    }

    void HandleControllerRaycast(OVRInput.Controller controller)
    {
        Vector3 controllerPosition = OVRInput.GetLocalControllerPosition(controller);
        Quaternion controllerRotation = OVRInput.GetLocalControllerRotation(controller);
        Vector3 controllerDirection = controllerRotation * Vector3.forward;

        Ray ray = new Ray(controllerPosition, controllerDirection);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                Renderer hitRenderer = hit.collider.GetComponent<Renderer>();

                if (hitRenderer != lastHitRenderer)
                {
                    ClearHighlight();
                    lastOriginalMaterial = hitRenderer.material;
                    hitRenderer.material = highlightMaterial;
                    lastHitRenderer = hitRenderer;
                    currentWallIndex = hit.collider.gameObject.GetInstanceID();
                    isHighlighting = true;
                }

                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller))
                {
                    // Select the wall and make the pokeCanvas visible for interaction
                    Debug.Log("Wall selected: " + hit.collider.name);
                }
            }
            else if (isHighlighting)
            {
                ClearHighlight();
            }
        }
        else if (isHighlighting)
        {
            ClearHighlight();
        }
    }

    void ClearHighlight()
    {
        if (lastHitRenderer != null)
        {
            lastHitRenderer.material = lastOriginalMaterial;
            lastHitRenderer = null;
            isHighlighting = false;
        }
    }

    public void ApplyMaterial(Material material)
    {
        if (lastHitRenderer != null)
        {
            lastHitRenderer.material = material;
            lastOriginalMaterial = material;
        }
    }
}
