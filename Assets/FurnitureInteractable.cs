using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FurnitureInteractable : MonoBehaviour
{
    private int rotationState = 0;

    public void OnSelectEnter(SelectEnterEventArgs args)
    {
        RotateFurniture();
    }

    private void RotateFurniture()
    {
        rotationState = (rotationState + 1) % 4;
        transform.rotation = Quaternion.Euler(0, rotationState * 90, 0);
    }
}
