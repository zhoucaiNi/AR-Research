using UnityEngine;

public class WallSelectorRaycast : MonoBehaviour
{
    public OVRInput.Controller leftController = OVRInput.Controller.LTouch; // Left controller
    public OVRInput.Controller rightController = OVRInput.Controller.RTouch; // Right controller
    public WallSelector wallSelector; // Reference to the WallSelector script
    public Material highlightMaterial; // Highlight material for raycast hit

    private Renderer lastHitRenderer;
    private Material lastOriginalMaterial;
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
                    isHighlighting = true;
                }

                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller))
                {
                    wallSelector.SelectWall(hit.collider.gameObject.GetInstanceID());
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
}
