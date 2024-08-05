using UnityEngine;

public class VisibilityToggle : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false); // Ensure the GameObject is off at start
    }

    public void ToggleVisibility()
    {
        gameObject.SetActive(!gameObject.activeSelf); // Toggle the active state
    }
}
